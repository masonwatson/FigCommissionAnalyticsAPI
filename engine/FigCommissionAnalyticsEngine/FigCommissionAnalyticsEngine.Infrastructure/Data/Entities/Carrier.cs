namespace FigCommissionAnalyticsEngine.Infrastructure.Data.Entities;

public class Carrier
{
    public int CarrierId { get; set; }
    public string CarrierName { get; set; } = string.Empty;

    public ICollection<AgentCarrier> AgentCarriers { get; set; } = new List<AgentCarrier>();
}
