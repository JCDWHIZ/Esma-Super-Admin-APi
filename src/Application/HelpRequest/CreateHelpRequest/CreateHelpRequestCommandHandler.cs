using Domain.HelpRequests;

namespace Application.HelpRequest.CreateHelpRequest;

public class CreateHelpRequestCommandHandler(IApplicationDbContext _dbContext) : ICommandHandler<CreateHelpRequestCommand, HelpRequestDto>
{
    public async Task<Result<HelpRequestDto>> Handle(CreateHelpRequestCommand command, CancellationToken cancellationToken)
    {
        var entity = new HelpRequests
        {
            TicketId = command.TicketId,
            Category = command.Category,
            Status = command.Status,
        };
        _dbContext.HelpRequests.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(new HelpRequestDto
        {
            Id = entity.Id,
            TicketId = entity.TicketId,
            Status = entity.Status,
            PublicId = entity.PublicId,
            Messages = new List<HelpRequestMessageDto>()
        });
    }
}
