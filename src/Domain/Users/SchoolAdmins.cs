using SharedKernel.Models;

namespace Domain.Users;

public sealed class SchoolAdmins : BaseEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public SharedKernel.Enums.Roles Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
}
