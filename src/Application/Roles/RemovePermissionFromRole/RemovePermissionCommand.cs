using System;

namespace Application.Roles.RemovePermissionFromRole;

public sealed record RemovePermissionCommand(Guid RoleId, Guid PermissionId) : ICommand<string>;