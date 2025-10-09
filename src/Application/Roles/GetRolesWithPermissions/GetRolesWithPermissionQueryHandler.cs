using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application;
using Application.Roles.GetRolesWithPermission;
using Domain.Roles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using static Application.Roles.GetPermissions.GetPermissionQueryHandler;


namespace Application.Roles.GetRolesWithPermission;

public class GetRolesWithPermissionQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetRolesWithPermissionQuery, List<RolesDto>>
{
    public async Task<Result<List<RolesDto>>> Handle(GetRolesWithPermissionQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Role> rolesQuery = context.Roles
         .Include(r => r.Permissions)
         .Include(r => r.Users) // Include users to count them
         .AsNoTracking();

        // Execute the query and bring data to memory
        List<Role> roles = await rolesQuery.ToListAsync(cancellationToken);

        // Process the data client-side
        var roleDtosQuery = roles.Select(role => new RolesDto
        {
            PublicId = role.PublicId,
            Name = role.Name,
            Description = role.Description,
            Permissions = role.Permissions
                .GroupBy(p => p.Name.Split('_', StringSplitOptions.None)[0])
                .Select(g => new GroupedPermissionDto
                {
                    GroupName = char.ToUpper(g.Key[0], CultureInfo.InvariantCulture) + g.Key.Substring(1),
                    Permissions = g.Select(p => new PermissionDto
                    {
                        PublicId = p.PublicId,
                        Name = p.Name,
                        Description = p.Description
                    }).OrderBy(p => p.Name).ToList()
                }).OrderBy(g => g.GroupName).ToList(),
            UserCount = role.Users.Count,
            PermissionCount = role.Permissions.Count
        }).ToList();

        return Result.Success(roleDtosQuery);
    }
}
