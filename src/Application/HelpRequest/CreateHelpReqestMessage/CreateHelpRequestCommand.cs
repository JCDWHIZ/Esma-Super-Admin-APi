using System;

namespace Application.HelpRequest.CreateHelpReqestMessage;

public sealed record CreateHelpRequestMessageCommand : ICommand<HelpRequestMessageDto>
{
    public int HelpRequestId { get; init; }
    public string Title { get; init; } = string.Empty;
    public ICollection<string> Attachments { get; init; } = new List<string>();
}
