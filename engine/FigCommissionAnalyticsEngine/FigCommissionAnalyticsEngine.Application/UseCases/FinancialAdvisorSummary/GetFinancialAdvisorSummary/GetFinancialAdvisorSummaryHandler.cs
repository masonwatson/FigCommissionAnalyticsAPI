using FigCommissionAnalyticsEngine.Domain.Shared;

namespace FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;

public class GetFinancialAdvisorSummaryHandler : IGetFinancialAdvisorSummaryHandler
{
    private readonly IFinancialAdvisorSummaryReader _reader;

    public GetFinancialAdvisorSummaryHandler(IFinancialAdvisorSummaryReader reader)
    {
        _reader = reader;
    }

    public async Task<GetFinancialAdvisorSummaryResponse> HandleAsync(
        GetFinancialAdvisorSummaryRequest request, 
        CancellationToken cancellationToken)
    {
        var reportingWindow = ReportingWindow.Create(request.StartDate, request.EndDate);

        return await _reader.GetFinancialAdvisorSummaryAsync(reportingWindow, request.Sort, cancellationToken);
    }
}
