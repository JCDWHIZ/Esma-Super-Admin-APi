using System;

namespace Application.Roles.GetRolesWithPermission;

public sealed record GetRolesWithPermissionQuery : IQuery<List<RoleDto>>;
