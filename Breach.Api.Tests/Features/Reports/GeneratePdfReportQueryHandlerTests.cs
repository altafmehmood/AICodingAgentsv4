using Breach.Api.Features.Reports;
using Breach.Api.Models;
using Breach.Api.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Breach.Api.Tests.Features.Reports;

public class GeneratePdfReportQueryHandlerTests
{
    private readonly IHaveIBeenPwnedService _haveIBeenPwnedService;
    private readonly IClaudeService _claudeService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<GeneratePdfReportQueryHandler> _logger;
    private readonly GeneratePdfReportQueryHandler _handler;

    public GeneratePdfReportQueryHandlerTests()
    {
        _haveIBeenPwnedService = Substitute.For<IHaveIBeenPwnedService>();
        _claudeService = Substitute.For<IClaudeService>();
        _pdfService = Substitute.For<IPdfService>();
        _logger = Substitute.For<ILogger<GeneratePdfReportQueryHandler>>();
        
        _handler = new GeneratePdfReportQueryHandler(
            _haveIBeenPwnedService,
            _claudeService,
            _pdfService,
            _logger);
    }

    [Fact]
    public async Task Handle_WithValidEmail_ShouldReturnPdfBytes()
    {
        // Arrange
        var email = "test@example.com";
        var request = new GeneratePdfReportQuery { Email = email };
        var breaches = CreateSampleBreaches();
        var riskAnalysis = CreateSampleRiskAnalysis(email, breaches);
        var expectedPdfBytes = new byte[] { 1, 2, 3, 4, 5 }; // Mock PDF data

        _haveIBeenPwnedService.GetBreachesForEmailAsync(email)
            .Returns(breaches);
        
        _claudeService.AnalyzeRiskAsync(email, breaches)
            .Returns(riskAnalysis);
        
        _pdfService.GenerateRiskAnalysisReportAsync(riskAnalysis)
            .Returns(expectedPdfBytes);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPdfBytes, result);
        
        await _haveIBeenPwnedService.Received(1).GetBreachesForEmailAsync(email);
        await _claudeService.Received(1).AnalyzeRiskAsync(email, breaches);
        await _pdfService.Received(1).GenerateRiskAnalysisReportAsync(riskAnalysis);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Handle_WithInvalidEmail_ShouldThrowArgumentException(string? email)
    {
        // Arrange
        var request = new GeneratePdfReportQuery { Email = email! };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(request, CancellationToken.None));
        
        Assert.Equal("Email address is required for PDF report generation", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenHaveIBeenPwnedServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var request = new GeneratePdfReportQuery { Email = email };
        var expectedException = new HttpRequestException("API error");

        _haveIBeenPwnedService.GetBreachesForEmailAsync(email)
            .Returns(Task.FromException<List<BreachData>>(expectedException));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<HttpRequestException>(
            () => _handler.Handle(request, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task Handle_WhenClaudeServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var request = new GeneratePdfReportQuery { Email = email };
        var breaches = CreateSampleBreaches();
        var expectedException = new InvalidOperationException("Claude API error");

        _haveIBeenPwnedService.GetBreachesForEmailAsync(email)
            .Returns(breaches);
        
        _claudeService.AnalyzeRiskAsync(email, breaches)
            .Returns(Task.FromException<RiskAnalysis>(expectedException));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(request, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, exception.Message);
    }

    [Fact]
    public async Task Handle_WhenPdfServiceThrows_ShouldPropagateException()
    {
        // Arrange
        var email = "test@example.com";
        var request = new GeneratePdfReportQuery { Email = email };
        var breaches = CreateSampleBreaches();
        var riskAnalysis = CreateSampleRiskAnalysis(email, breaches);
        var expectedException = new InvalidOperationException("PDF generation error");

        _haveIBeenPwnedService.GetBreachesForEmailAsync(email)
            .Returns(breaches);
        
        _claudeService.AnalyzeRiskAsync(email, breaches)
            .Returns(riskAnalysis);
        
        _pdfService.GenerateRiskAnalysisReportAsync(riskAnalysis)
            .Returns(Task.FromException<byte[]>(expectedException));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(request, CancellationToken.None));
        
        Assert.Equal(expectedException.Message, exception.Message);
    }

    private static List<BreachData> CreateSampleBreaches()
    {
        return new List<BreachData>
        {
            new()
            {
                Name = "TestBreach",
                Title = "Test Breach",
                Domain = "testsite.com",
                BreachDate = new DateTime(2023, 1, 15),
                AddedDate = new DateTime(2023, 1, 20),
                ModifiedDate = new DateTime(2023, 1, 20),
                PwnCount = 1000000,
                Description = "A test breach for demonstration purposes",
                DataClasses = new[] { "Email addresses", "Passwords" },
                IsVerified = true,
                IsFabricated = false,
                IsSensitive = false,
                IsRetired = false,
                IsSpamList = false,
                IsMalware = false,
                LogoPath = ""
            }
        };
    }

    private static RiskAnalysis CreateSampleRiskAnalysis(string email, List<BreachData> breaches)
    {
        return new RiskAnalysis
        {
            EmailAddress = email,
            TotalBreaches = breaches.Count,
            RiskLevel = RiskLevel.Medium,
            Summary = "Test risk analysis",
            Recommendations = new[] { "Change passwords", "Enable 2FA" },
            AnalysisDate = DateTime.UtcNow,
            Breaches = breaches
        };
    }
} 