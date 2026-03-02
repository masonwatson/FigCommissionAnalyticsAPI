namespace FigCommissionAnalyticsEngine.Application.UseCases.Agents.GetAllAgents;

public class GetAllAgentsHandler : IGetAllAgentsHandler
{
    private readonly IGetAllAgentsReader _reader;

    public GetAllAgentsHandler(IGetAllAgentsReader reader)
    {
        _reader = reader;
    }

    public async Task<GetAllAgentsResponse> HandleAsync(
        GetAllAgentsRequest request, 
        CancellationToken cancellationToken)
    {
        return await _reader.GetInsuranceCarrierBreakdownAsync(request, cancellationToken);
    }
}
