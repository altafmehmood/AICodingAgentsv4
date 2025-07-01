
using Xunit;
using NSubstitute;
using System.Threading.Tasks;
using Breach.Api.Features.Breaches;
using Microsoft.Extensions.Logging;
using System.Threading;
using Flurl.Http.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Breach.Api.Tests
{
    public class GetBreachesByEmailTests
    {
        [Fact]
        public async Task Handle_ReturnsBreaches_WhenApiCallIsSuccessful()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetBreachesByEmail.Handler>>();
            var handler = new GetBreachesByEmail.Handler(logger);
            var query = new GetBreachesByEmail.Query("test@example.com");

            var expectedBreaches = new List<GetBreachesByEmail.Breach>
            {
                new GetBreachesByEmail.Breach
                {
                    Name = "Adobe",
                    Title = "Adobe",
                    Domain = "adobe.com",
                    BreachDate = new DateTime(2013, 10, 4),
                    PwnCount = 152445165,
                    Description = "Description 1",
                    DataClasses = new[] { "Email addresses" },
                    IsVerified = true,
                    IsFabricated = false,
                    IsSensitive = false,
                    IsRetired = false,
                    IsSpamList = false,
                    LogoPath = "path1"
                }
            };

            httpTest.RespondWithJson(expectedBreaches);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBreaches.Count, result.Count());
            httpTest.ShouldHaveCalled("https://haveibeenpwned.com/api/v3/breachedaccount/test@example.com");
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenApiReturns404()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetBreachesByEmail.Handler>>();
            var handler = new GetBreachesByEmail.Handler(logger);
            var query = new GetBreachesByEmail.Query("nonexistent@example.com");

            httpTest.RespondWith("", 404);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            httpTest.ShouldHaveCalled("https://haveibeenpwned.com/api/v3/breachedaccount/nonexistent@example.com");
        }

        [Fact]
        public async Task Handle_ThrowsException_WhenApiReturnsNon404Error()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetBreachesByEmail.Handler>>();
            var handler = new GetBreachesByEmail.Handler(logger);
            var query = new GetBreachesByEmail.Query("error@example.com");

            httpTest.RespondWith("Internal Server Error", 500);

            // Act & Assert
            await Assert.ThrowsAsync<Flurl.Http.FlurlHttpException>(() => handler.Handle(query, CancellationToken.None));
            httpTest.ShouldHaveCalled("https://haveibeenpwned.com/api/v3/breachedaccount/error@example.com");
        }
    }
}
