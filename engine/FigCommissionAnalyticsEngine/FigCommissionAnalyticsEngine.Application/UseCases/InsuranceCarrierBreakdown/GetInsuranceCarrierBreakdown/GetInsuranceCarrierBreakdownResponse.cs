namespace FigCommissionAnalyticsEngine.Application.UseCases.InsuranceCarrierBreakdown.GetInsuranceCarrierBreakdown;

public class GetInsuranceCarrierBreakdownResponse
{
    public required long? AgentId { get; set; }
    public required string AgentName { get; set; }
    public required DateOnly MinStartDate { get; set; }
    public required DateOnly MaxEndDate { get; set; }
    public required List<AgentCarrierBreakdown> AgentCarrierBreakdowns { get; set; }
}

public class AgentCarrierBreakdown
{
    private double _agentCarrierRelativeWeight;

    public required long CarrierId { get; set; }
    public required string CarrierName { get; set; }
    public required long AgentCarrierTotalCommission { get; set; }
    public required double AgentCarrierRelativeWeight 
    {
        get => _agentCarrierRelativeWeight;
        set => _agentCarrierRelativeWeight = Math.Round(value, 3);
    }
}
