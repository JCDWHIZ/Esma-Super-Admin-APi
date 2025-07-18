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
using Application.Abstractions.Models;

namespace Infrastructure.Services;

public class TenantResponseHandlerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<TenantResponseHandlerService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public TenantResponseHandlerService(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<TenantResponseHandlerService> logger)
    {
        _serviceProvider = serviceProvider;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = "tenant-response-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_kafkaSettings.TenantResponseTopic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult?.Message?.Value != null)
                {
                    await ProcessTenantResponse(consumeResult.Message.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in tenant response handler service");
        }
    }

    private async Task ProcessTenantResponse(string messageJson)
    {
        try
        {
            var message = JsonSerializer.Deserialize<KafkaMessage<TenantCreatedResponse>>(messageJson);
            if (message?.Data == null)
            {
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            if (message.Data.Success)
            {
                // Send welcome email
                var emailMessage = new EmailMessage
                {
                    To = message.Data.OriginalCommand.User.Email,
                    Subject = "Welcome to the School Management System",
                    Body = $"Dear {message.Data.OriginalCommand.User.FirstName},\n\nYour school '{message.Data.OriginalCommand.SchoolName}' has been successfully created.\n\nBest regards,\nThe Team",
                    // Add other email properties as needed
                };

                await emailService.SendEmailAsync(emailMessage);

                _logger.LogInformation("Welcome email sent for school: {SchoolId}", message.Data.SchoolId);
            }
            else
            {
                // Handle failure - send error email or log
                _logger.LogError("Tenant creation failed for school: {SchoolId}. Error: {Error}",
                    message.Data.SchoolId, message.Data.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process tenant response");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}