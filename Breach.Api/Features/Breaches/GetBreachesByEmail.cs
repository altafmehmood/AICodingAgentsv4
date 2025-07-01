
using MediatR;
using Flurl.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;

namespace Breach.Api.Features.Breaches
{
    public static class GetBreachesByEmail
    {
        public record Query(string Email) : IRequest<IEnumerable<Breach>>;

        public class Handler : IRequestHandler<Query, IEnumerable<Breach>>
        {
            private readonly ILogger<Handler> _logger;

            public Handler(ILogger<Handler> logger)
            {
                _logger = logger;
            }

            public async Task<IEnumerable<Breach>> Handle(Query request, CancellationToken cancellationToken)
            {
                var baseUrl = $"https://haveibeenpwned.com/api/v3/breachedaccount/{request.Email}";

                try
                {
                    var breaches = await baseUrl.GetJsonAsync<IEnumerable<Breach>>(cancellationToken: cancellationToken);
                    return breaches;
                }
                catch (FlurlHttpException ex) when (ex.StatusCode == 404)
                {
                    _logger.LogInformation("No breaches found for the given email (404 Not Found).");
                    return new List<Breach>(); // Return empty list if no breaches found
                }
                catch (FlurlHttpException ex)
                {
                    _logger.LogError(ex, "Flurl HTTP error occurred while fetching breaches by email: {StatusCode} - {Message}", ex.StatusCode, ex.Message);
                    throw; // Re-throw the exception after logging
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while fetching breaches by email.");
                    throw; // Re-throw the exception after logging
                }
            }
        }

        public record Breach
        {
            public required string Name { get; init; }
            public required string Title { get; init; }
            public required string Domain { get; init; }
            public DateTime BreachDate { get; init; }
            public int PwnCount { get; init; }
            public required string Description { get; init; }
            public required string[] DataClasses { get; init; }
            public bool IsVerified { get; init; }
            public bool IsFabricated { get; init; }
            public bool IsSensitive { get; init; }
            public bool IsRetired { get; init; }
            public bool IsSpamList { get; init; }
            public required string LogoPath { get; init; }
        }
    }
}
