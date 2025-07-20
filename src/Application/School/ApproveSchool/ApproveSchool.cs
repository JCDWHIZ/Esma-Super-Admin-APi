using System;
using Application.Interfaces;
using Hangfire;
using Application.BackgroundJobs;
using Domain.Schools;

namespace Application.School.ApproveSchool;

public sealed record ApproveSchoolCommand(Guid PublicId) : ICommand<string>;

public class ApproveSchoolHandler(IApplicationDbContext context) : ICommandHandler<ApproveSchoolCommand, string>
{
    public async Task<Result<string>> Handle(ApproveSchoolCommand request, CancellationToken cancellationToken)
    {
        Schools? school = await context.Schools.FirstOrDefaultAsync(x => x.PublicId == request.PublicId, cancellationToken);
        if (school == null)
        {
            return Result.Failure<string>(SchoolErrors.NotFound(request.PublicId));
        }

        school.Status = SchoolStatus.APPROVED;
        await context.SaveChangesAsync(cancellationToken);

        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
        service => service.CreateOrganizationForSchoolAsync(school.Id, cancellationToken));

        return Result.Success("School approved successfully.");
    }
}


// Task<int> ICommandHandler<ApproveSchoolCommand, int>.Handle(ApproveSchoolCommand request, CancellationToken cancellationToken)
// {
//      var entity = _context.Schools.Find(request.Id);
//      Guard.Against.NotFound(request.Id, entity);
//      entity.Status = SchoolStatus.APPROVED;
//      await _context.SaveChangesAsync(cancellationToken);
//      return entity.Id;
// }