using System;
using Application.Interfaces;
using Domain.Schools;

namespace Application.School.SoftDeleteSchool;


public sealed class SoftDeleteSchoolCommandHandler(IApplicationDbContext _context) : ICommandHandler<SoftDeleteSchoolCommand, string>
{
    public async Task<Result<string>> Handle(SoftDeleteSchoolCommand command, CancellationToken cancellationToken)
    {
        Domain.Schools.Schools? entity = await _context.Schools.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.PublicId));
        }
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("School deleted successfully");
    }
}

