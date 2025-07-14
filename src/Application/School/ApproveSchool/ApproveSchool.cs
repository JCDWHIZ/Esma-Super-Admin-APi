using System;
using admin_service.Application.Common.Interfaces;
using admin_service.Domain.Enums;
using Hangfire;

namespace admin_service.Application.School.Commands.ApproveSchool;


public class ApproveSchoolCommandValidator : AbstractValidator<ApproveSchoolCommand>
{
    public ApproveSchoolCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}


public record ApproveSchoolCommand(int Id) : IRequest;

public class ApproveSchoolHandler(IApplicationDbContext context) : IRequestHandler<ApproveSchoolCommand>
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(ApproveSchoolCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Schools.FindAsync(new object[] { request.Id }, cancellationToken);
        Guard.Against.NotFound(request.Id, entity); // Ensure SchoolItemDto has an Id property


        entity.Status = SchoolStatus.APPROVED;
        // notification here
        BackgroundJob.Enqueue<IKeycloakOrganizationService>(
       service => service.CreateOrganizationForSchoolAsync(entity.Id, cancellationToken));
        await _context.SaveChangesAsync(cancellationToken);
    }
}


// Task<int> IRequestHandler<ApproveSchoolCommand, int>.Handle(ApproveSchoolCommand request, CancellationToken cancellationToken)
// {
//      var entity = _context.Schools.Find(request.Id);
//      Guard.Against.NotFound(request.Id, entity);
//      entity.Status = SchoolStatus.APPROVED;
//      await _context.SaveChangesAsync(cancellationToken);
//      return entity.Id;
// }