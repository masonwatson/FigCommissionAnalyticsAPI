namespace FigCommissionAnalyticsEngine.Infrastructure.Data.Entities;

public class AgentCarrier
{
    public int AgentCarrierId { get; set; }
    public int AgentId { get; set; }
    public int CarrierId { get; set; }
    public string WritingNumber { get; set; } = string.Empty;

    public Agent Agent { get; set; } = null!;
    public Carrier Carrier { get; set; } = null!;
    public ICollection<AgentCarrierCommissionStatement> CommissionStatements { get; set; } = new List<AgentCarrierCommissionStatement>();
}
