using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Roles;

namespace Application.Roles.GetPermissions;
public class GetPermissionQueryHandler(IApplicationDbContext context) : IQueryHandler<GetPermissionsQuery, List<PermissionDto>>
{
    public async Task<Result<List<PermissionDto>>> Handle(GetPermissionsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Permission> permissionQuery = context.Permissions.AsNoTracking();

        List<PermissionDto> permissionDtoQuery = await permissionQuery.Select(role => new PermissionDto
        {
            PublicId = role.PublicId,
            Name = role.Name,
            Description = role.Description,
        }).ToListAsync(cancellationToken);

        return Result.Success(permissionDtoQuery);
    }
}
