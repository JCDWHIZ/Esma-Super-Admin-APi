using System;
using Application.Interfaces;

namespace admin_service.Application.School.Commands.DeleteSchool;

public record DeleteSchoolCommand(string PublicId) : ICommand;

public class DeleteSchoolCommandHandler : ICommandHandler<DeleteSchoolCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteSchoolCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools.FirstOrDefaultAsync(x => x.PublicId == request.PublicId);

        Guard.Against.NotFound(request.PublicId, entity);
        if (entity.Subscriptions != null)
        {
            _context.Subscriptions.Remove(entity.Subscriptions);
        }
        _context.Schools.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}