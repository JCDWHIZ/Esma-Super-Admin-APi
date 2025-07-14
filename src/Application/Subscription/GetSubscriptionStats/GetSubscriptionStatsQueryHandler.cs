using System;
using admin_service.Application.Common.Interfaces;

namespace admin_service.Application.Subscription.Queries.GetSubscriptionWithPagination;

public record GetSubscriptionStatsQuery : IRequest<SubscriptionStatsDto>;
public class GetSubscriptionStatsQueryHandler(IApplicationDbContext context, IMapper mapper) : IRequestHandler<GetSubscriptionStatsQuery, SubscriptionStatsDto>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<SubscriptionStatsDto> Handle(GetSubscriptionStatsQuery request, CancellationToken cancellationToken)
    {
        var currentDate = DateTime.UtcNow;
    var expiryThreshold = currentDate.AddDays(15);

    var subscriptionStats = await _context.Subscriptions
        .GroupBy(_ => true)
        .Select(g => new SubscriptionStatsDto
        {
            TotalSubscriptions = g.Count(),
            ActiveSubscriptions = g.Count(x => x.EndDate >= currentDate),
            ExpiringIn15Days = g.Count(x => x.EndDate > currentDate && x.EndDate <= expiryThreshold),
            ExpiredSubscriptions = g.Count(x => x.EndDate < currentDate)
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
