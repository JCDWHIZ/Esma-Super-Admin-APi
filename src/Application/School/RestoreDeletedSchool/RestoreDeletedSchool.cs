using System;
using Application.Interfaces;

namespace admin_service.Application.School.Commands.RestoreDeletedSchool;

public record RestoreDeletedSchoolCommand(string PublicId) : ICommand; // Ensure SchoolItemDto has an Id property
public class RestoreDeletedSchoolCommandHandler(IApplicationDbContext context) : ICommandHandler<RestoreDeletedSchoolCommand>
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(RestoreDeletedSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools.FirstOrDefaultAsync(x => x.PublicId == request.PublicId);

        Guard.Against.NotFound(request.PublicId, entity); // Ensure SchoolItemDto has an Id property
        entity.IsDeleted = false;
        entity.DeletedAt = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

