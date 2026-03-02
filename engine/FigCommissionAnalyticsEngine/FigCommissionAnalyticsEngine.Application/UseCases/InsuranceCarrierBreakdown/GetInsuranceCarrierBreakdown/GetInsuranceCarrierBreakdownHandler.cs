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
        return await _reader.GetInsuranceCarrierBreakdownAsync(request, cancellationToken);
    }
}
