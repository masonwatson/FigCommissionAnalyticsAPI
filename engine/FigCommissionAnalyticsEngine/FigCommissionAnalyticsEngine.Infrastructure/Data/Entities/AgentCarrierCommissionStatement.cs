namespace FigCommissionAnalyticsEngine.Infrastructure.Data.Entities;

public class AgentCarrierCommissionStatement
{
    public int AgentCarrierCommissionStatementId { get; set; }
    public int AgentCarrierId { get; set; }
    public int? CommissionAmount { get; set; }
    public string StatementDate { get; set; } = string.Empty;

    public AgentCarrier AgentCarrier { get; set; } = null!;
}
