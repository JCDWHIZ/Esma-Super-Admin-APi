namespace Application.Admin.EditAdmin;
public sealed record EditAdminCommand : ICommand<UserDto>
{
    public Guid PublicId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string? ProfilePic { get; set; }
    public string? PhoneNumber { get; set; }
    public string LastName { get; set; }
    public string RoleName { get; set; }
}
