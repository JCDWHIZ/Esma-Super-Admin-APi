namespace Application.HelpRequest.CreateHelpRequestMessage;

public sealed record CreateHelpRequestMessageCommand : ICommand<HelpRequestMessageDto>
{
    public Guid HelpRequestId { get; init; }
    public string Title { get; init; } = string.Empty;
    public ICollection<string> Attachments { get; init; } = new List<string>();
}
