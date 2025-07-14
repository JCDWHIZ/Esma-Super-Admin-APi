using System;
using admin_service.Application.Common.Interfaces;

namespace admin_service.Application.School.Commands.DeleteSchool;

public record DeleteSchoolCommand(string PublicId) : IRequest;

public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand>
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