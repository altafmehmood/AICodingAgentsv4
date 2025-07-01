using Breach.Api.Models;

namespace Breach.Api.Services;

public interface IHaveIBeenPwnedService
{
    Task<List<BreachData>> GetBreachesForEmailAsync(string email);
    Task<List<BreachData>> GetAllBreachesAsync();
}