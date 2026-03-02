using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigCommissionAnalyticsEngine.Application.UseCases.Agents.GetAllAgents
{
    public class GetAllAgentsResponse
    {
        public List<Agent> Agents { get; set; } = new List<Agent>();
    }

    public class Agent
    {
        public required int AgentId { get; set; }
        public required string AgentName { get; set; } = string.Empty;
    }
}
