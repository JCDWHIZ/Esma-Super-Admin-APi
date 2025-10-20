namespace Application.Roles.CreatePermission;

public sealed class CreatePermissionCommand : ICommand<PermissionDto>
{
    public string Name { get; set; }
    public string Description { get; set; }
}