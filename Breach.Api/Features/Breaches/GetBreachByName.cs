
using MediatR;
using Flurl.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.Extensions.Configuration;

namespace Breach.Api.Features.Breaches
{
    public static class GetBreachByName
    {
        public record Query(string Name) : IRequest<Breach?>;

        public class Handler : IRequestHandler<Query, Breach?>
        {
            public Handler()
            {
            }

            public async Task<Breach?> Handle(Query request, CancellationToken cancellationToken)
            {
                var baseUrl = "https://haveibeenpwned.com/api/v3/breach/";

                try
                {
                    var breach = await new Flurl.Url(baseUrl)
                        .AppendPathSegment(request.Name)
                        .GetJsonAsync<Breach>(cancellationToken: cancellationToken);

                    return breach;
                }
                catch (FlurlHttpException ex) when (ex.StatusCode == 404)
                {
                    return null;
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
