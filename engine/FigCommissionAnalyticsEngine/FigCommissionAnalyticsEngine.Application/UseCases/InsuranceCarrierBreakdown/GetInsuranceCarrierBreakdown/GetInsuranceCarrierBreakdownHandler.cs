using FigCommissionAnalyticsEngine.Domain.Shared;

namespace FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;

public class GetInsuranceCarrierBreakdownHandler : IGetInsuranceCarrierBreakdownHandler
{
    private readonly IInsuranceCarrierBreakdownReader _reader;

    public GetInsuranceCarrierBreakdownHandler(IInsuranceCarrierBreakdownReader reader)
    {
        _reader = reader;
    }

    public async Task<GetInsuranceCarrierBreakdownResponse> HandleAsync(
        GetInsuranceCarrierBreakdownRequest request, 
        CancellationToken cancellationToken)
    {
        var reportingWindow = ReportingWindow.Create(request.StartDate, request.EndDate);

        return await _reader.GetInsuranceCarrierBreakdownAsync(request.AgentId, reportingWindow, cancellationToken);
    }
}
