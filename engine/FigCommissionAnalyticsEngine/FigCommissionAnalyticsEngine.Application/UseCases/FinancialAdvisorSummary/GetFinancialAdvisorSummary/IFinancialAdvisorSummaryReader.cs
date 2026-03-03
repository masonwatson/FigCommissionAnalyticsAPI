using FigCommissionAnalyticsEngine.Domain.Shared;

namespace FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;

public interface IFinancialAdvisorSummaryReader
{
    Task<GetFinancialAdvisorSummaryResponse> GetFinancialAdvisorSummaryAsync(
        ReportingWindow? reportingWindow,
        string? sort,
        CancellationToken cancellationToken);
}
