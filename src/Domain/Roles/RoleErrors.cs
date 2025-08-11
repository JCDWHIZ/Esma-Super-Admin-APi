using System;
using SharedKernel;

namespace Domain.Roles;

public static class RoleErrors
{
    public static Error NotFound(Guid id)
    {
        return Error.NotFound(
            "Role.NotFound",
            $"The Role with the ID {id} was not found");
    }

    public static Error AlreadyExists()
    {
        return Error.AlreadyExists(
            "Role.AlreadyExists",
        "The Role with the specified name already exists");
    }

    public static Error PermissionNotFound(Guid permissionId)
    {
        return Error.NotFound(
            "Role.PermissionNotFound",
            $"The Permission with the ID {permissionId} was not found");
    }

    public static Error PermissionAlreadyExists(string permissionName)
    {
        return Error.AlreadyExists(
            "Role.PermissionAlreadyExists",
            $"The Permission with the name {permissionName} already exists");
    }
}
