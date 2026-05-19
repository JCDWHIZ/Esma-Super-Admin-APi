using Application.Abstractions.Data;
using Application.HelpRequest;
using Application.Interfaces;
using Domain.HelpRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;
public class HelpRequestMessagePublisher : IHelpRequestMessagePublisher
{
    private readonly IApplicationDbContext _context;
    private readonly IMessageProducer _messageProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<HelpRequestMessagePublisher> _logger;

    public HelpRequestMessagePublisher(
        IApplicationDbContext context,
        IMessageProducer messageProducer,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<HelpRequestMessagePublisher> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _kafkaSettings = kafkaSettings.Value ?? throw new ArgumentNullException(nameof(kafkaSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishHelpRequestMessage(int helpRequestId, HelpRequestMessageDto message, CancellationToken cancellationToken)
    {
        try
        {
            // Fetch the HelpRequest to get TenantHelpRequestId and SchoolId
            HelpRequests? helpRequest = await _context.HelpRequests
                .FirstOrDefaultAsync(hr => hr.Id == helpRequestId, cancellationToken);

            if (helpRequest == null)
            {
                _logger.LogWarning("Help request not found for Id: {HelpRequestId}", helpRequestId);
                return;
            }

            // Create the message payload
            var messageData = new HelpRequestMessageResponse
            {
                Title = message.Title,
                Attachments = message.Attachments.ToList(),
                UserName = message.UserName,
                UserProfilePic = message.UserProfilePic,
                TenantHelpRequestId = helpRequest.TenantHelpRequestId,
                SchoolId = helpRequest.SchoolId
            };

            // Use IMessageProducer to send the message
            string offset = await _messageProducer.SendMessageAsync(
                messageType: nameof(HelpRequestMessageResponse),
                data: messageData,
                topic: _kafkaSettings.HelpRequestRespondTopic);

            _logger.LogInformation("Published message to Kafka topic {Topic}. MessageId: {MessageId}, HelpRequestId: {HelpRequestId}, Offset: {Offset}",
                _kafkaSettings.HelpRequestRespondTopic, message.Id, helpRequestId, offset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to Kafka topic {Topic}. MessageId: {MessageId}, HelpRequestId: {HelpRequestId}",
                _kafkaSettings.HelpRequestRespondTopic, message.Id, helpRequestId);
        }
    }
}
