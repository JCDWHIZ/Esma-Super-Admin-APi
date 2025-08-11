using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.Users;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;

namespace Application.Roles.CreateRole;

internal sealed class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, RoleDto>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateRoleCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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

        return RoleMapper.MapToRoleDto(role);
    }
}
