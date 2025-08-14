using System;
using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Models;
using Application.Interfaces;
using Application.Interfaces.Services;
using Confluent.Kafka;
using Domain.HelpRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Enums;

namespace Infrastructure.Services;
public class HelpRequestGetHandler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<HelpRequestGetHandler> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly IKafkaAdminService _kafkaAdminService;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public HelpRequestGetHandler(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<HelpRequestGetHandler> logger,
        IKafkaAdminService kafkaAdminService)
    {
        _serviceProvider = serviceProvider;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = "help-request-get-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _kafkaAdminService = kafkaAdminService;
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
            _consumer.Subscribe(_kafkaSettings.HelpRequestGetTopic);
            _logger.LogInformation("Started consuming from topic: {Topic}", _kafkaSettings.HelpRequestGetTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ConsumeResult<string, string>? consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Received message from topic {Topic}", consumeResult.Topic);
                        await ProcessHelpRequest(consumeResult.Message.Value, stoppingToken);
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
            _logger.LogError(ex, "Error in tenant response handler service");
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

    private async Task ProcessHelpRequest(string messageJson, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Processing help request message: {Message}", messageJson);

            KafkaMessage<HelpRequestGetMessage>? kafkaMessage = JsonSerializer.Deserialize<KafkaMessage<HelpRequestGetMessage>>(messageJson, JsonOptions);

            if (kafkaMessage?.Data == null)
            {
                _logger.LogWarning("Received null or empty help request data");
                return;
            }

            HelpRequestGetMessage helpRequestData = kafkaMessage.Data;

            using IServiceScope scope = _serviceProvider.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            HelpRequests? existingHelpRequest = await dbContext.HelpRequests
                .FirstOrDefaultAsync(hr => hr.TenantHelpRequestId == helpRequestData.TenantHelpRequestId
                                         && hr.SchoolId == helpRequestData.SchoolId, cancellationToken);

            if (existingHelpRequest != null)
            {
                _logger.LogWarning("Help request already exists. TenantHelpRequestId: {TenantHelpRequestId}, SchoolId: {SchoolId}",
                    helpRequestData.TenantHelpRequestId, helpRequestData.SchoolId);
                return;
            }

            var helpRequest = new HelpRequests
            {
                TicketId = helpRequestData.TicketId,
                Status = helpRequestData.Status,
                Category = helpRequestData.Category,
                UserProfilePic = helpRequestData.UserProfilePic,
                UserName = helpRequestData.UserName,
                TenantHelpRequestId = helpRequestData.TenantHelpRequestId,
                SchoolId = helpRequestData.SchoolId,
                Messages = new List<HelpRequestMessages>
                {
                    new HelpRequestMessages
                    {
                        Title = helpRequestData.InitialMessage.Title,
                        Attachments = helpRequestData.InitialMessage.Attachments ?? new List<string>(),
                    }
                }
            };

            dbContext.HelpRequests.Add(helpRequest);
            await dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully created help request. HelpRequestId: {HelpRequestId}, TenantHelpRequestId: {TenantHelpRequestId}, SchoolId: {SchoolId}",
                helpRequest.Id, helpRequest.TenantHelpRequestId, helpRequestData.SchoolId);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize help request message: {Message}", messageJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process help request");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
public class HelpRequestGetMessage
{
    public string? TicketId { get; set; }
    public HelpStatus Status { get; set; } = HelpStatus.OPEN_REQUEST;
    public HelpCategory? Category { get; set; }
    public string TenantHelpRequestId { get; set; }
    public string SchoolId { get; set; }
    public string UserProfilePic { get; set; }
    public string UserName { get; set; } = string.Empty;
    public HelpRequestMessageData InitialMessage { get; set; } = new();
}

public class HelpRequestMessageData
{
    public string? Title { get; set; }
    public List<string> Attachments { get; set; } = new();
}

//    _logger.LogDebug("Processing help request message: {Message}", messageJson);

//    // Deserialize the Kafka message wrapper
//    var kafkaMessage = JsonSerializer.Deserialize<KafkaMessage<HelpRequestGetMessage>>(messageJson, JsonOptions);

//    if (kafkaMessage?.Data == null)
//    {
//        _logger.LogWarning("Received null or empty help request data");
//        return;
//    }

//    var helpRequestData = kafkaMessage.Data;

//    using var scope = _serviceProvider.CreateScope();
//    var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

//    // Create the help request entity
//    var helpRequest = new HelpRequests
//    {
//        TicketId = helpRequestData.TicketId,
//        Status = helpRequestData.Status,
//        Category = helpRequestData.Category,
//        TenantHelpRequestId = helpRequestData.TenantHelpRequestId,
//        SchoolId = helpRequestData.SchoolId,
//        Messages = helpRequestData.Messages?.Select(m => new HelpRequestMessages
//        {
//            Title = m.Title,
//            Attachments = m.Attachments ?? new List<string>()
//        }).ToList() ?? new List<HelpRequestMessages>()
//    };

//    // Add to database
//    dbContext.HelpRequests.Add(helpRequest);
//    await dbContext.SaveChangesAsync(cancellationToken);

//    _logger.LogInformation("Successfully processed help request. HelpRequestId: {HelpRequestId}, TenantHelpRequestId: {TenantHelpRequestId}, SchoolId: {SchoolId}",
//        helpRequest.Id, helpRequest.TenantHelpRequestId, helpRequest.SchoolId);
//}
//catch (JsonException ex)
//{
//    _logger.LogError(ex, "Failed to deserialize help request message: {Message}", messageJson);
//}
//catch (Exception ex)
//{
//    _logger.LogError(ex, "Failed to process help request");
//}
