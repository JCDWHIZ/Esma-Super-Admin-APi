namespace Application.Roles.EditRole;
public class EditRoleCommand : ICommand<string>
{
    public Guid RolePublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<Guid> PermissionIds { get; set; } = new List<Guid>();
}
