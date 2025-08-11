using System;
using Application.School.CreateSchool;

namespace Application.Abstractions.Models;

public class CreateTenantMessage
{
    public int SchoolId { get; set; }
    public string OrganizationId { get; set; }
    public Guid SchoolPublicId { get; set; }
    public string SchoolName { get; set; }
    public string SchoolAdminEmail { get; set; }
    public string SchoolAdminFirstName { get; set; }
    public string SchoolAdminLastName { get; set; }
    public string SchoolAdminUsername { get; set; }
    public string? SchoolAdminPhoneNumber { get; set; }
    public string SchoolLogoUrl { get; set; }
    public string SchoolEmail { get; set; }
    public string? SchoolPhoneNumber { get; set; }
    public AddressDto? SchoolAddress { get; set; }
    public SharedKernel.Enums.Roles SchoolAdminRole { get; set; }
    public ICollection<Modules> Modules { get; set; }
    public SubscriptionDto Subscriptions { get; set; }
}
