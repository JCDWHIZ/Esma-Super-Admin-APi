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
            Total = await context.Subscriptions
        .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted, cancellationToken),

            Active = await context.Subscriptions
        .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted && s.EndDate > now, cancellationToken),

            Expired = await context.Subscriptions
        .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted && s.EndDate <= now, cancellationToken),

            ExpiringIn15Days = await context.Subscriptions
        .CountAsync(s => s.Schools != null
                      && !s.Schools.IsDeleted
                      && s.EndDate > now
                      && s.EndDate <= now.AddDays(15), cancellationToken)
        };

        List<double> resolvedTickets = await context.HelpRequests
            .Where(t => t.Status == HelpStatus.RESOLVED)
            .Select(t => (t.LastModified - t.Created).TotalMinutes)
            .ToListAsync(cancellationToken);

        // With this block:
        double averageResolutionMinutes = resolvedTickets.Any()
            ? Math.Round(resolvedTickets.Average(), 0)
            : 0;

        var ticketStats = new TicketStatsDto
        {
            Open = await context.HelpRequests.CountAsync(t => t.Status == HelpStatus.OPEN_REQUEST, cancellationToken),
            InProgress = await context.HelpRequests.CountAsync(t => t.Status == HelpStatus.IN_PROGRESS, cancellationToken),
            Resolved = resolvedTickets.Count,
            AverageResolutionMinutes = averageResolutionMinutes
        };

        var subscriptionUsage = new SubscriptionUsageDto
        {
            Basic = await context.Subscriptions
                .Where(s => s.SubscriptionType == SubscriptionType.BASIC)
                .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted, cancellationToken),
            Premium = await context.Subscriptions
                .Where(s => s.SubscriptionType == SubscriptionType.PREMIUM)
                .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted, cancellationToken),
            Standard = await context.Subscriptions
                .Where(s => s.SubscriptionType == SubscriptionType.STANDARD)
                .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted, cancellationToken),
            Custom = await context.Subscriptions
                .Where(s => s.SubscriptionType == SubscriptionType.CUSTOMIZED)
                .CountAsync(s => s.Schools != null && !s.Schools.IsDeleted, cancellationToken),
        };

        var result = new DashboardStatsDto
        {
            Schools = schoolStats,
            Subscriptions = subscriptionStats,
            Tickets = ticketStats,
            SubscriptionUsage = subscriptionUsage
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

//double averageResolutionMinutes = resolvedTickets.Any()
//    ? resolvedTickets.Average().ToString("F2", System.Globalization.CultureInfo.InvariantCulture) // Format to 2 decimal places
//    : 0
//    
// Replace this block:
//double averageResolutionMinutes = resolvedTickets.Any()
//    ? resolvedTickets.Average().ToString("F2", System.Globalization.CultureInfo.InvariantCulture) // Format to 2 decimal places
//    : 0;
