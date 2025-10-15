using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Roles.GetRoleById;
public record GetRoleByIdQuery(Guid RolePublicId) : IQuery<RolesDto>;
