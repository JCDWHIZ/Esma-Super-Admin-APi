using System.Globalization;
using Domain.Roles;

namespace Application.Roles.GetRoleById;


public class GetRoleByIdQueryHandler(IApplicationDbContext _context) : IQueryHandler<GetRoleByIdQuery, RolesDto>
{
    public async Task<Result<RolesDto>> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        Role? role = await _context.Roles
            .AsNoTracking()
            .Include(r => r.Permissions)
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.PublicId == query.RolePublicId, cancellationToken);

        if (role == null)
        {
            return Result.Failure<RolesDto>(RoleErrors.NotFound(query.RolePublicId));
        }

        var roleDto = new RolesDto
        {
            PublicId = role.PublicId,
            Name = role.Name,
            Description = role.Description,
            IsDefault = role.IsDefault,
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
        };

        return Result.Success(roleDto);
    }
}
