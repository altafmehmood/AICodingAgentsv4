
using Microsoft.Extensions.Logging;
using Xunit;
using NSubstitute;
using System.Threading.Tasks;
using Breach.Api.Features.Breaches;

using System.Threading;
using Flurl.Http.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Breach.Api.Tests
{
    public class GetAllBreachesTests
    {
        [Fact]
        public async Task Handle_ReturnsAllBreaches_WhenApiCallIsSuccessful()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetAllBreaches.Handler>>();
            var handler = new GetAllBreaches.Handler(logger);
            var query = new GetAllBreaches.Query(null, null);

            var expectedBreaches = new List<GetAllBreaches.Breach>
            {
                new GetAllBreaches.Breach
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
                },
                new GetAllBreaches.Breach
                {
                    Name = "Canva",
                    Title = "Canva",
                    Domain = "canva.com",
                    BreachDate = new DateTime(2019, 1, 17),
                    PwnCount = 137000000,
                    Description = "Description 2",
                    DataClasses = new[] { "Email addresses" },
                    IsVerified = true,
                    IsFabricated = false,
                    IsSensitive = false,
                    IsRetired = false,
                    IsSpamList = false,
                    LogoPath = "path2"
                }
            };

            httpTest.RespondWithJson(expectedBreaches);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBreaches.Count, result.Count());
            httpTest.ShouldHaveCalled("https://haveibeenpwned.com/api/v3/breaches");
        }

        [Fact]
        public async Task Handle_ReturnsFilteredBreaches_WhenFromDateIsProvided()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetAllBreaches.Handler>>();
            var handler = new GetAllBreaches.Handler(logger);
            var fromDate = new DateTime(2015, 1, 1);
            var query = new GetAllBreaches.Query(fromDate, null);

            var expectedBreaches = new List<GetAllBreaches.Breach>
            {
                new GetAllBreaches.Breach
                {
                    Name = "Canva",
                    Title = "Canva",
                    Domain = "canva.com",
                    BreachDate = new DateTime(2019, 1, 17),
                    PwnCount = 137000000,
                    Description = "Description 2",
                    DataClasses = new[] { "Email addresses" },
                    IsVerified = true,
                    IsFabricated = false,
                    IsSensitive = false,
                    IsRetired = false,
                    IsSpamList = false,
                    LogoPath = "path2"
                }
            };

            httpTest.RespondWithJson(expectedBreaches);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBreaches.Count, result.Count());
            httpTest.ShouldHaveCalled($"https://haveibeenpwned.com/api/v3/breaches?from={fromDate.ToString("yyyy-MM-dd")}");
        }

        [Fact]
        public async Task Handle_ReturnsFilteredBreaches_WhenToDateIsProvided()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetAllBreaches.Handler>>();
            var handler = new GetAllBreaches.Handler(logger);
            var toDate = new DateTime(2015, 1, 1);
            var query = new GetAllBreaches.Query(null, toDate);

            var expectedBreaches = new List<GetAllBreaches.Breach>
            {
                new GetAllBreaches.Breach
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
            httpTest.ShouldHaveCalled($"https://haveibeenpwned.com/api/v3/breaches?to={toDate.ToString("yyyy-MM-dd")}");
        }

        [Fact]
        public async Task Handle_ReturnsFilteredBreaches_WhenBothDatesAreProvided()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetAllBreaches.Handler>>();
            var handler = new GetAllBreaches.Handler(logger);
            var fromDate = new DateTime(2015, 1, 1);
            var toDate = new DateTime(2020, 1, 1);
            var query = new GetAllBreaches.Query(fromDate, toDate);

            var expectedBreaches = new List<GetAllBreaches.Breach>
            {
                new GetAllBreaches.Breach
                {
                    Name = "Canva",
                    Title = "Canva",
                    Domain = "canva.com",
                    BreachDate = new DateTime(2019, 1, 17),
                    PwnCount = 137000000,
                    Description = "Description 2",
                    DataClasses = new[] { "Email addresses" },
                    IsVerified = true,
                    IsFabricated = false,
                    IsSensitive = false,
                    IsRetired = false,
                    IsSpamList = false,
                    LogoPath = "path2"
                }
            };

            httpTest.RespondWithJson(expectedBreaches);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBreaches.Count, result.Count());
            httpTest.ShouldHaveCalled($"https://haveibeenpwned.com/api/v3/breaches?from={fromDate.ToString("yyyy-MM-dd")}&to={toDate.ToString("yyyy-MM-dd")}");
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenApiReturns404()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var logger = Substitute.For<ILogger<GetAllBreaches.Handler>>();
            var handler = new GetAllBreaches.Handler(logger);
            var query = new GetAllBreaches.Query(null, null);

            httpTest.RespondWith("", 404);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            httpTest.ShouldHaveCalled("https://haveibeenpwned.com/api/v3/breaches");
        }
    }
}
