using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.HelpRequest;
using Domain.HelpRequests;

namespace Application.Interfaces;
public interface IHelpRequestMessagePublisher
{
    Task PublishHelpRequestMessage(int helpRequestId, HelpRequestMessageDto message, CancellationToken cancellationToken);
}
