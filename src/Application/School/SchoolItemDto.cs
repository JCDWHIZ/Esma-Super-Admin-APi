

using Application.School.CreateSchool;

namespace Application.School;

public record SchoolItemDto
{
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public AddressDto? Address { get; set; } = new AddressDto();
    public string OrganizationId { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;
    public bool Subscribed
    { get; set; }
    public SchoolStatus Status { get; set; } = SchoolStatus.PENDING;
    public string PhoneNumber { get; set; } = string.Empty;
    public ICollection<string>? DocumentUrl { get; set; } = new List<string>();
    public ICollection<SchoolModuleResponseDto> Modules { get; set; } = new List<SchoolModuleResponseDto>();
    public ICollection<SchoolModuleAvailabilityDto> ModuleAvailability { get; set; } = new List<SchoolModuleAvailabilityDto>();
    public ICollection<SisModuleResponseDto> SisModules { get; set; } = new List<SisModuleResponseDto>();
    public ICollection<SisModuleAvailabilityDto> SisModuleAvailability { get; set; } = new List<SisModuleAvailabilityDto>();
    public ICollection<LmsModuleResponseDto> LmsModules { get; set; } = new List<LmsModuleResponseDto>();
    public ICollection<LmsModuleAvailabilityDto> LmsModuleAvailability { get; set; } = new List<LmsModuleAvailabilityDto>();
    public required SubscriptionDto? Subscriptions { get; set; }
    public required UserDto? User { get; set; }
    public bool IsDeleted { get; internal set; }
}
