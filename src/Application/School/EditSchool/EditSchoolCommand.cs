namespace Application.School.EditSchool;


public sealed record EditSchoolCommand : ICommand<string>
{
    public Guid PublicId { get; init; }
    public string SchoolName { get; init; } = string.Empty;

    public string LogoUrl { get; init; } = string.Empty;

    public AddressDto Address { get; init; } = new();

    public string EmailAddress { get; init; } = string.Empty;

    public string PhoneNumber { get; init; } = string.Empty;

    public List<string> DocumentUrl { get; init; } = new();

    public List<string> Modules { get; init; } = new();

    public SubscriptionDto Subscriptions { get; init; } = new();
}

public record AddressDto
{
    public string State { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string Lga { get; init; } = string.Empty;
    public string StreetAddress { get; init; } = string.Empty;
}

public record SubscriptionDto
{
    public SubscriptionType SubscriptionType { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal Amount { get; init; }
}



