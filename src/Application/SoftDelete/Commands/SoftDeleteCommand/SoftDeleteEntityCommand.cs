using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Common;

namespace admin_service.Application.SoftDelete.Commands;

public record SoftDeleteEntityCommand<T>(T Entity) : ICommand<Unit> where T : class, ISoftDelete;


public class SoftDeleteEntityCommandHandler<T> : ICommandHandler<SoftDeleteEntityCommand<T>, Unit>
    where T : class, ISoftDelete
{
    private readonly IApplicationDbContext _context;

    public SoftDeleteEntityCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(SoftDeleteEntityCommand<T> request, CancellationToken cancellationToken)
    {
        var entity = request.Entity;
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
