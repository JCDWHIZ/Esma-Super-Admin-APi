namespace Application.Admin;

public class UserDto
{
    public Guid PublicId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePic { get; set; }
    public string? RoleName { get; set; } // Use Role.Name instead of enum
    public DateTimeOffset? CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
}
