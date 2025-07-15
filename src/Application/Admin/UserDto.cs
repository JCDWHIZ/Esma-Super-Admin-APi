namespace Application.Admin;

public class UserDto
{
    public Roles Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    // public int Signups { get; set; }
}
