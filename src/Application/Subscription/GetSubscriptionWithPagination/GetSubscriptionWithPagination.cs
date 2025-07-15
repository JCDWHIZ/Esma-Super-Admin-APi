using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.Common.Mappings;
using Application.Abstractions.Models;
using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.Subscription.Queries.GetSubscriptionStats;


public record GetSubscriptionWithPaginationQuery : ICommand<PaginatedList<SubscriptionDto>>
{
    public SubscriptionType? SubscriptionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Amount { get; set; }
    public string? SchoolName { get; set; } = string.Empty;
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}

public class GetSubscriptionWithPagination(IApplicationDbContext context, IMapper mapper) : ICommandHandler<GetSubscriptionWithPaginationQuery,
    PaginatedList<SubscriptionDto>>
{

    private readonly IApplicationDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaginatedList<SubscriptionDto>> Handle(GetSubscriptionWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Subscriptions.AsQueryable();

        if (!string.IsNullOrEmpty(request.SchoolName))
        {
            query = query.Where(s => _context.Schools.Any(x => x.SchoolName.Contains(request.SchoolName)));
        }

        if (request.SubscriptionType.HasValue)
        {
            query = query.Where(s => s.subscriptionType == request.SubscriptionType);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(s => s.StartDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(s => s.EndDate <= request.EndDate.Value);
        }

        if (request.Amount > 0)
        {
            query = query.Where(s => s.Amount == request.Amount);
        }

        return await query
            .ProjectTo<SubscriptionDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10);
    }
}
