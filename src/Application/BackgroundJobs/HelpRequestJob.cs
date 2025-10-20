using Application.HelpRequest;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.BackgroundJobs;

public interface IHelpRequestJobHandler
{
    Task SendHelpRequestMessage(int helpRequestId, HelpRequestMessageDto message, CancellationToken cancellationToken);
    Task UpdateHelpRequestStatus(int helpRequestId, CancellationToken cancellationToken);
}

public class HelpRequestJob : IHelpRequestJobHandler
{
    //private readonly IApplicationDbContext _context;
    //private readonly IProducer<string, string> _producer;
    //private readonly KafkaSettings _kafkaSettings;
    //private readonly ILogger<HelpRequestJob> _logger;
    //private static readonly JsonSerializerOptions JsonOptions = new()
    //{
    //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    //};

    //public HelpRequestJob(
    //    IApplicationDbContext context,
    //    IProducer<string, string> producer,
    //    IOptions<KafkaSettings> kafkaSettings,
    //    ILogger<HelpRequestJob> logger)
    //{
    //    _context = context ?? throw new ArgumentNullException(nameof(context));
    //    _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    //    _kafkaSettings = kafkaSettings.Value ?? throw new ArgumentNullException(nameof(kafkaSettings));
    //    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //}

    //public async Task SendHelpRequestMessage(int helpRequestId, HelpRequestMessages message, CancellationToken cancellationToken)
    //{
    //    try
    //    {
    //        // Fetch the HelpRequest to get TenantHelpRequestId and SchoolId
    //        HelpRequests? helpRequest = await _context.HelpRequests
    //            .FirstOrDefaultAsync(hr => hr.Id == helpRequestId, cancellationToken);

    //        if (helpRequest == null)
    //        {
    //            _logger.LogWarning("Help request not found for Id: {HelpRequestId}", helpRequestId);
    //            return;
    //        }

    //        // Create Kafka message
    //        var kafkaMessage = new KafkaMessage<HelpRequestMessageResponse>
    //        {
    //            Data = new HelpRequestMessageResponse
    //            {
    //                Title = message.Title,
    //                Attachments = message.Attachments.ToList(),
    //                UserName = message.UserName,
    //                UserProfilePic = message.UserProfilePic,
    //                TenantHelpRequestId = helpRequest.TenantHelpRequestId,
    //                SchoolId = helpRequest.SchoolId
    //            }
    //        };

    //        // Serialize to JSON
    //        string messageJson = JsonSerializer.Serialize(kafkaMessage, JsonOptions);
    //        _logger.LogDebug("Publishing Kafka message: {MessageJson}", messageJson);

    //        // Publish to Kafka
    //        await _producer.ProduceAsync(_kafkaSettings.HelpRequestRespondTopic, new Message<string, string>
    //        {
    //            Key = message.PublicId.ToString(),
    //            Value = messageJson
    //        }, cancellationToken);

    //        _logger.LogInformation("Published message to Kafka topic {Topic}. MessageId: {MessageId}, HelpRequestId: {HelpRequestId}",
    //            _kafkaSettings.HelpRequestRespondTopic, message.Id, helpRequestId);
    //    }
    //    catch (ProduceException<string, string> ex)
    //    {
    //        _logger.LogError(ex, "Failed to publish message to Kafka topic {Topic}. MessageId: {MessageId}, HelpRequestId: {HelpRequestId}",
    //            _kafkaSettings.HelpRequestRespondTopic, message.Id, helpRequestId);
    //    }
    //    catch (DbUpdateException ex)
    //    {
    //        _logger.LogError(ex, "Failed to fetch help request for Id: {HelpRequestId}", helpRequestId);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Unexpected error while sending help request message to Kafka. MessageId: {MessageId}, HelpRequestId: {HelpRequestId}",
    //            message.Id, helpRequestId);
    //    }
    //}

    //public Task UpdateHelpRequestStatus(int helpRequestId, CancellationToken cancellationToken)
    //{
    //    throw new NotImplementedException();
    //}

    private readonly IHelpRequestMessagePublisher _messagePublisher;
    private readonly ILogger<HelpRequestJob> _logger;
    public HelpRequestJob(
        IHelpRequestMessagePublisher messagePublisher,
        ILogger<HelpRequestJob> logger)
    {
        _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SendHelpRequestMessage(int helpRequestId, HelpRequestMessageDto message, CancellationToken cancellationToken)
    {
        try
        {
            await _messagePublisher.PublishHelpRequestMessage(helpRequestId, message, cancellationToken);
            _logger.LogInformation("Successfully triggered Kafka publishing for MessageId: {MessageId}, HelpRequestId: {HelpRequestId}",
                message.Id, helpRequestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger Kafka publishing for MessageId: {MessageId}, HelpRequestId: {HelpRequestId}",
                message.Id, helpRequestId);
        }
    }

    public Task UpdateHelpRequestStatus(int helpRequestId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
