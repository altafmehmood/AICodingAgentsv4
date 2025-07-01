
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using HandlebarsDotNet;
using PuppeteerSharp;

namespace Breach.Api.Features.Breaches
{
    public static class GenerateReport
    {
        public record Query(GetBreachByName.Breach Breach, string RiskAnalysis) : IRequest<byte[]>;

        public class Handler : IRequestHandler<Query, byte[]>
        {
            public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "BreachReport.hbs");
                var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);

                var template = Handlebars.Compile(templateContent);
                var html = template(new { request.Breach, request.RiskAnalysis });

                await new BrowserFetcher().DownloadAsync();
                var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
                var page = await browser.NewPageAsync();
                await page.SetContentAsync(html);
                var pdf = await page.PdfDataAsync();

                await browser.CloseAsync();

                return pdf;
            }
        }
    }
}
