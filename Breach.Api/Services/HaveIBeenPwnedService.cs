using Breach.Api.Models;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Breach.Api.Services;

public class HaveIBeenPwnedService : IHaveIBeenPwnedService
{
    private readonly ILogger<HaveIBeenPwnedService> _logger;
    private readonly HaveIBeenPwnedOptions _options;

    public HaveIBeenPwnedService(ILogger<HaveIBeenPwnedService> logger, IOptions<HaveIBeenPwnedOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<List<BreachData>> GetBreachesForEmailAsync(string email)
    {
        try
        {
            _logger.LogInformation("Fetching breaches for email: {Email}", email);

            var response = await $"{_options.BaseUrl}/breachedaccount/{email}"
                .WithHeader("hibp-api-key", _options.ApiKey)
                .WithHeader("User-Agent", _options.UserAgent)
                .GetJsonAsync<List<BreachData>>();

            _logger.LogInformation("Found {Count} breaches for email: {Email}", response.Count, email);
            return response;
        }
        catch (FlurlHttpException ex) when (ex.StatusCode == 404)
        {
            _logger.LogInformation("No breaches found for email: {Email}", email);
            return new List<BreachData>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching breaches for email: {Email}", email);
            throw;
        }
    }

    public async Task<List<BreachData>> GetAllBreachesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all breaches");

            var response = await $"{_options.BaseUrl}/breaches"
                .WithHeader("hibp-api-key", _options.ApiKey)
                .WithHeader("User-Agent", _options.UserAgent)
                .GetJsonAsync<List<BreachData>>();

            _logger.LogInformation("Found {Count} total breaches", response.Count);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all breaches");
            throw;
        }
    }
}

public class HaveIBeenPwnedOptions
{
    public string BaseUrl { get; set; } = "https://haveibeenpwned.com/api/v3";
    public string ApiKey { get; set; } = string.Empty;
    public string UserAgent { get; set; } = "BreachManagementSystem/1.0";
}