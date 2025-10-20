using System.Globalization;
using Domain.Roles;

namespace Application.Roles.GetPermissions;
public class GetPermissionQueryHandler(IApplicationDbContext context) : IQueryHandler<GetPermissionsQuery, List<GroupedPermissionDto>>
{

    public async Task<Result<List<GroupedPermissionDto>>> Handle(GetPermissionsQuery query, CancellationToken cancellationToken)
    {
        IQueryable<Permission> permissionQuery = context.Permissions.AsNoTracking();

        List<PermissionDto> permissions = await permissionQuery.Select(p => new PermissionDto
        {
            PublicId = p.PublicId,
            Name = p.Name,
            Description = p.Description,
        }).ToListAsync(cancellationToken);

        var groupedPermissions = permissions
        .GroupBy(p => p.Name.Split('_')[0]) // Group by first word before underscore
        .Select(g => new GroupedPermissionDto
        {
            GroupName = char.ToUpper(g.Key[0], CultureInfo.InvariantCulture) + g.Key.Substring(1), // Capitalize first letter with invariant culture
            Permissions = g.OrderBy(p => p.Name).ToList()
        })
        .OrderBy(g => g.GroupName)
        .ToList();

        // Ensure common groups are always present, even if empty
        var allGroups = new List<string> { "Dashboard", "School", "Admin", "Blog", "Help", "Audit", "Role", "Permission", "Email", "Auth" };
        foreach (string group in allGroups)
        {
            if (!groupedPermissions.Any(g => g.GroupName.Equals(group, StringComparison.OrdinalIgnoreCase)))
            {
                groupedPermissions.Add(new GroupedPermissionDto { GroupName = group, Permissions = new List<PermissionDto>() });
            }
        }

        return Result.Success(groupedPermissions.OrderBy(g => g.GroupName).ToList());
    }
}
