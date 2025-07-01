using Breach.Api.Models;
using Breach.Api.Services;
using MediatR;

namespace Breach.Api.Features.Breaches;

public class GetAllBreachesQuery : IRequest<List<BreachData>>
{
}

public class GetAllBreachesQueryHandler : IRequestHandler<GetAllBreachesQuery, List<BreachData>>
{
    private readonly IHaveIBeenPwnedService _haveIBeenPwnedService;
    private readonly ILogger<GetAllBreachesQueryHandler> _logger;

    public GetAllBreachesQueryHandler(IHaveIBeenPwnedService haveIBeenPwnedService, ILogger<GetAllBreachesQueryHandler> logger)
    {
        _haveIBeenPwnedService = haveIBeenPwnedService;
        _logger = logger;
    }

    public async Task<List<BreachData>> Handle(GetAllBreachesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllBreachesQuery");

        var breaches = await _haveIBeenPwnedService.GetAllBreachesAsync();
        
        _logger.LogInformation("Retrieved {Count} total breaches", breaches.Count);
        return breaches;
    }
}