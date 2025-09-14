using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Roles.GetRolesWithPermission;
using Domain.Roles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;


namespace Application.Roles.GetRolesWithPermission;

public class GetRolesWithPermissionQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetRolesWithPermissionQuery, List<RoleDto>>
{
    public async Task<Result<List<RoleDto>>> Handle(GetRolesWithPermissionQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Role> rolesQuery = context.Roles
            .Include(r => r.Permissions)
            .AsNoTracking();

        // Project to RoleDto
        List<RoleDto> roleDtosQuery = await rolesQuery.Select(role => new RoleDto
        {
            PublicId = role.PublicId,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.Permissions.Select(p => new PermissionDto
            {
                PublicId = p.PublicId,
                Name = p.Name,
                Description = p.Description
            }).ToList()
        }).ToListAsync(cancellationToken);

        return Result.Success(roleDtosQuery);
    }
}
