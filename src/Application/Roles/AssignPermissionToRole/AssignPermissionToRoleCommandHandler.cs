using System;
using Domain.Roles;

namespace Application.Roles.AssignPermissionToRole;

public class AssignPermissionToRoleCommandHandler(IApplicationDbContext context) : ICommandHandler<AssignPermissionToRoleCommand, string>
{
    public async Task<Result<string>> Handle(AssignPermissionToRoleCommand command, CancellationToken cancellationToken)
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

        entity.AddPermission(permission);
        await context.SaveChangesAsync(cancellationToken);
        entity.Raise(new SyncRolesDomainEvent());
        return Result.Success("Permission assigned successfully");
    }
}
