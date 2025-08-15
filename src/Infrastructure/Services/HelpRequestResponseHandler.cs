using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Abstractions.Data;
using Application.Abstractions.Models;
using Application.Interfaces;
using Confluent.Kafka;
using Domain.HelpRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Enums;

namespace Infrastructure.Services;
public class HelpRequestRespondHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<HelpRequestRespondHandler> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IKafkaAdminService _kafkaAdminService;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public HelpRequestRespondHandler(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<HelpRequestRespondHandler> logger, IKafkaAdminService kafkaAdminService)
    {
        _serviceProvider = serviceProvider;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _kafkaAdminService = kafkaAdminService;
        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = "help-request-respond-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);
        try
        {
            await _kafkaAdminService.EnsureTopicsExistAsync(
                _kafkaSettings.DefaultTopic,
                _kafkaSettings.CreateOrganizationTopic,
                _kafkaSettings.CreateTenantTopic,
                _kafkaSettings.EmailTopic,
                _kafkaSettings.TenantResponseTopic,
                _kafkaSettings.HelpRequestGetTopic,
                _kafkaSettings.HelpRequestRespondTopic
            );
            _consumer.Subscribe(_kafkaSettings.HelpRequestRespondTopic);
            _logger.LogInformation("Started consuming from topic: {Topic}", _kafkaSettings.HelpRequestRespondTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<string, string>? consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));
                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Received message from topic {Topic}", consumeResult.Topic);
                        await ProcessHelpRequestResponse(consumeResult.Message.Value, stoppingToken);
                        _consumer.Commit(consumeResult);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in help request respond handler service");
        }
        finally
        {
            try
            {
                _consumer.Close();
                _logger.LogInformation("Kafka consumer closed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing Kafka consumer");
            }
        }
    }

    private async Task ProcessHelpRequestResponse(string messageJson, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Processing help request message response: {Message}", messageJson);

            // This expects a message to be added to an existing help request
            KafkaMessage<HelpRequestMessageResponse>? kafkaMessage = JsonSerializer.Deserialize<KafkaMessage<HelpRequestMessageResponse>>(messageJson, JsonOptions);

            if (kafkaMessage?.Data == null)
            {
                _logger.LogWarning("Received null or empty help request message data");
                return;
            }

            HelpRequestMessageResponse messageData = kafkaMessage.Data;

            using IServiceScope scope = _serviceProvider.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            HelpRequests? helpRequest = await dbContext.HelpRequests
                .Include(hr => hr.Messages)
                .FirstOrDefaultAsync(hr => hr.TenantHelpRequestId == messageData.TenantHelpRequestId
                                         && hr.SchoolId == messageData.SchoolId, cancellationToken);

            if (helpRequest == null)
            {
                _logger.LogWarning("Help request not found. TenantHelpRequestId: {TenantHelpRequestId}, SchoolId: {SchoolId}",
                    messageData.TenantHelpRequestId, messageData.SchoolId);
                return;
            }

            var newMessage = new HelpRequestMessages
            {
                Title = messageData.Title,
                Attachments = messageData.Attachments ?? new List<string>(),
                HelpRequestId = helpRequest.Id,
                UserName = messageData.UserName,
                UserProfilePic = messageData.UserProfilePic
            };

            dbContext.HelpRequestMessages.Add(newMessage);
            helpRequest.LastModified = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully added message to help request. HelpRequestId: {HelpRequestId}, TenantHelpRequestId: {TenantHelpRequestId}, MessageId: {MessageId}",
                helpRequest.Id, helpRequest.TenantHelpRequestId, newMessage.Id);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize help request message: {Message}", messageJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process help request message");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
// DTOs for the messages

public class HelpRequestMessageResponse
{
    public string? Title { get; set; }
    public List<string> Attachments { get; set; } = new();
    public string UserName { get; set; }
    public string UserProfilePic { get; set; }
    public string TenantHelpRequestId { get; set; }
    public string SchoolId { get; set; }
}

//public class HelpRequestResponseMessage
//{
//    public string? TicketId { get; set; }
//    public HelpStatus Status { get; set; }
//    public HelpCategory? Category { get; set; }
//    public string UserProfilePic { get; set; }
//    public string UserName { get; set; } = string.Empty;
//    public string TenantHelpRequestId { get; set; }
//    public string SchoolId { get; set; }
//    public ICollection<HelpRequestMessages>? Messages { get; set; } = new List<HelpRequestMessages>();
//}

//try
//{
//    _logger.LogDebug("Processing help request response message: {Message}", messageJson);

//    // Deserialize the Kafka message wrapper - expecting a help request ID
//    KafkaMessage<int>? kafkaMessage = JsonSerializer.Deserialize<KafkaMessage<int>>(messageJson, JsonOptions);

//    if (kafkaMessage?.Data == null || kafkaMessage.Data <= 0)
//    {
//        _logger.LogWarning("Received invalid help request ID: {HelpRequestId}", kafkaMessage?.Data);
//        return;
//    }

//    int helpRequestId = kafkaMessage.Data;

//    using IServiceScope scope = _serviceProvider.CreateScope();
//    IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
//    IMessageProducer messageProducer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();

//    // Retrieve the help request with messages
//    var helpRequest = await dbContext.HelpRequests
//        .Include(hr => hr.Messages)
//        .FirstOrDefaultAsync(hr => hr.Id == helpRequestId, cancellationToken);

//    if (helpRequest == null)
//    {
//        _logger.LogWarning("Help request not found with ID: {HelpRequestId}", helpRequestId);
//        return;
//    }

//    // Create response message
//    var responseMessage = new HelpRequestResponseMessage
//    {
//        HelpRequestId = helpRequest.Id,
//        TenantHelpRequestId = helpRequest.TenantHelpRequestId,
//        SchoolId = helpRequest.SchoolId,
//        TicketId = helpRequest.TicketId,
//        Status = helpRequest.Status,
//        CreatedAt = helpRequest.CreatedAt,
//        UpdatedAt = helpRequest.UpdatedAt,
//        Messages = helpRequest.Messages?.Select(m => new HelpRequestMessageResponse
//        {
//            Id = m.Id,
//            Title = m.Title,
//            Attachments = m.Attachments?.ToList() ?? new List<string>(),
//            CreatedAt = m.CreatedAt
//        }).ToList() ?? new List<HelpRequestMessageResponse>()
//    };

//    // Send response back to tenant
//    await messageProducer.SendMessageAsync(
//        "HelpRequestResponse",
//        responseMessage,
//        _kafkaSettings.HelpRequestRespondTopic);

//    _logger.LogInformation("Successfully sent help request response. HelpRequestId: {HelpRequestId}, TenantHelpRequestId: {TenantHelpRequestId}, SchoolId: {SchoolId}",
//        helpRequest.Id, helpRequest.TenantHelpRequestId, helpRequest.SchoolId);
//}
//catch (JsonException ex)
//{
//    _logger.LogError(ex, "Failed to deserialize help request response message: {Message}", messageJson);
//}
//catch (Exception ex)
//{
//    _logger.LogError(ex, "Failed to process help request response");
//}
