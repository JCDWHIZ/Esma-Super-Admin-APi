using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BackgroundJobs;
using Application.Interfaces.Services;
using Domain.Roles;
using Domain.Users;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.CreateRole;

public class CreateRoleCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<CreateRoleCommand, RoleDto>
{
    public async Task<Result<RoleDto>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        bool roleExists = await _dbContext.Roles
            .AnyAsync(r => r.Name == command.Name, cancellationToken);

        if (roleExists)
        {
            return Result.Failure<RoleDto>(RoleErrors.AlreadyExists());
        }

        var role = Role.Create(command.Name, command.Description);

        _dbContext.Roles.Add(role);
        await _dbContext.SaveChangesAsync(cancellationToken);
        role.Raise(new SyncRolesDomainEvent());
        return RoleMapper.MapToRoleDto(role);
    }
}
