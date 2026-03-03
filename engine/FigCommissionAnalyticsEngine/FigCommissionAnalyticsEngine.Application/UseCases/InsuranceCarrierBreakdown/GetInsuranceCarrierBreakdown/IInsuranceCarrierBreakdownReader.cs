using FigCommissionAnalyticsEngine.Domain.Shared;

namespace FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;

public interface IInsuranceCarrierBreakdownReader
{
    Task<GetInsuranceCarrierBreakdownResponse> GetInsuranceCarrierBreakdownAsync(
        long? agentId,
        ReportingWindow? reportingWindow,
        CancellationToken cancellationToken);
}
