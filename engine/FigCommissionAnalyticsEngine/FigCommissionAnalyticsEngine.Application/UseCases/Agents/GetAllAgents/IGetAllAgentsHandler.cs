namespace FigCommissionAnalyticsEngine.Application.UseCases.Agents.GetAllAgents;

public interface IGetAllAgentsHandler
{
    Task<GetAllAgentsResponse> HandleAsync(
        GetAllAgentsRequest request, 
        CancellationToken cancellationToken);
}
