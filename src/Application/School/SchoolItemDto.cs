

using admin_service.Domain.Entities;
using admin_service.Domain.Enums;

namespace admin_service.Application.School.Queries;

public class SchoolItemDto
{
    public string PublicId { get; set; } = string.Empty;
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
    public string OrganizationId { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;
    public bool Subscribed
    { get; set; } = false;
    public SchoolStatus Status { get; set; } = SchoolStatus.PENDING;
    public string PhoneNumber { get; set; } = string.Empty;
    public ICollection<string> DocumentUrl { get; set; } = new List<string>();
    public ICollection<Modules> Modules { get; set; } = new List<Modules>();
    public required Subscriptions Subscriptions { get; set; }
    public required User User { get; set; }
    public bool IsDeleted { get; internal set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Schools, SchoolItemDto>();
        }
    }
}
