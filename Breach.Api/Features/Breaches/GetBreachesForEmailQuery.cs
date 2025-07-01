using Breach.Api.Models;
using Breach.Api.Services;
using MediatR;

namespace Breach.Api.Features.Breaches;

public class GetBreachesForEmailQuery : IRequest<List<BreachData>>
{
    public string Email { get; set; } = string.Empty;
}

public class GetBreachesForEmailQueryHandler : IRequestHandler<GetBreachesForEmailQuery, List<BreachData>>
{
    private readonly IHaveIBeenPwnedService _haveIBeenPwnedService;
    private readonly ILogger<GetBreachesForEmailQueryHandler> _logger;

    public GetBreachesForEmailQueryHandler(IHaveIBeenPwnedService haveIBeenPwnedService, ILogger<GetBreachesForEmailQueryHandler> logger)
    {
        _haveIBeenPwnedService = haveIBeenPwnedService;
        _logger = logger;
    }

    public async Task<List<BreachData>> Handle(GetBreachesForEmailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetBreachesForEmailQuery for email: {Email}", request.Email);

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            _logger.LogWarning("Empty email provided for breach search");
            return new List<BreachData>();
        }

        var breaches = await _haveIBeenPwnedService.GetBreachesForEmailAsync(request.Email);
        
        _logger.LogInformation("Found {Count} breaches for email: {Email}", breaches.Count, request.Email);
        return breaches;
    }
}