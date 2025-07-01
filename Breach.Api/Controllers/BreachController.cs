using Breach.Api.Features.Breaches;
using Breach.Api.Features.RiskAnalysis;
using Breach.Api.Features.Reports;
using Breach.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Breach.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BreachController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BreachController> _logger;

    public BreachController(IMediator mediator, ILogger<BreachController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all available breaches
    /// </summary>
    /// <returns>List of all breaches</returns>
    [HttpGet]
    public async Task<ActionResult<List<BreachData>>> GetAllBreaches()
    {
        _logger.LogInformation("Getting all breaches");
        
        var query = new GetAllBreachesQuery();
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Get breaches for a specific email address
    /// </summary>
    /// <param name="email">Email address to search for</param>
    /// <returns>List of breaches affecting the email</returns>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<List<BreachData>>> GetBreachesForEmail(string email)
    {
        _logger.LogInformation("Getting breaches for email: {Email}", email);
        
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email address is required");
        }

        var query = new GetBreachesForEmailQuery { Email = email };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Analyze risk for a specific email address
    /// </summary>
    /// <param name="email">Email address to analyze</param>
    /// <returns>Risk analysis including breaches and recommendations</returns>
    [HttpGet("analyze/{email}")]
    public async Task<ActionResult<Models.RiskAnalysis>> AnalyzeRisk(string email)
    {
        _logger.LogInformation("Analyzing risk for email: {Email}", email);
        
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email address is required");
        }

        var query = new AnalyzeRiskQuery { Email = email };
        var result = await _mediator.Send(query);
        
        return Ok(result);
    }

    /// <summary>
    /// Generate and download a PDF risk analysis report for a specific email address
    /// </summary>
    /// <param name="email">Email address to generate report for</param>
    /// <returns>PDF file with risk analysis report</returns>
    [HttpGet("report/{email}")]
    public async Task<IActionResult> GeneratePdfReport(string email)
    {
        _logger.LogInformation("Generating PDF report for email: {Email}", email);
        
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest("Email address is required");
        }

        try
        {
            var query = new GeneratePdfReportQuery { Email = email };
            var pdfBytes = await _mediator.Send(query);
            
            var fileName = $"Risk_Analysis_Report_{email.Replace("@", "_at_")}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
            
            return File(pdfBytes, "application/pdf", fileName);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for PDF report generation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF report for email: {Email}", email);
            return StatusCode(500, "An error occurred while generating the PDF report");
        }
    }
}