using SharedKernel.Models;

namespace Domain.Roles;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string KeycloakId { get; set; } = string.Empty;

    private readonly List<Permission> _permissions = new();
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

    public void AddPermission(Permission permission)
    {
        if (!_permissions.Contains(permission))
        {
            _permissions.Add(permission);
        }
    }

    public void RemovePermission(Permission permission)
    {
        _permissions.Remove(permission);
    }

    public static Role Create(string name, string description) =>
        new() { Name = name, Description = description };
}
