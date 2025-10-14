using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BackgroundJobs;
using Application.Interfaces.Services;
using Domain.Roles;
using Domain.Schools;
using Domain.Users;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.CreateRole;

public class CreateRoleCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<CreateRoleCommand, string>
{
    public async Task<Result<string>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        bool roleExists = await _dbContext.Roles
            .AnyAsync(r => r.Name == command.Name, cancellationToken);

        if (roleExists)
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

        var role = Role.Create(command.Name, command.Description, command.IsDefault);

        _dbContext.Roles.Add(role);

        foreach (Permission permission in permissions)
        {
            role.AddPermission(permission);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        role.Raise(new SyncRolesDomainEvent());
        return Result.Success("Roles created successfully");
    }
}
