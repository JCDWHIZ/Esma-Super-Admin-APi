using System;

namespace Application.Admin.InviteAdmin;


public sealed class InviteAdminCommand : ICommand<UserDto>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string? ProfilePic { get; set; }
    public string? PhoneNumber { get; set; }
    public string LastName { get; set; }
    public string RoleName { get; set; }
}
