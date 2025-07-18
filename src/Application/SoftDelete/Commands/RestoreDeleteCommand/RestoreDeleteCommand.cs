using System;
using Application.Interfaces;

namespace admin_service.Application.SoftDelete.Commands.RestoreDeleteCommand;

public record RestoreEntityCommand<T>(T Entity) : ICommand where T : class, ISoftDelete;

public class RestoreEntityCommandHandler<T>(IApplicationDbContext context) : ICommandHandler<RestoreEntityCommand<T>> where T : class, ISoftDelete
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(RestoreEntityCommand<T> request, CancellationToken cancellationToken)
    {
        request.Entity.IsDeleted = false;
        request.Entity.DeletedAt = null;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
