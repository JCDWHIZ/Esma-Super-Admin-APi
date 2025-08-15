using System;
using Application.Abstractions.Authentication;
using Application.Interfaces;
using Domain.HelpRequests;

namespace Application.HelpRequest.CreateHelpReqestMessage;

public class AddHelpRequestMessageCommandHandler(IApplicationDbContext _context, IUserContext userContext) : ICommandHandler<CreateHelpRequestMessageCommand, HelpRequestMessageDto>
{
    public async Task<Result<HelpRequestMessageDto>> Handle(CreateHelpRequestMessageCommand command, CancellationToken cancellationToken)
    {
        HelpRequests? helpRequest = await _context.HelpRequests.FirstOrDefaultAsync(hr => hr.PublicId == command.HelpRequestId, cancellationToken);
        if (helpRequest == null)
        {
            return Result.Failure<HelpRequestMessageDto>(HelpRequestErrors.NotFound(command.HelpRequestId));
        }

        User? user = await _context.Users.FirstOrDefaultAsync(u => u.PublicId == userContext.UserPublicId, cancellationToken);

        if (user == null || userContext.UserPublicId == null)
        {
            return Result.Failure<HelpRequestMessageDto>(UserErrors.NotFoundInToken);
        }

        var message = new HelpRequestMessages
        {
            Title = command.Title,
            Attachments = new List<string>(command.Attachments),
            HelpRequestId = helpRequest.Id,
            UserName = $"{user.FirstName} {user.LastName}",
            UserProfilePic = user.ProfilePic ?? string.Empty,
            HelpRequest = helpRequest
        };

        _context.HelpRequestMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(new HelpRequestMessageDto
        {
            Id = message.Id,
            PublicId = message.PublicId,
            Title = message.Title,
            UserName = message.UserName,
            UserProfilePic = message.UserProfilePic,
            Attachments = message.Attachments,
        });
    }
}
