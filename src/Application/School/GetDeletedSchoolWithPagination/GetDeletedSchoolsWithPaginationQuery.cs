using System;
using System;
using Application.Interfaces;

namespace Application.School.GetDeletedSchoolWithPagination;

public sealed record GetDeletedSchoolsWithPaginationQuery : IQuery<PaginatedList<SchoolItemDto>>
{
    public string? SchoolName { get; set; }
    public string? LogoUrl { get; set; }
    public string? AddressCountry { get; set; }
    public string? AddressState { get; set; }
    public string? AddressLga { get; set; }
    public string? AddressStreetAddress { get; set; }
    public string? EmailAddress { get; set; }
    public bool? Subscribed { get; set; }
    public SchoolStatus? Status { get; set; }
    public string? PhoneNumber { get; set; }
    // public ICollection<string> DocumentUrl { get; set; } 
    // public ICollection<Modules> Modules { get; set; } 
    public SubscriptionType? SubscriptionType { get; set; }
    public int? PageNumber { get; set; } = 1;
    public int? PageSize { get; set; } = 10;
}
