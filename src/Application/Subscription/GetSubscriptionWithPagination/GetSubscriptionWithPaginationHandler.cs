using Application.School.CreateSchool;

namespace Application.Subscription.GetSubscriptionWithPagination;

public sealed class GetSubscriptionWithPagination(IApplicationDbContext _context) : IQueryHandler<GetSubscriptionWithPaginationQuery,
    PaginatedList<SubscriptionDto>>
{
    public async Task<Result<PaginatedList<SubscriptionDto>>> Handle(GetSubscriptionWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Schools.Schools> subscriptionQuery = _context.Schools.AsQueryable();
        if (!string.IsNullOrEmpty(query.SchoolName))
        {
            subscriptionQuery = subscriptionQuery.Where(x => x.SchoolName.Contains(query.SchoolName));
        }

        if (query.SubscriptionType.HasValue)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.Subscriptions.SubscriptionType == query.SubscriptionType);
        }

        if (query.StartDate.HasValue)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.Subscriptions.StartDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.Subscriptions.EndDate <= query.EndDate.Value);
        }

        if (query.Amount > 0)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.Subscriptions.Amount == query.Amount);
        }

        PaginatedList<SubscriptionDto> paginatedList = await PaginatedList<SubscriptionDto>.CreateAsync(subscriptionQuery.Select(s => new SubscriptionDto
        {
            SubscriptionType = s.Subscriptions.SubscriptionType,
            StartDate = s.Subscriptions.StartDate,
            EndDate = s.Subscriptions.EndDate,
            Amount = s.Subscriptions.Amount,
            SchoolName = s.SchoolName,
            SchoolLogo = s.LogoUrl,
            Status = Domain.Subscriptions.Subscriptions.GetStatus(s.Subscriptions.StartDate, s.Subscriptions.EndDate),
            SchoolAdminName = s.User.FirstName + " " + s.User.LastName,
            SchoolModules = s.Modules
        }),
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );
        return Result.Success(paginatedList);
    }
}
