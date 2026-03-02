using Asp.Versioning;
using FigCommissionAnalyticsEngine.Application.UseCases.Agents.GetAllAgents;
using Microsoft.AspNetCore.Mvc;

namespace FigCommissionAnalyticsEngine.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/agent")]
public class AgentController : ControllerBase
{
    private readonly IGetAllAgentsHandler _getAllAgentsHandler;

    public AgentController(
        IGetAllAgentsHandler handler)
    {
        _getAllAgentsHandler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetFinancialAdvisorSummary(
        [FromQuery] GetAllAgentsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _getAllAgentsHandler.HandleAsync(request, cancellationToken);
        return Ok(response);
    }
}
