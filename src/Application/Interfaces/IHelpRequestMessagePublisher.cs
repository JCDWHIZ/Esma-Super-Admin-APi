using Application.HelpRequest;

namespace Application.Interfaces;
public interface IHelpRequestMessagePublisher
{
    Task PublishHelpRequestMessage(int helpRequestId, HelpRequestMessageDto message, CancellationToken cancellationToken);
}
