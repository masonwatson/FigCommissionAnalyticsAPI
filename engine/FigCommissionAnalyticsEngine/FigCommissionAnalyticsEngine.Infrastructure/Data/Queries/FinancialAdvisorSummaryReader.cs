using FigCommissionAnalyticsEngine.Application.UseCases.FinancialAdvisorSummary.GetFinancialAdvisorSummary;
using Microsoft.EntityFrameworkCore;

namespace FigCommissionAnalyticsEngine.Infrastructure.Data.Queries;

public class FinancialAdvisorSummaryReader : IFinancialAdvisorSummaryReader
{
    private readonly AppDbContext _context;

    public FinancialAdvisorSummaryReader(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GetFinancialAdvisorSummaryResponse> GetFinancialAdvisorSummaryAsync(
        GetFinancialAdvisorSummaryRequest request, 
        CancellationToken cancellationToken)
    {
        // Load all commission statements with related Agent and Carrier data
        // using eager loading to avoid N+1 query issues
        var statements = await _context.AgentCarrierCommissionStatements
            .Include(s => s.AgentCarrier)
                .ThenInclude(ac => ac.Agent)
            .Include(s => s.AgentCarrier)
                .ThenInclude(ac => ac.Carrier)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // Filter statements by date range if provided in the request
        // StatementDate is stored as TEXT in SQLite, so we parse it to DateOnly for comparison
        var filteredStatements = statements
            .Where(s => !string.IsNullOrEmpty(s.StatementDate) && 
                       DateOnly.TryParse(s.StatementDate, out var date) &&
                       (!request.StartDate.HasValue || date >= request.StartDate.Value) &&
                       (!request.EndDate.HasValue || date <= request.EndDate.Value))
            .ToList();
        
        // Calculate the maximum end date as the last day of the previous month
        // This ensures we don't include incomplete data from the current month
        var maxEndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-DateTime.Today.Day));

        // Find the earliest statement date in the database
        var tempMinStartDate1 = statements
            .Select(s => DateOnly.Parse(s.StatementDate))
            .DefaultIfEmpty(DateOnly.MinValue)
            .Min();

        // Calculate a date 12 months back from maxEndDate, normalized to the 1st of the month
        // This provides a rolling 12-month window for analysis
        var tempMinStartDate2 = maxEndDate.AddMonths(-11);
        var tempStartDateDays = tempMinStartDate2.Day;
        tempMinStartDate2 = tempMinStartDate2.AddDays(-tempStartDateDays + 1);

        // Use the later of the two dates to ensure we don't go beyond available data
        var minStartDate = tempMinStartDate1 > tempMinStartDate2 ? tempMinStartDate1 : tempMinStartDate2;

        // Group statements by agent and calculate summary metrics
        var agentSummaries = filteredStatements
            .GroupBy(s => new { s.AgentCarrier.AgentId, s.AgentCarrier.Agent.AgentName })
            .Select(g =>
            {
                var agentStatements = g.ToList();
                var totalCommission = agentStatements.Sum(s => s.CommissionAmount ?? 0);
                var statementVolume = agentStatements.Count;

                // Count unique months represented in the statements
                // This is used to calculate meaningful averages
                var monthsCount = agentStatements
                    .Select(s => s.StatementDate)
                    .Distinct()
                    .Count();

                // Calculate averages based on the number of months with data
                // This prevents skewed averages when agents have sparse data
                var averageMonthlyCommission = monthsCount > 0 ? totalCommission / monthsCount : 0;
                var averageMonthlyStatementVolume = monthsCount > 0 ? (double)statementVolume / monthsCount : 0;

                // Find the month with the highest commission total for this agent
                var bestYearMonth = agentStatements
                    .GroupBy(s => s.StatementDate)
                    .Select(mg => new
                    {
                        YearMonth = mg.Key,
                        MonthTotal = mg.Sum(s => s.CommissionAmount ?? 0)
                    })
                    .OrderByDescending(x => x.MonthTotal)
                    .FirstOrDefault();

                // Identify the carrier that generated the most commission for this agent
                var topCarrier = agentStatements
                    .GroupBy(s => new { s.AgentCarrier.CarrierId, s.AgentCarrier.Carrier.CarrierName })
                    .Select(cg => new
                    {
                        cg.Key.CarrierId,
                        cg.Key.CarrierName,
                        Total = cg.Sum(s => s.CommissionAmount ?? 0)
                    })
                    .OrderByDescending(x => x.Total)
                    .FirstOrDefault();

                return new AgentSummary
                {
                    AgentId = g.Key.AgentId,
                    AgentName = g.Key.AgentName,
                    TotalCommission = totalCommission,
                    AverageMonthlyCommission = averageMonthlyCommission,
                    StatementVolume = statementVolume,
                    AverageMonthlyStatementVolume = averageMonthlyStatementVolume,
                    BestYearMonth = bestYearMonth != null ? DateOnly.Parse(bestYearMonth.YearMonth) : DateOnly.MinValue,
                    TopCarrier = new TopCarrier
                    {
                        CarrierId = topCarrier?.CarrierId ?? 0,
                        CarrierName = topCarrier?.CarrierName ?? string.Empty
                    }
                };
            })
            .ToList();

        // Apply sorting based on the request parameter
        if (!string.IsNullOrEmpty(request.Sort))
        {
            switch (request.Sort.ToLower())
            {
                case "name":
                    agentSummaries = agentSummaries.OrderByDescending(a => a.AgentName).ToList();
                    break;
                case "totalcommission":
                    agentSummaries = agentSummaries.OrderByDescending(a => a.TotalCommission).ToList();
                    break;
                case "averagecommission":
                    agentSummaries = agentSummaries.OrderByDescending(a => a.AverageMonthlyCommission).ToList();
                    break;
                case "statementvolume":
                    agentSummaries = agentSummaries.OrderByDescending(a => a.StatementVolume).ToList();
                    break;
                default:
                    break;
            }
        }

        return new GetFinancialAdvisorSummaryResponse
        {
            MinStartDate = minStartDate,
            MaxEndDate = maxEndDate,
            AgentSummaries = agentSummaries
        };
    }
}
