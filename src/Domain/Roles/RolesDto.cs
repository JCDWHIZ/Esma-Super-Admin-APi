using System;
using Domain.Roles;

namespace Domain.Users;

public class RolesDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }

    public List<GroupedPermissionDto> Permissions { get; set; } = new();
}

public class RoleDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }

    public List<PermissionDto> Permissions { get; set; } = new();
}

public class GroupedPermissionDto
{
    public string GroupName { get; set; } = string.Empty;
    public List<PermissionDto> Permissions { get; set; } = new();
}

public class PermissionDto
{
    public Guid PublicId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public static class RoleMapper
{
    public static RoleDto MapToRoleDto(Role role)
    {
        return new RoleDto
        {
            PublicId = role.PublicId,
            Name = role.Name,
            Description = role.Description,
            Permissions = [.. role.Permissions
                .Select(p => new PermissionDto
                {
                    PublicId = p.PublicId,
                    Name = p.Name,
                    Description = p.Description
                })]
        };
    }
}
