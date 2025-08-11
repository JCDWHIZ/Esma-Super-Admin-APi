using System;
using Domain.Roles;

namespace Application.Roles.CreatePermission;

public sealed class CreatePermissionCommandHandler(IApplicationDbContext context) : ICommandHandler<CreatePermissionCommand, PermissionDto>
{
    public async Task<Result<PermissionDto>> Handle(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        Permission? existingPermission = await context.Permissions
            .FirstOrDefaultAsync(x => x.Name == command.Name, cancellationToken);

        if (existingPermission != null)
        {
            return Result.Failure<PermissionDto>(RoleErrors.PermissionAlreadyExists(command.Name));
        }

        var newPermission = Permission.Create(
            command.Name,
            command.Description
        );

        context.Permissions.Add(newPermission);
        await context.SaveChangesAsync(cancellationToken);

        var result = new PermissionDto
        {
            PublicId = newPermission.PublicId,
            Name = newPermission.Name,
            Description = newPermission.Description
        };

        return Result.Success(result);
    }
}
