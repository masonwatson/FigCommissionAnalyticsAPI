namespace FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;

public interface IMonthlyTrendReader
{
    Task<GetMonthlyTrendResponse> GetMonthlyTrendAsync(
        GetMonthlyTrendRequest request, 
        CancellationToken cancellationToken);
}
