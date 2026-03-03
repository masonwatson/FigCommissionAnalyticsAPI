using FigCommissionAnalyticsEngine.Domain.Shared;

namespace FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;

public interface IMonthlyTrendReader
{
    Task<GetMonthlyTrendResponse> GetMonthlyTrendAsync(
        long agentId,
        ReportingWindow? reportingWindow,
        CancellationToken cancellationToken);
}
