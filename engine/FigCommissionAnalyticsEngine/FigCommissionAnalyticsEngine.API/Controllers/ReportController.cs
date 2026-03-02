using Asp.Versioning;
using FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;
using FigCommissionAnalyticsEngine.Application.UseCases.MonthlyTrend.GetMonthlyTrend;
using FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;
using Microsoft.AspNetCore.Mvc;

namespace FigCommissionAnalyticsEngine.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/report")]
public class ReportController : ControllerBase
{
    private readonly IGetFinancialAdvisorSummaryHandler _financialAdviorSummaryHandler;
    private readonly IGetMonthlyTrendHandler _monthlyTrendHandler;
    private readonly IGetInsuranceCarrierBreakdownHandler _insuranceCarrierBreakdownHandler;

    public ReportController(
        IGetFinancialAdvisorSummaryHandler handler, 
        IGetMonthlyTrendHandler monthlyTrendHandler, 
        IGetInsuranceCarrierBreakdownHandler insuranceCarrierBreakdownHandler)
    {
        _financialAdviorSummaryHandler = handler;
        _monthlyTrendHandler = monthlyTrendHandler;
        _insuranceCarrierBreakdownHandler = insuranceCarrierBreakdownHandler;
    }

    [HttpGet("financialAdvisorSummary")]
    public async Task<IActionResult> GetFinancialAdvisorSummary(
        [FromQuery] GetFinancialAdvisorSummaryRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _financialAdviorSummaryHandler.HandleAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("monthlyTrend/{agentId}")]
    public async Task<IActionResult> GetMonthlyTrend(
        [FromRoute] int agentId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        CancellationToken cancellationToken)
    {
        var request = new GetMonthlyTrendRequest
        {
            AgentId = agentId,
            StartDate = startDate,
            EndDate = endDate
        };
        var response = await _monthlyTrendHandler.HandleAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("insuranceCarrierBreakdown")]
    public async Task<IActionResult> GetInsuranceCarrierBreakdown(
        [FromQuery] GetInsuranceCarrierBreakdownRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _insuranceCarrierBreakdownHandler.HandleAsync(request, cancellationToken);
        return Ok(response);
    }
}
