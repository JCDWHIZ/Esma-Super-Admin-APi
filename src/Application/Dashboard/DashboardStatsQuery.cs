using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dashboard;
public sealed record DashboardStatsQuery : IQuery<DashboardStatsDto>;

public class DashboardStatsDto
{
    public SchoolStatsDto Schools { get; set; } = new();
    public SubscriptionStatsDto Subscriptions { get; set; } = new();
    public TicketStatsDto Tickets { get; set; } = new();
}

public class SchoolStatsDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Inactive { get; set; }
    public int Pending { get; set; }
}

public class SubscriptionStatsDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Expired { get; set; }
    public int ExpiringIn15Days { get; set; }
}

public class TicketStatsDto
{
    public int Open { get; set; }
    public int InProgress { get; set; }
    public int Resolved { get; set; }
    public double AverageResolutionMinutes { get; set; }
}
