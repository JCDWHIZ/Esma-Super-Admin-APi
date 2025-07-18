using System;
using System.Text.Json;
using Application.Interfaces;
using Application.Abstractions.Data;
using Application.Interfaces.Services;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class OrganizationCreationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<OrganizationCreationService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public OrganizationCreationService(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<OrganizationCreationService> logger)
    {
        _serviceProvider = serviceProvider;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = "organization-creation-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_kafkaSettings.CreateOrganizationTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string> consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult?.Message?.Value != null)
                {
                    await ProcessOrganizationCreation(consumeResult.Message.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in organization creation service");
        }
    }

    private async Task ProcessOrganizationCreation(string messageJson)
    {
        try
        {
            KafkaMessage<CreateOrganizationMessage>? message = JsonSerializer.Deserialize<KafkaMessage<CreateOrganizationMessage>>(messageJson);
            if (message?.Data == null)
            {
                return;
            }

            using IServiceScope scope = _serviceProvider.CreateScope();
            KeycloakService keycloakService = scope.ServiceProvider.GetRequiredService<KeycloakService>();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            IMessageProducer messageProducer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();

            // Create organization in Keycloak
            string organizationId = await keycloakService.CreateOrganizationAsync(message.Data.SchoolName);

            // Update school with organization ID
            Domain.Schools.Schools? school = await dbContext.Schools.FindAsync(Guid.Parse(message.Data.SchoolId));
            if (school != null)
            {
                school.OrganizationId = organizationId;
                await dbContext.SaveChangesAsync();
            }

            // Enqueue tenant creation task
            var tenantMessage = new CreateTenantMessage
            {
                SchoolId = message.Data.SchoolId,
                OrganizationId = organizationId,
                OriginalCommand = message.Data.OriginalCommand
            };

            await messageProducer.SendMessageAsync(
                "CreateTenant",
                tenantMessage,
                _kafkaSettings.CreateTenantTopic);

            _logger.LogInformation("Organization created and tenant creation task enqueued for school: {SchoolId}",
                message.Data.SchoolId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process organization creation");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}