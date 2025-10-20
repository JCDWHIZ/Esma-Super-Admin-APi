using Application.BackgroundJobs;
using Domain.Schools;
using Hangfire;

namespace Application.School.ApproveSchool;

public sealed record ApproveSchoolCommand(List<Guid> PublicIds) : ICommand<string>;

public class ApproveSchoolsHandler(IApplicationDbContext context)
    : ICommandHandler<ApproveSchoolCommand, string>
{
    public async Task<Result<string>> Handle(ApproveSchoolCommand command, CancellationToken cancellationToken)
    {

        List<Schools> schools = await context.Schools
            .Where(x => command.PublicIds.Contains(x.PublicId))
            .ToListAsync(cancellationToken);

        var missingIds = command.PublicIds
            .Except(schools.Select(s => s.PublicId))
            .ToList();

        if (missingIds.Any())
        {
            return Result.Failure<string>(SchoolErrors.NotFoundList(missingIds));
        }

        foreach (Schools school in schools)
        {
            school.Status = SchoolStatus.PROCESSING;
        }

        await context.SaveChangesAsync(cancellationToken);

        foreach (Schools school in schools)
        {
            BackgroundJob.Enqueue<IKeycloakOrganizationService>(
                service => service.CreateOrganizationForSchoolAsync(school.Id, cancellationToken)
            );
        }

        return Result.Success($"{schools.Count} school(s) approved successfully.");
    }
}

// Task<int> ICommandHandler<ApproveSchoolCommand, int>.Handle(ApproveSchoolCommand request, CancellationToken cancellationToken)
// {
//      var entity = _context.Schools.Find(command.Id);
//      Guard.Against.NotFound(command.Id, entity);
//      entity.Status = SchoolStatus.APPROVED;
//      await _context.SaveChangesAsync(cancellationToken);
//      return entity.Id;
// }
