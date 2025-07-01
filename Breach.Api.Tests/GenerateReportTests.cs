
using Xunit;
using System.Threading.Tasks;
using Breach.Api.Features.Breaches;
using System.Threading;
using System;

namespace Breach.Api.Tests
{
    public class GenerateReportTests
    {
        [Fact]
        public async Task Handle_ReturnsPdf_WhenCalledWithBreachAndRiskAnalysis()
        {
            // Arrange
            var breach = new GetBreachByName.Breach
            {
                Name = "Adobe",
                Title = "Adobe",
                Domain = "adobe.com",
                BreachDate = new DateTime(2013, 10, 4),
                PwnCount = 152445165,
                Description = "In October 2013, 153 million Adobe accounts were breached with each containing an internal ID, username, email, encrypted password and a password hint in plain text. The password cryptography was poorly done and many were quickly resolved back to plain text. The unencrypted hints also disclosed much about the passwords adding further to the risk.",
                DataClasses = new[] { "Email addresses", "Password hints", "Passwords", "Usernames" },
                IsVerified = true,
                IsFabricated = false,
                IsSensitive = false,
                IsRetired = false,
                IsSpamList = false,
                LogoPath = "https://haveibeenpwned.com/Content/Images/PwnedLogos/Adobe.png"
            };
            var riskAnalysis = "This is a risk analysis.";

            var handler = new GenerateReport.Handler();
            var query = new GenerateReport.Query(breach, riskAnalysis);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }
    }
}
