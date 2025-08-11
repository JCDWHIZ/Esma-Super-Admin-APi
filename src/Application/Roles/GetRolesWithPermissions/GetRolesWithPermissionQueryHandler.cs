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

internal sealed class GetRolesWithPermissionQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetRolesWithPermissionQuery, PaginatedList<RoleDto>>
{
    public async Task<Result<PaginatedList<RoleDto>>> Handle(GetRolesWithPermissionQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Role> rolesQuery = context.Roles
            .Include(r => r.Permissions)
            .AsNoTracking();

        // Project to RoleDto
        IQueryable<RoleDto> roleDtosQuery = rolesQuery.Select(role => new RoleDto
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
        });

        // Paginate the result
        PaginatedList<RoleDto> paginatedRoles = await PaginatedList<RoleDto>.CreateAsync(
            roleDtosQuery,
            query.Page ?? 1,
            query.PageSize ?? 10
        );

        return Result.Success(paginatedRoles);
    }
}
