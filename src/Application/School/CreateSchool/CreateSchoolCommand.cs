namespace Application.School.CreateSchool;

public sealed record CreateSchoolCommand : ICommand<string>
{
    public string SchoolName { get; init; } = string.Empty;

    public string LogoUrl { get; init; } = string.Empty;

    public AddressDto Address { get; init; } = new();

    public string EmailAddress { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public List<string> DocumentUrl { get; init; } = new();

    public List<Modules> Modules { get; init; } = new();

    public SubscriptionDto Subscriptions { get; init; } = new();
    public UserDto SchoolAdmin { get; init; } = new();

}

public record AddressDto
{
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Lga { get; init; } = string.Empty;
    public string StreetAddress { get; init; } = string.Empty;
}

public record UserDto
{
    public Roles Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePic { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? PublicId { get; set; }
    public int? Id { get; set; }
    public string Email { get; set; } = string.Empty;
}

public record SubscriptionDto
{
    public SubscriptionType SubscriptionType { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public decimal Amount { get; init; }
    public string? SchoolLogo { get; init; }
    public string? SchoolName { get; init; }
    public string? SchoolAdminName { get; init; }
    public ICollection<Modules>? SchoolModules { get; init; }
    public SubscriptionStatus? Status { get; set; }
}

