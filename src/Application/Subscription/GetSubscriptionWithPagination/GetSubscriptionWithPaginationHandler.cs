using System;
using Application.Interfaces;
using Application.Abstractions.Models;
using Application.School.CreateSchool;

namespace Application.Subscription.GetSubscriptionWithPagination;

public sealed class GetSubscriptionWithPagination(IApplicationDbContext _context) : IQueryHandler<GetSubscriptionWithPaginationQuery,
    PaginatedList<SubscriptionDto>>
{
    public async Task<Result<PaginatedList<SubscriptionDto>>> Handle(GetSubscriptionWithPaginationQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Domain.Subscriptions.Subscriptions> subscriptionQuery = _context.Subscriptions.AsQueryable();
        if (!string.IsNullOrEmpty(query.SchoolName))
        {
            subscriptionQuery = subscriptionQuery.Where(s => _context.Schools.Any(x => x.SchoolName.Contains(query.SchoolName)));
        }

        if (query.SubscriptionType.HasValue)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.SubscriptionType == query.SubscriptionType);
        }

        if (query.StartDate.HasValue)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.StartDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.EndDate <= query.EndDate.Value);
        }

        if (query.Amount > 0)
        {
            subscriptionQuery = subscriptionQuery.Where(s => s.Amount == query.Amount);
        }

        PaginatedList<SubscriptionDto> paginatedList = await PaginatedList<SubscriptionDto>.CreateAsync(subscriptionQuery.Select(s => new SubscriptionDto
        {
            SubscriptionType = s.SubscriptionType,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Amount = s.Amount,
            // SchoolName = _context.Schools.FirstOrDefault(x => x.Id == s.SchoolId)?.SchoolName ?? string.Empty
        }),
            query.PageNumber ?? 1,
            query.PageSize ?? 10
        );
        return Result.Success(paginatedList);
    }
}
