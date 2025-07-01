
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using HandlebarsDotNet;
using PuppeteerSharp;
using Microsoft.Extensions.Logging;

namespace Breach.Api.Features.Breaches
{
    public static class GenerateReport
    {
        public record Query(GetBreachByName.Breach Breach, string RiskAnalysis) : IRequest<byte[]>;

        public class Handler : IRequestHandler<Query, byte[]>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger)
            {
                _logger = logger;
            }

            public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    _logger.LogInformation("Generating report for breach: {BreachName}", request.Breach.Name);

                    var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BreachReport.hbs");
                    if (!File.Exists(templatePath))
                    {
                        _logger.LogError("Breach report template not found at {TemplatePath}", templatePath);
                        throw new FileNotFoundException($"Breach report template not found at {templatePath}");
                    }

                    var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);

                    var template = Handlebars.Compile(templateContent);
                    var html = template(new { request.Breach, request.RiskAnalysis });

                    _logger.LogInformation("Downloading Chromium browser for Puppeteer...");
                    await new BrowserFetcher().DownloadAsync();
                    _logger.LogInformation("Launching Puppeteer browser...");
                    var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                    var page = await browser.NewPageAsync();
                    await page.SetContentAsync(html);
                    var pdf = await page.PdfDataAsync();

                    await browser.CloseAsync();
                    _logger.LogInformation("Report generated successfully for breach: {BreachName}", request.Breach.Name);

                    return pdf;
                }
                catch (FileNotFoundException ex)
                {
                    _logger.LogError(ex, "File not found error during report generation.");
                    throw; // Re-throw the exception after logging
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex, "Puppeteer error occurred during report generation.");
                    throw; // Re-throw the exception after logging
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred during report generation.");
                    throw; // Re-throw the exception after logging
                }
            }
        }
    }
}
