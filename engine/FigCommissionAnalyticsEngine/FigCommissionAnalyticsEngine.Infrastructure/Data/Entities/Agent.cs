namespace FigCommissionAnalyticsEngine.Infrastructure.Data.Entities;

public class Agent
{
    public int AgentId { get; set; }
    public string AgentName { get; set; } = string.Empty;

    public ICollection<AgentCarrier> AgentCarriers { get; set; } = new List<AgentCarrier>();
}
