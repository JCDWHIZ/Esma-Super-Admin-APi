using Domain.HelpRequests;

namespace Application.HelpRequest.UpdateHelpRequestStatus;

public sealed class UpdateHelpRequestStatus(IApplicationDbContext context) : ICommandHandler<UpdateHelpRequestStatusCommand, HelpRequestDto>
{
    public async Task<Result<HelpRequestDto>> Handle(UpdateHelpRequestStatusCommand command, CancellationToken cancellationToken)
    {
        HelpRequests? entity = await context.HelpRequests.FirstOrDefaultAsync(hr => hr.PublicId == command.PublicId, cancellationToken);
        if (entity == null)
        {
            return Result.Failure<HelpRequestDto>(HelpRequestErrors.NotFound(command.PublicId));
        }

        entity.Status = command.Status;
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(new HelpRequestDto
        {
            Id = entity.Id,
            TicketId = entity.TicketId,
            Status = entity.Status,
            PublicId = entity.PublicId,
        });
    }
}
