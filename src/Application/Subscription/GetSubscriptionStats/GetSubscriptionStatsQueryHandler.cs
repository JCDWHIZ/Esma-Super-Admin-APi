namespace Application.Subscription.GetSubscriptionStats;

public sealed record GetSubscriptionStatsQuery : ICommand<SubscriptionStatsDto>;
public sealed class GetSubscriptionStatsQueryHandler(IApplicationDbContext _context) : ICommandHandler<GetSubscriptionStatsQuery, SubscriptionStatsDto>
{
    async Task<Result<SubscriptionStatsDto>> ICommandHandler<GetSubscriptionStatsQuery, SubscriptionStatsDto>.Handle(GetSubscriptionStatsQuery command, CancellationToken cancellationToken)
    {
        DateTime currentDate = DateTime.UtcNow;
        DateTime expiryThreshold = currentDate.AddDays(15);

        SubscriptionStatsDto? subscriptionStats = await _context.Subscriptions.Where(x => !x.IsDeleted)
            .GroupBy(_ => true)
            .Select(g => new SubscriptionStatsDto
            {
                TotalSubscriptions = g.Count(x => x.Schools != null && !x.Schools.IsDeleted),
                ActiveSubscriptions = g.Count(x => x.Schools != null && !x.Schools.IsDeleted && x.EndDate > currentDate),
                ExpiringIn15Days = g.Count(x => x.Schools != null
                      && !x.Schools.IsDeleted
                      && x.EndDate > currentDate
                      && x.EndDate <= expiryThreshold),
                ExpiredSubscriptions = g.Count(x => x.Schools != null && !x.Schools.IsDeleted && x.EndDate < currentDate)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return subscriptionStats ?? new SubscriptionStatsDto();
    }
}

public class SubscriptionStatsDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int ExpiringIn15Days { get; set; }
    public int ExpiredSubscriptions { get; set; }
}
