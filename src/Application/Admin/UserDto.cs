namespace Application.Admin;

public class UserDto
{
    public Roles Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePic { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid PublicId { get; set; }
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    // public int Signups { get; set; }
}
