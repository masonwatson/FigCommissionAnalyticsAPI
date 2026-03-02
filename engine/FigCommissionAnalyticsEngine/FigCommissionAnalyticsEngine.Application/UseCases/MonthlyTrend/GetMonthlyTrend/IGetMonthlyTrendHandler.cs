namespace FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;

public interface IGetMonthlyTrendHandler
{
    Task<GetMonthlyTrendResponse> HandleAsync(
        GetMonthlyTrendRequest request, 
        CancellationToken cancellationToken);
}
