using System;
using Domain.Roles;

namespace Application.Roles.RemovePermissionFromRole;

public sealed class RemovePermissionCommandHandler(IApplicationDbContext context) : ICommandHandler<RemovePermissionCommand, string>
{
    public async Task<Result<string>> Handle(RemovePermissionCommand command, CancellationToken cancellationToken)
    {
        Role? entity = await context.Roles
            .Include(x => x.Permissions)
            .FirstOrDefaultAsync(x => x.PublicId == command.RoleId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<string>(RoleErrors.NotFound(command.RoleId));
        }

        Permission? permission = await context.Permissions
            .FirstOrDefaultAsync(x => x.PublicId == command.PermissionId, cancellationToken);

        if (permission == null)
        {
            return Result.Failure<string>(RoleErrors.PermissionNotFound(command.PermissionId));
        }

        entity.RemovePermission(permission);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success("Permission removed successfully");
    }
}
