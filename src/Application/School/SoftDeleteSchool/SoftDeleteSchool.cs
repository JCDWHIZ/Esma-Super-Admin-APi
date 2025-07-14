using System;
using admin_service.Application.Common.Interfaces;

namespace admin_service.Application.School.Commands.SoftDeleteSchool;


public record SoftDeleteSchoolCommand(string PublicId) : IRequest; // Ensure SchoolItemDto has an Id property
public class SoftDeleteSchoolCommandHandler(IApplicationDbContext context) : IRequestHandler<SoftDeleteSchoolCommand>
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(SoftDeleteSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools.FirstOrDefaultAsync(x => x.PublicId == request.PublicId); 

        Guard.Against.NotFound(request.PublicId, entity); // Ensure SchoolItemDto has an Id property
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
