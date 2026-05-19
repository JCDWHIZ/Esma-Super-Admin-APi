using System.Text.Json.Serialization;
using Domain.Users;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace Domain.Schools;

public sealed class Schools : BaseAuditableEntity
{
    public string SchoolName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();

    public string EmailAddress { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string OrganizationId { get; set; } = string.Empty;
    public bool Subscribed { get; set; } = true;
    public SchoolStatus Status { get; set; } = SchoolStatus.PENDING;
    public string PhoneNumber { get; set; } = string.Empty;
    public ICollection<string> DocumentUrl { get; set; } = new List<string>();
    public ICollection<SchoolModule> Modules { get; set; } = new List<SchoolModule>();
    [JsonIgnore]
    public required Subscriptions.Subscriptions Subscriptions { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public required SchoolAdmins User { get; set; }
}

public class Address
{
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? LGA { get; set; }
    public string? StreetAddress { get; set; }
}
