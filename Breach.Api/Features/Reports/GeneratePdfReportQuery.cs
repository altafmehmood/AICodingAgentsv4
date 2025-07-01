using Breach.Api.Models;
using Breach.Api.Services;
using MediatR;

namespace Breach.Api.Features.Reports;

public class GeneratePdfReportQuery : IRequest<byte[]>
{
    public string Email { get; set; } = string.Empty;
}

public class GeneratePdfReportQueryHandler : IRequestHandler<GeneratePdfReportQuery, byte[]>
{
    private readonly IHaveIBeenPwnedService _haveIBeenPwnedService;
    private readonly IClaudeService _claudeService;
    private readonly IPdfService _pdfService;
    private readonly ILogger<GeneratePdfReportQueryHandler> _logger;

    public GeneratePdfReportQueryHandler(
        IHaveIBeenPwnedService haveIBeenPwnedService,
        IClaudeService claudeService,
        IPdfService pdfService,
        ILogger<GeneratePdfReportQueryHandler> logger)
    {
        _haveIBeenPwnedService = haveIBeenPwnedService;
        _claudeService = claudeService;
        _pdfService = pdfService;
        _logger = logger;
    }

    public async Task<byte[]> Handle(GeneratePdfReportQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GeneratePdfReportQuery for email: {Email}", request.Email);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("Empty email provided for PDF report generation");
            throw new ArgumentException("Email address is required for PDF report generation");
        }

        try
        {
            // Get the breaches for this email
            var breaches = await _haveIBeenPwnedService.GetBreachesForEmailAsync(request.Email);
            
            // Analyze the risk using Claude AI
            var riskAnalysis = await _claudeService.AnalyzeRiskAsync(request.Email, breaches);
            
            // Generate PDF report
            var pdfBytes = await _pdfService.GenerateRiskAnalysisReportAsync(riskAnalysis);
            
            _logger.LogInformation("PDF report generated successfully for email: {Email}, Size: {Size} bytes", 
                request.Email, pdfBytes.Length);
            
            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF report for email: {Email}", request.Email);
            throw;
        }
    }
} 