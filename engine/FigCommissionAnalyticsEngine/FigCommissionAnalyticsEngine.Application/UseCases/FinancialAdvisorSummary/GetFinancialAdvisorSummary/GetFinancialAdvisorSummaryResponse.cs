namespace FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;

public class GetFinancialAdvisorSummaryResponse
{
    public required DateOnly MinStartDate { get; set; }
    public required DateOnly MaxEndDate { get; set; }
    public required List<AgentSummary> AgentSummaries { get; set; }
}

public class AgentSummary
{
    private double _averageMonthlyStatementVolume;

    public required long AgentId { get; set; }
    public required string AgentName { get; set; }
    public required long AverageMonthlyCommission { get; set; }
    public required long TotalCommission { get; set; }
    public required double AverageMonthlyStatementVolume
    {
        get => _averageMonthlyStatementVolume;
        set => _averageMonthlyStatementVolume = Math.Round(value, 2);
    }
    public required long StatementVolume { get; set; }
    public required DateOnly BestYearMonth { get; set; }
    public required TopCarrier TopCarrier { get; set; }
}

public class TopCarrier
{
    public required long CarrierId { get; set; }
    public required string CarrierName { get; set; }
}
