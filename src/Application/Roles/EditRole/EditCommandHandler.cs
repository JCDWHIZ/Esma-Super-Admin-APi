using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.EditRole;
internal class EditCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<EditRoleCommand, string>
{
    public async Task<Result<string>> Handle(EditRoleCommand command, CancellationToken cancellationToken)
    {
        Role? role = await _dbContext.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.PublicId == command.RolePublicId, cancellationToken);

        if (role == null)
        {
            return Result.Failure<string>(RoleErrors.NotFound(command.RolePublicId));
        }

        bool roleNameExists = await _dbContext.Roles
            .AnyAsync(r => r.Name == command.Name && r.PublicId != command.RolePublicId, cancellationToken);

        if (roleNameExists)
        {
            return Result.Failure<string>(RoleErrors.AlreadyExists());
        }

        List<Permission> permissions = await _dbContext.Permissions
            .Where(x => command.PermissionIds.Contains(x.PublicId))
            .ToListAsync(cancellationToken);

        var missingIds = command.PermissionIds
            .Except(permissions.Select(s => s.PublicId))
            .ToList();

        if (missingIds.Any())
        {
            return Result.Failure<string>(RoleErrors.NotFoundList(missingIds));
        }

        role.Name = command.Name;
        role.Description = command.Description;
        role.IsDefault = command.IsDefault;

        role.Permissions.Aggregate(new List<Permission>(), (toRemove, permission) =>
        {
            if (!permissions.Any(p => p.Id == permission.Id))
            {
                toRemove.Add(permission);
            }
            return toRemove;
        }).ForEach(p => role.RemovePermission(p));
        foreach (Permission permission in permissions)
        {
            role.AddPermission(permission);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        role.Raise(new SyncRolesDomainEvent());

        return Result.Success("Role updated successfully");
    }
}
