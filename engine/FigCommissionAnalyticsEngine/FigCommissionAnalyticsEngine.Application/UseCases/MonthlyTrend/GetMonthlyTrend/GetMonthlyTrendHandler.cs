namespace FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;

public class GetMonthlyTrendHandler : IGetMonthlyTrendHandler
{
    private readonly IMonthlyTrendReader _reader;

    public GetMonthlyTrendHandler(IMonthlyTrendReader reader)
    {
        _reader = reader;
    }

    public async Task<GetMonthlyTrendResponse> HandleAsync(
        GetMonthlyTrendRequest request, 
        CancellationToken cancellationToken)
    {
        return await _reader.GetMonthlyTrendAsync(request, cancellationToken);
    }
}
