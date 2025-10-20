using Application.School.CreateSchool;

namespace Application.Subscription.GetSubscriptionWithPagination;

public sealed record GetSubscriptionWithPaginationQuery : IQuery<PaginatedList<SubscriptionDto>>
{
    public SubscriptionType? SubscriptionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Amount { get; set; }
    public string? SchoolName { get; set; } = string.Empty;
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}
