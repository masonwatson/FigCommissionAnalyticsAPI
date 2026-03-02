namespace FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;

public interface IGetInsuranceCarrierBreakdownHandler
{
    Task<GetInsuranceCarrierBreakdownResponse> HandleAsync(
        GetInsuranceCarrierBreakdownRequest request, 
        CancellationToken cancellationToken);
}
