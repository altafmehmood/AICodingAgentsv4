using Breach.Api.Models;

namespace Breach.Api.Services;

public interface IClaudeService
{
    Task<RiskAnalysis> AnalyzeRiskAsync(string email, List<BreachData> breaches);
}