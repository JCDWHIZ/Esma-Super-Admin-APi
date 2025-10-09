using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Roles.GetPermissions.GetPermissionQueryHandler;

namespace Application.Roles.GetPermissions;
public record GetPermissionsQuery : IQuery<List<GroupedPermissionDto>>;
