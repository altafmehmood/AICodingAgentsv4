
using Xunit;
using NSubstitute;
using System.Threading.Tasks;
using Breach.Api.Features.Breaches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading;
using Flurl.Http.Testing;

namespace Breach.Api.Tests
{
    public class GetRiskAnalysisTests
    {
        [Fact]
        public async Task Handle_ReturnsRiskAnalysis_WhenApiCallIsSuccessful()
        {
            // Arrange
            using var httpTest = new HttpTest();
            var configuration = Substitute.For<IConfiguration>();
            configuration["ClaudeApiKey"].Returns("test_key");
            var logger = Substitute.For<ILogger<GetRiskAnalysis.Handler>>();

            var handler = new GetRiskAnalysis.Handler(configuration, logger);
            var query = new GetRiskAnalysis.Query("Some breach description");

            var expectedResponse = new GetRiskAnalysis.ClaudeResponse
            {
                Content = new[]
                {
                    new GetRiskAnalysis.Content
                    {
                        Text = "This is a risk analysis."
                    }
                }
            };

            httpTest.RespondWithJson(expectedResponse);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal("This is a risk analysis.", result);
            httpTest.ShouldHaveCalled("https://api.anthropic.com/v1/messages");
        }
    }
}
