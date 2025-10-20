using Domain.Schools;

namespace Application.School.RestoreDeletedSchool;

public sealed class RestoreDeletedSchoolCommandHandler(IApplicationDbContext _context) : ICommandHandler<RestoreDeletedSchoolCommand, string>
{

    async Task<Result<string>> ICommandHandler<RestoreDeletedSchoolCommand, string>.Handle(RestoreDeletedSchoolCommand command, CancellationToken cancellationToken)
    {
        Domain.Schools.Schools? entity = await _context.Schools.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.PublicId));
        }
        entity.IsDeleted = false;
        entity.DeletedAt = null;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success("School restored successfully");
    }
}

