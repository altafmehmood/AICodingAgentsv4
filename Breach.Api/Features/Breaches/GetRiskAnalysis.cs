
using MediatR;
using Flurl.Http;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Breach.Api.Features.Breaches
{
    public static class GetRiskAnalysis
    {
        public record Query(string Description) : IRequest<string>;

        public class Handler : IRequestHandler<Query, string>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<string> Handle(Query request, CancellationToken cancellationToken)
            {
                var apiKey = _configuration["ClaudeApiKey"];
                var baseUrl = "https://api.anthropic.com/v1/messages";

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
