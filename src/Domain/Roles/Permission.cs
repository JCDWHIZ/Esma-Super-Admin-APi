using SharedKernel.Models;

namespace Domain.Roles;

public class Permission : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string KeycloakId { get; set; } = string.Empty;

    private readonly List<Role> _roles = new();
    public IReadOnlyList<Role> Roles => _roles.AsReadOnly();

    public void AddRole(Role role)
    {
        if (!_roles.Contains(role))
        {
            _roles.Add(role);
        }
    }

    public void RemoveRole(Role role)
    {
        _roles.Remove(role);
    }
    public static Permission Create(string name, string description) =>
        new() { Name = name, Description = description };
}


