

using Domain.Schools;
using Domain.Subscriptions;

namespace Application.School;

public record SchoolItemDto
{
    public string PublicId { get; set; } = string.Empty;
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
    public string OrganizationId { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;
    public bool Subscribed
    { get; set; }
    public SchoolStatus Status { get; set; } = SchoolStatus.PENDING;
    public string PhoneNumber { get; set; } = string.Empty;
    public ICollection<string> DocumentUrl { get; set; } = new List<string>();
    public ICollection<Modules> Modules { get; set; } = new List<Modules>();
    public required Subscriptions Subscriptions { get; set; }
    public required User User { get; set; }
    public bool IsDeleted { get; internal set; }
}
