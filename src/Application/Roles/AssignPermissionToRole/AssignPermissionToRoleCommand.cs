using System;

namespace Application.Roles.AssignPermissionToRole;

public sealed record AssignPermissionToRoleCommand(Guid RoleId, Guid PermissionId) : ICommand<string>;