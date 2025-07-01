using Breach.Api.Models;
using HandlebarsDotNet;
using PuppeteerSharp;
using System.Reflection;

namespace Breach.Api.Services;

public class PdfService : IPdfService
{
    private readonly ILogger<PdfService> _logger;
    private readonly string _templatePath;

    public PdfService(ILogger<PdfService> logger)
    {
        _logger = logger;
        _templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
    }

    public async Task<byte[]> GenerateRiskAnalysisReportAsync(RiskAnalysis riskAnalysis)
    {
        ArgumentNullException.ThrowIfNull(riskAnalysis);
        
        try
        {
            _logger.LogInformation("Generating PDF report for email: {Email}", riskAnalysis.EmailAddress);

            // Ensure Puppeteer browser is downloaded
            await EnsureBrowserAsync();

            // Generate HTML content from template
            var htmlContent = await GenerateHtmlContentAsync(riskAnalysis);

            // Generate PDF from HTML
            var pdfBytes = await GeneratePdfFromHtmlAsync(htmlContent);

            _logger.LogInformation("PDF report generated successfully for email: {Email}, Size: {Size} bytes", 
                riskAnalysis.EmailAddress, pdfBytes.Length);

            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF report for email: {Email}", riskAnalysis.EmailAddress);
            throw;
        }
    }

    private async Task EnsureBrowserAsync()
    {
        try
        {
            // For newer versions of PuppeteerSharp, the browser download is handled automatically
            // during Puppeteer.LaunchAsync if needed
            _logger.LogInformation("Ensuring Chromium browser is available for PDF generation...");
            await Task.CompletedTask; // Placeholder for future browser setup if needed
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure browser is available");
            throw;
        }
    }

    private async Task<string> GenerateHtmlContentAsync(RiskAnalysis riskAnalysis)
    {
        try
        {
            // Load the HTML template
            var templateFile = Path.Combine(_templatePath, "RiskAnalysisReport.hbs");
            var templateContent = await File.ReadAllTextAsync(templateFile);

            // Compile the template
            var template = Handlebars.Compile(templateContent);

            // Prepare data for the template
            var templateData = new
            {
                EmailAddress = riskAnalysis.EmailAddress,
                TotalBreaches = riskAnalysis.TotalBreaches,
                RiskLevel = riskAnalysis.RiskLevel.ToString(),
                RiskLevelClass = GetRiskLevelCssClass(riskAnalysis.RiskLevel),
                Summary = riskAnalysis.Summary,
                Recommendations = riskAnalysis.Recommendations,
                AnalysisDate = riskAnalysis.AnalysisDate.ToString("MMMM dd, yyyy"),
                AnalysisTime = riskAnalysis.AnalysisDate.ToString("HH:mm:ss UTC"),
                Breaches = riskAnalysis.Breaches.Select(b => new
                {
                    Title = b.Title,
                    Domain = b.Domain,
                    BreachDate = b.BreachDate.ToString("MMMM dd, yyyy"),
                    Description = b.Description,
                    PwnCount = b.PwnCount.ToString("N0"),
                    DataClasses = string.Join(", ", b.DataClasses),
                    IsVerified = b.IsVerified,
                    IsSensitive = b.IsSensitive,
                    StatusBadges = GetStatusBadges(b)
                }).OrderByDescending(b => DateTime.Parse(b.BreachDate))
            };

            // Generate HTML content
            var htmlContent = template(templateData);
            return htmlContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating HTML content from template");
            throw;
        }
    }

    private async Task<byte[]> GeneratePdfFromHtmlAsync(string htmlContent)
    {
        try
        {
            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-dev-shm-usage",
                    "--disable-gpu"
                }
            });

            using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);

            var pdfOptions = new PdfOptions
            {
                Format = PuppeteerSharp.Media.PaperFormat.A4,
                PrintBackground = true,
                MarginOptions = new PuppeteerSharp.Media.MarginOptions
                {
                    Top = "1in",
                    Right = "0.5in",
                    Bottom = "1in",
                    Left = "0.5in"
                },
                HeaderTemplate = "<div style='font-size: 9px; margin: 0 auto; color: #666;'>Breach Management System - Risk Analysis Report</div>",
                FooterTemplate = "<div style='font-size: 9px; margin: 0 auto; color: #666;'>Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>",
                DisplayHeaderFooter = true
            };

            var pdfBytes = await page.PdfDataAsync(pdfOptions);
            return pdfBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF from HTML");
            throw;
        }
    }

    private static string GetRiskLevelCssClass(RiskLevel riskLevel)
    {
        return riskLevel switch
        {
            RiskLevel.Low => "risk-low",
            RiskLevel.Medium => "risk-medium",
            RiskLevel.High => "risk-high",
            RiskLevel.Critical => "risk-critical",
            _ => "risk-medium"
        };
    }

    private static List<string> GetStatusBadges(BreachData breach)
    {
        var badges = new List<string>();
        
        if (breach.IsVerified) badges.Add("Verified");
        if (breach.IsSensitive) badges.Add("Sensitive");
        if (breach.IsFabricated) badges.Add("Fabricated");
        if (breach.IsRetired) badges.Add("Retired");
        if (breach.IsSpamList) badges.Add("Spam List");
        if (breach.IsMalware) badges.Add("Malware");
        
        return badges;
    }
} 