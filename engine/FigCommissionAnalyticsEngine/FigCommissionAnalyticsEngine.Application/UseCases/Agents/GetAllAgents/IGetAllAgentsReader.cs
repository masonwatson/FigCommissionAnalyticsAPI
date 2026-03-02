using FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigCommissionAnalyticsEngine.Application.UseCases.Agents.GetAllAgents
{
    public interface IGetAllAgentsReader
    {
        Task<GetAllAgentsResponse> GetInsuranceCarrierBreakdownAsync(
            GetAllAgentsRequest request,
            CancellationToken cancellationToken);
    }
}
