using System;
using Application.Interfaces;
using Domain.HelpRequests;

namespace Application.HelpRequest.CreateHelpReqestMessage;

public class AddHelpRequestMessageCommandHandler(IApplicationDbContext _context) : ICommandHandler<CreateHelpRequestMessageCommand, HelpRequestMessageDto>
{
    public async Task<Result<HelpRequestMessageDto>> Handle(CreateHelpRequestMessageCommand command, CancellationToken cancellationToken)
    {
        HelpRequests? helpRequest = await _context.HelpRequests.FindAsync(new object[] { command.HelpRequestId }, cancellationToken);
        if (helpRequest == null)
        {
            return Result.Failure<HelpRequestMessageDto>(HelpRequestErrors.NotFound(command.HelpRequestId));
        }

        var message = new HelpRequestMessages
        {
            Title = command.Title,
            Attachments = new List<string>(command.Attachments),
            HelpRequestId = command.HelpRequestId,
            HelpRequest = helpRequest
        };

        _context.HelpRequestMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(new HelpRequestMessageDto
        {
            Id = message.Id,
            PublicId = message.PublicId,
            Title = message.Title,
            Attachments = message.Attachments,
        });
    }
}
