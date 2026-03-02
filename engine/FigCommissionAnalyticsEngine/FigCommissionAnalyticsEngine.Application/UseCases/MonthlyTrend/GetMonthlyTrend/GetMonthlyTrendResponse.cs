namespace FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;

public class GetMonthlyTrendResponse
{
    public required long AgentId { get; set; }
    public required DateOnly MinStartDate { get; set; }
    public required DateOnly MaxEndDate { get; set; }
    public required List<MonthlyCommission> MonthlyCommissions { get; set; }
}

public class MonthlyCommission
{
    public required string YearMonth { get; set; }
    public required long TotalCommission { get; set; }
}
