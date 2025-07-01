using Breach.Api.Models;
using Flurl.Http;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Breach.Api.Services;

public class ClaudeService : IClaudeService
{
    private readonly ILogger<ClaudeService> _logger;
    private readonly ClaudeOptions _options;

    public ClaudeService(ILogger<ClaudeService> logger, IOptions<ClaudeOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<RiskAnalysis> AnalyzeRiskAsync(string email, List<BreachData> breaches)
    {
        try
        {
            _logger.LogInformation("Analyzing risk for email: {Email} with {Count} breaches", email, breaches.Count);

            var prompt = BuildRiskAnalysisPrompt(email, breaches);
            var requestBody = new
            {
                model = _options.Model,
                max_tokens = _options.MaxTokens,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                }
            };

            var response = await _options.BaseUrl
                .WithHeader("x-api-key", _options.ApiKey)
                .WithHeader("anthropic-version", "2023-06-01")
                .WithHeader("Content-Type", "application/json")
                .PostJsonAsync(requestBody);

            var responseContent = await response.GetStringAsync();
            var claudeResponse = JsonSerializer.Deserialize<ClaudeResponse>(responseContent);

            var analysis = ParseRiskAnalysis(email, breaches, claudeResponse?.Content?.FirstOrDefault()?.Text ?? "");
            
            _logger.LogInformation("Risk analysis completed for email: {Email}, Risk Level: {RiskLevel}", email, analysis.RiskLevel);
            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing risk for email: {Email}", email);
            throw;
        }
    }

    private string BuildRiskAnalysisPrompt(string email, List<BreachData> breaches)
    {
        var breachSummary = breaches.Any() 
            ? string.Join("\n", breaches.Select(b => $"- {b.Title} ({b.BreachDate:yyyy-MM-dd}): {b.Description}"))
            : "No breaches found.";

        return $@"
Analyze the security risk for email address: {email}

Data breaches found ({breaches.Count}):
{breachSummary}

Please provide a risk analysis in the following JSON format:
{{
    ""riskLevel"": ""Low|Medium|High|Critical"",
    ""summary"": ""Brief summary of the risk assessment"",
    ""recommendations"": [""recommendation1"", ""recommendation2"", ""recommendation3""]
}}

Consider factors like:
- Number of breaches
- Sensitivity of breached data
- Recency of breaches
- Verification status of breaches
- Types of data compromised (passwords, financial, personal info, etc.)
";
    }

    private RiskAnalysis ParseRiskAnalysis(string email, List<BreachData> breaches, string claudeResponse)
    {
        try
        {
            // Try to extract JSON from Claude's response
            var jsonStart = claudeResponse.IndexOf('{');
            var jsonEnd = claudeResponse.LastIndexOf('}') + 1;
            
            if (jsonStart >= 0 && jsonEnd > jsonStart)
            {
                var jsonString = claudeResponse.Substring(jsonStart, jsonEnd - jsonStart);
                var parsedResponse = JsonSerializer.Deserialize<ClaudeRiskResponse>(jsonString);
                
                return new RiskAnalysis
                {
                    EmailAddress = email,
                    TotalBreaches = breaches.Count,
                    RiskLevel = Enum.Parse<RiskLevel>(parsedResponse?.RiskLevel ?? "Medium"),
                    Summary = parsedResponse?.Summary ?? "Risk analysis completed",
                    Recommendations = parsedResponse?.Recommendations ?? Array.Empty<string>(),
                    AnalysisDate = DateTime.UtcNow,
                    Breaches = breaches
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse Claude response, using fallback analysis");
        }

        // Fallback analysis if parsing fails
        return CreateFallbackAnalysis(email, breaches);
    }

    private RiskAnalysis CreateFallbackAnalysis(string email, List<BreachData> breaches)
    {
        var riskLevel = breaches.Count switch
        {
            0 => RiskLevel.Low,
            1 or 2 => RiskLevel.Medium,
            3 or 4 or 5 => RiskLevel.High,
            _ => RiskLevel.Critical
        };

        var recommendations = new List<string>
        {
            "Change passwords for all affected accounts",
            "Enable two-factor authentication where possible",
            "Monitor accounts for suspicious activity"
        };

        if (breaches.Any(b => b.DataClasses.Any(dc => dc.ToLower().Contains("password"))))
        {
            recommendations.Add("Use a password manager to generate unique passwords");
        }

        return new RiskAnalysis
        {
            EmailAddress = email,
            TotalBreaches = breaches.Count,
            RiskLevel = riskLevel,
            Summary = $"Found {breaches.Count} data breaches associated with this email address",
            Recommendations = recommendations.ToArray(),
            AnalysisDate = DateTime.UtcNow,
            Breaches = breaches
        };
    }
}

public class ClaudeOptions
{
    public string BaseUrl { get; set; } = "https://api.anthropic.com/v1/messages";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "claude-3-5-sonnet-20240620";
    public int MaxTokens { get; set; } = 1000;
}

public class ClaudeResponse
{
    public ClaudeContent[]? Content { get; set; }
}

public class ClaudeContent
{
    public string? Text { get; set; }
}

public class ClaudeRiskResponse
{
    public string? RiskLevel { get; set; }
    public string? Summary { get; set; }
    public string[]? Recommendations { get; set; }
}