using System;

namespace Application.Roles.CreateRole;

public sealed record CreateRoleCommand : ICommand<RoleDto>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
