namespace Application.Roles.CreateRole;

public sealed record CreateRoleCommand : ICommand<string>
{
    public string Name { get; init; } = string.Empty;
    public bool IsDefault { get; init; }
    public ICollection<Guid> PermissionIds { get; init; }
    public string Description { get; init; } = string.Empty;
}
