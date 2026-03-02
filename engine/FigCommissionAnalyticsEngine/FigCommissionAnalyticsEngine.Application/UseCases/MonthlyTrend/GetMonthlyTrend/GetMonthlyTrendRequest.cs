namespace FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;

public class GetMonthlyTrendRequest
{
    public required long AgentId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
