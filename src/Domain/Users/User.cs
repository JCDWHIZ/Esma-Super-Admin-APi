using SharedKernel;
using SharedKernel.Enums;
using SharedKernel.Models;

namespace Domain.Users;

public sealed class User : BaseAuditableEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }
    public Roles Role { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
}
