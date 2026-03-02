namespace FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;

public interface IGetFinancialAdvisorSummaryHandler
{
    Task<GetFinancialAdvisorSummaryResponse> HandleAsync(
        GetFinancialAdvisorSummaryRequest request, 
        CancellationToken cancellationToken);
}
