using Microsoft.AspNetCore.Mvc;

namespace FigCommissionAnalyticsEngine.API.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(new { status = "Healthy" });
    }
}
