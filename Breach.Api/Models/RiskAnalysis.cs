namespace Breach.Api.Models;

public class RiskAnalysis
{
    public string EmailAddress { get; set; } = string.Empty;
    public int TotalBreaches { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string[] Recommendations { get; set; } = Array.Empty<string>();
    public DateTime AnalysisDate { get; set; }
    public List<BreachData> Breaches { get; set; } = new();
}

public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}