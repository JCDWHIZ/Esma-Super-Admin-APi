using System;

namespace Application.Admin.InviteAdmin;

public sealed record InviteUserCommand : ICommand<UserDto>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Roles Role { get; set; }
}