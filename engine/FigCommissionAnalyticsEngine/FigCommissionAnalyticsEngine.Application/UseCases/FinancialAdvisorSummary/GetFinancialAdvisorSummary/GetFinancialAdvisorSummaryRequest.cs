namespace FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;

public class GetFinancialAdvisorSummaryRequest
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Sort { get; set; }
}
