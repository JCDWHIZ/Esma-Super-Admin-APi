using System;
using Domain.Roles;

namespace Application.Roles.DeleteRole;

public sealed class DeleteRoleCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteRoleCommand, string>
{

    async Task<Result<string>> ICommandHandler<DeleteRoleCommand, string>.Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        Role? entity = await context.Roles.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<string>(RoleErrors.NotFound(command.PublicId));
        }

        context.Roles.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success("Done");
    }
}
