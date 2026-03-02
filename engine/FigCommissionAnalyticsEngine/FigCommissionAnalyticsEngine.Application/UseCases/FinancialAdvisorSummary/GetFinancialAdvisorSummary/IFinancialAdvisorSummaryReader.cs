namespace FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;

public interface IFinancialAdvisorSummaryReader
{
    Task<GetFinancialAdvisorSummaryResponse> GetFinancialAdvisorSummaryAsync(
        GetFinancialAdvisorSummaryRequest request, 
        CancellationToken cancellationToken);
}
