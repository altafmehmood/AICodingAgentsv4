using Breach.Api.Models;

namespace Breach.Api.Services;

public interface IPdfService
{
    Task<byte[]> GenerateRiskAnalysisReportAsync(RiskAnalysis riskAnalysis);
} 