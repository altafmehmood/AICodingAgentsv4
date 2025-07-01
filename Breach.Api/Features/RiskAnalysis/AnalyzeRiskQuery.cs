using Breach.Api.Models;
using Breach.Api.Services;
using MediatR;

namespace Breach.Api.Features.RiskAnalysis;

public class AnalyzeRiskQuery : IRequest<Models.RiskAnalysis>
{
    public string Email { get; set; } = string.Empty;
}

public class AnalyzeRiskQueryHandler : IRequestHandler<AnalyzeRiskQuery, Models.RiskAnalysis>
{
    private readonly IHaveIBeenPwnedService _haveIBeenPwnedService;
    private readonly IClaudeService _claudeService;
    private readonly ILogger<AnalyzeRiskQueryHandler> _logger;

    public AnalyzeRiskQueryHandler(
        IHaveIBeenPwnedService haveIBeenPwnedService,
        IClaudeService claudeService,
        ILogger<AnalyzeRiskQueryHandler> logger)
    {
        _haveIBeenPwnedService = haveIBeenPwnedService;
        _claudeService = claudeService;
        _logger = logger;
    }

    public async Task<Models.RiskAnalysis> Handle(AnalyzeRiskQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AnalyzeRiskQuery for email: {Email}", request.Email);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("Empty email provided for risk analysis");
            return new Models.RiskAnalysis
            {
                EmailAddress = request.Email,
                RiskLevel = RiskLevel.Low,
                Summary = "No email provided for analysis",
                AnalysisDate = DateTime.UtcNow
            };
        }

        // First, get the breaches for this email
        var breaches = await _haveIBeenPwnedService.GetBreachesForEmailAsync(request.Email);
        
        // Then, analyze the risk using Claude AI
        var riskAnalysis = await _claudeService.AnalyzeRiskAsync(request.Email, breaches);
        
        _logger.LogInformation("Risk analysis completed for email: {Email}, Risk Level: {RiskLevel}", 
            request.Email, riskAnalysis.RiskLevel);
        
        return riskAnalysis;
    }
}