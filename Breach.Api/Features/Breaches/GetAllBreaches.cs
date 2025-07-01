
using MediatR;
using Flurl.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Breach.Api.Features.Breaches
{
    public static class GetAllBreaches
    {
        public record Query(DateTime? FromDate, DateTime? ToDate) : IRequest<IEnumerable<Breach>>;

        public class Handler : IRequestHandler<Query, IEnumerable<Breach>>
        {
            private readonly IConfiguration _configuration;

            public Handler(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public async Task<IEnumerable<Breach>> Handle(Query request, CancellationToken cancellationToken)
            {
                var apiKey = _configuration["HaveIBeenPwnedApiKey"];
                var baseUrl = "https://haveibeenpwned.com/api/v3/breaches"; // Assuming this is the endpoint for all breaches

                try
                {
                    var requestUrl = baseUrl.WithHeader("hibp-api-key", apiKey);

                    if (request.FromDate.HasValue)
                    {
                        requestUrl = requestUrl.SetQueryParam("from", request.FromDate.Value.ToString("yyyy-MM-dd"));
                    }

                    if (request.ToDate.HasValue)
                    {
                        requestUrl = requestUrl.SetQueryParam("to", request.ToDate.Value.ToString("yyyy-MM-dd"));
                    }

                    var breaches = await requestUrl.GetJsonAsync<IEnumerable<Breach>>(cancellationToken: cancellationToken);

                    return breaches;
                }
                catch (FlurlHttpException ex) when (ex.StatusCode == 404)
                {
                    return new List<Breach>(); // Return empty list if no breaches found
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
