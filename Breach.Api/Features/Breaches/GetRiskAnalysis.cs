
using MediatR;
using Flurl.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Breach.Api.Features.Breaches
{
    public static class GetRiskAnalysis
    {
        public record Query(string Description) : IRequest<string>;

        public class Handler : IRequestHandler<Query, string>
        {
            private readonly IConfiguration _configuration;
            private readonly ILogger<Handler> _logger;

            public Handler(IConfiguration configuration, ILogger<Handler> logger)
            {
                _configuration = configuration;
                _logger = logger;
            }

            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var apiKey = _configuration["ClaudeApiKey"];
                var baseUrl = "https://api.anthropic.com/v1/messages";

                try
                {
                    var response = await baseUrl
                        .WithHeader("x-api-key", apiKey)
                        .WithHeader("anthropic-version", "2023-06-01")
                        .PostJsonAsync(new
                        {
                            model = "claude-3-5-sonnet-20240620",
                            max_tokens = 1000,
                            messages = new[]
                            {
                                new { role = "user", content = $"Analyze the following breach description and provide a risk analysis: {request.Description}" }
                            }
                        }, cancellationToken: cancellationToken);

                    var result = await response.GetJsonAsync<ClaudeResponse>();

                    return result.Content[0].Text;
                }
                catch (FlurlHttpException ex)
                {
                    _logger.LogError(ex, "Flurl HTTP error occurred while calling Claude API: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
                    throw; // Re-throw the exception after logging
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while calling Claude API.");
                    throw; // Re-throw the exception after logging
                }
            }
        }

        public record ClaudeResponse
        {
            public required Content[] Content { get; init; }
        }

        public record Content
        {
            public required string Text { get; init; }
        }
    }
}
