namespace FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;

public interface IInsuranceCarrierBreakdownReader
{
    Task<GetInsuranceCarrierBreakdownResponse> GetInsuranceCarrierBreakdownAsync(
        GetInsuranceCarrierBreakdownRequest request, 
        CancellationToken cancellationToken);
}
