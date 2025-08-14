using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dashboard;
public class GetDashboardStatsHandler(IApplicationDbContext context)
    : IQueryHandler<DashboardStatsQuery, DashboardStatsDto>
{
    public async Task<Result<DashboardStatsDto>> Handle(DashboardStatsQuery request, CancellationToken cancellationToken)
    {
        DateTime now = DateTime.UtcNow;

        var schoolStats = new SchoolStatsDto
        {
            Total = await context.Schools.CountAsync(cancellationToken),
            Active = await context.Schools.CountAsync(s => s.Status == SchoolStatus.ACTIVE, cancellationToken),
            Inactive = await context.Schools.CountAsync(s => s.Status == SchoolStatus.INACTIVE, cancellationToken),
            Pending = await context.Schools.CountAsync(s => s.Status == SchoolStatus.PENDING, cancellationToken)
        };

        var subscriptionStats = new SubscriptionStatsDto
        {
            Total = await context.Subscriptions.CountAsync(cancellationToken),
            Active = await context.Subscriptions.CountAsync(s => s.EndDate > now, cancellationToken),
            Expired = await context.Subscriptions.CountAsync(s => s.EndDate <= now, cancellationToken),
            ExpiringIn15Days = await context.Subscriptions.CountAsync(
                s => s.EndDate > now && s.EndDate <= now.AddDays(15), cancellationToken)
        };

        List<double> resolvedTickets = await context.HelpRequests
            .Where(t => t.Status == HelpStatus.RESOLVED)
            .Select(t => (t.LastModified - t.Created).TotalMinutes)
            .ToListAsync(cancellationToken);

        double averageResolutionMinutes = resolvedTickets.Any()
            ? resolvedTickets.Average()
            : 0;

        var ticketStats = new TicketStatsDto
        {
            Open = await context.HelpRequests.CountAsync(t => t.Status == HelpStatus.OPEN_REQUEST, cancellationToken),
            InProgress = await context.HelpRequests.CountAsync(t => t.Status == HelpStatus.IN_PROGRESS, cancellationToken),
            Resolved = resolvedTickets.Count,
            AverageResolutionMinutes = averageResolutionMinutes
        };


        var result = new DashboardStatsDto
        {
            Schools = schoolStats,
            Subscriptions = subscriptionStats,
            Tickets = ticketStats
        };

        return Result.Success(result);
    }
}

// In the AverageAsync call, change EF.Functions.DateDiffMinute to EF.Functions.DateDiffMinute(t.Created, t.LastModified)
// However, the error indicates that DateDiffMinute is not available. 
// The correct method is DateDiffMinute, but it is only available in Microsoft.EntityFrameworkCore.SqlServer.
// If you are not using SQL Server, you need to calculate the difference in C# instead.

//AverageResolutionMinutes = await context.HelpRequests
//    .Where(t => t.Status == HelpStatus.RESOLVED)
//    .Select(t => (double)(EF.Functions.DateDiffMinute(t.Created, t.LastModified)))
//    .AverageAsync(cancellationToken)
//AverageResolutionMinutes = await context.HelpRequests
//    .Where(t => t.Status == HelpStatus.RESOLVED)
//    .Select(t => (t.LastModified - t.Created).TotalMinutes)
//    .AverageAsync(cancellationToken)
