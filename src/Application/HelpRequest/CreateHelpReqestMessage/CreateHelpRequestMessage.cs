using System;
using admin_service.Application.Common.Exceptions;
using admin_service.Application.Common.Interfaces;
using admin_service.Application.HelpRequest.Queries;
using admin_service.Domain.Entities;

namespace admin_service.Application.HelpRequest.Commands.CreateHelpReqestMessage;

public record AddHelpRequestMessageCommand : IRequest<HelpRequestMessageDto>
{
    public int HelpRequestId { get; init; }
    public string Title { get; init; } = string.Empty;
    public ICollection<string> Attachments { get; init; }= new List<string>();
}

public class AddHelpRequestMessageCommandHandler : IRequestHandler<AddHelpRequestMessageCommand, HelpRequestMessageDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddHelpRequestMessageCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<HelpRequestMessageDto> Handle(AddHelpRequestMessageCommand request, CancellationToken cancellationToken)
    {
        var helpRequest = await _context.HelpRequests.FindAsync(new object[] { request.HelpRequestId }, cancellationToken);
        if (helpRequest == null)
        {
            throw new AlreadyExistsException("Help request not found.");
        }

        var message = new HelpRequestMessages
        {
            Title = request.Title,
            Attachments = new List<string>(request.Attachments),
            HelpRequestId = request.HelpRequestId,
            HelpRequest = helpRequest
        };

        _context.HelpRequestMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<HelpRequestMessageDto>(message);
    }
}
