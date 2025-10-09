using Domain.Users;
using SharedKernel.Models;

namespace Domain.Roles;

public class Role : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string KeycloakId { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    private readonly List<Permission> _permissions = new();
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

    private readonly List<User> _users = new();
    public IReadOnlyList<User> Users => _users.AsReadOnly();

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

    public static Role Create(string name, string description, bool isDefault = false) =>
        new() { Name = name, Description = description, IsDefault = isDefault };
}
