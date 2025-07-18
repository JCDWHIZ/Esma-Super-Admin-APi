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
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class TenantResponseHandlerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<TenantResponseHandlerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

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
                ConsumeResult<string, string> consumeResult = _consumer.Consume(stoppingToken);

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
            KafkaMessage<TenantCreatedResponse>? message = JsonSerializer.Deserialize<KafkaMessage<TenantCreatedResponse>>(messageJson, JsonOptions);
            if (message?.Data == null)
            {
                return;
            }

            using IServiceScope scope = _serviceProvider.CreateScope();
            IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
            IConfiguration configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var schoolId = Guid.Parse(message.Data.SchoolId);
            Domain.Schools.Schools? school = await dbContext.Schools
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.PublicId == schoolId);

            if (school == null)
            {
                _logger.LogWarning("School not found for ID: {SchoolId}", message.Data.SchoolId);
                return;
            }

            if (message.Data.Success)
            {
                var payload = new Dictionary<string, object>
                {
                    { "schoolId", school.Id },
                    { "schoolPublicId", school.PublicId },
                    { "organizationId", school.OrganizationId ?? string.Empty },
                    { "schoolName", school.SchoolName },
                    { "email", school.EmailAddress },
                    { "firstName", school.User.FirstName },
                    { "lastName", school.User.LastName },
                    { "role", school.User.Role.ToString() },
                    { "username", school.User.Username },
                    { "phoneNumber", school.User?.PhoneNumber ?? string.Empty }
                };

                string token = tokenService.GenerateToken(payload);

                var emailMessage = new EmailMessage
                {
                    Email = school.EmailAddress,
                    Title = "Your School Organization is Ready",
                    SchoolName = school.SchoolName,
                    Description = "We've successfully onboarded your school to our platform. We're excited to share that your school has been successfully added to our platform! This marks the beginning of a seamless, integrated experience designed to empower your institution with the tools and support needed to thrive. Welcome aboard—we're looking forward to growing with you.",
                    EmailButton = true,
                    ButtonLink = $"{configuration["Frontend:BaseUrl"]}/onboarding?token={token}",
                    ButtonText = "Complete Your Setup"
                };

                await emailService.SendEmailAsync(emailMessage);

                _logger.LogInformation("Welcome email sent for school: {SchoolName} ({SchoolId})",
                    school.SchoolName, message.Data.SchoolId);
            }
            else
            {
                _logger.LogError("Tenant creation failed for school: {SchoolName} ({SchoolId}). Error: {Error}",
                    school.SchoolName, message.Data.SchoolId, message.Data.ErrorMessage);

                var errorEmailMessage = new EmailMessage
                {
                    Email = school.EmailAddress,
                    Title = "School Setup Issue",
                    SchoolName = school.SchoolName,
                    Description = "We encountered an issue while setting up your school. Our technical team has been notified and will resolve this shortly. We'll contact you once everything is ready.",
                    EmailButton = false
                };

                await emailService.SendEmailAsync(errorEmailMessage);
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