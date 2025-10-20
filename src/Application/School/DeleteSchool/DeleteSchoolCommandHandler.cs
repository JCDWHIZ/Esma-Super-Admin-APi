using Domain.Schools;

namespace Application.School.DeleteSchool;

public sealed class DeleteSchoolCommandHandler(IApplicationDbContext _context) : ICommandHandler<DeleteSchoolCommand, string>
{
    async Task<Result<string>> ICommandHandler<DeleteSchoolCommand, string>.Handle(DeleteSchoolCommand command, CancellationToken cancellationToken)
    {
        Schools? entity = await _context.Schools.FirstOrDefaultAsync(x => x.PublicId == command.PublicId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(command.PublicId));
        }

        if (entity.Subscriptions != null)
        {
            _context.Subscriptions.Remove(entity.Subscriptions);
        }
        _context.Schools.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success("School deleted successfully");
    }
}