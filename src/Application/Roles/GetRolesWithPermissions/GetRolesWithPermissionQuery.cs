using System;

namespace Application.Roles.GetRolesWithPermission;

public sealed record GetRolesWithPermissionQuery(int? Page, int? PageSize) : IQuery<PaginatedList<RoleDto>>;
