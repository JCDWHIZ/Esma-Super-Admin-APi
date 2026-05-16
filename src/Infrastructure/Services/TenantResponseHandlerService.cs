using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Models;
using Application.Interfaces;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
    private readonly IKafkaAdminService _kafkaAdminService;

    public TenantResponseHandlerService(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<TenantResponseHandlerService> logger, IKafkaAdminService kafkaAdminService)
    {
        _serviceProvider = serviceProvider;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _kafkaAdminService = kafkaAdminService;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = "tenant-response-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    // protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    // {
    //         await _kafkaAdminService.EnsureTopicsExistAsync(
    //             _kafkaSettings.DefaultTopic,
    //             _kafkaSettings.CreateOrganizationTopic,
    //             _kafkaSettings.CreateTenantTopic,
    //             _kafkaSettings.EmailTopic,
    //             _kafkaSettings.TenantResponseTopic
    //         );

    //     _consumer.Subscribe(_kafkaSettings.TenantResponseTopic);

    //     _logger.LogInformation("Started consuming from topic: {Topic}", _kafkaSettings.TenantResponseTopic);
    //     try
    //     {
    //         while (!stoppingToken.IsCancellationRequested)
    //         {
    //             ConsumeResult<string, string> consumeResult = _consumer.Consume(stoppingToken);

    //             if (consumeResult?.Message?.Value != null)
    //             {
    //                 await ProcessTenantResponse(consumeResult.Message.Value);
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error in tenant response handler service");
    //     }
    // }
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

            _consumer.Subscribe(_kafkaSettings.TenantResponseTopic);
            _logger.LogInformation("Started consuming from topic: {Topic}", _kafkaSettings.TenantResponseTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Use a timeout to prevent indefinite blocking
                    ConsumeResult<string, string>? consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Received message from topic {Topic}", consumeResult.Topic);
                        await ProcessTenantResponse(consumeResult.Message.Value, stoppingToken);
                    }
                    // If no message received within timeout, continue loop (allows cancellation check)
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
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

    private async Task ProcessTenantResponse(string messageJson, CancellationToken cancellationToken)
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

            var schoolId = Guid.Parse(message.Data.SchoolPublicId);
            Domain.Schools.Schools? school = await dbContext.Schools
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.PublicId == schoolId, cancellationToken);

            if (school == null)
            {
                _logger.LogWarning("School not found for ID: {SchoolId}", message.Data.SchoolPublicId);
                return;
            }

            if (message.Data.Success)
            {
                school.TenantId = message.Data.TenantId;
                school.Status = SharedKernel.Enums.SchoolStatus.ACTIVE;

                await dbContext.SaveChangesAsync(cancellationToken);
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
                    Name = school.SchoolName,
                    Description = "We've successfully onboarded your school to our platform. We're excited to share that your school has been successfully added to our platform! This marks the beginning of a seamless, integrated experience designed to empower your institution with the tools and support needed to thrive. Welcome aboard—we're looking forward to growing with you.",
                    EmailButton = true,
                    ButtonLink = $"{configuration["Frontend:BaseUrl"]}/onboarding?token={token}",
                    ButtonText = "Complete Your Setup"
                };

                await emailService.SendEmailAsync(emailMessage);

                _logger.LogInformation("Welcome email sent for school: {SchoolName} ({SchoolId})",
                    school.SchoolName, message.Data.SchoolPublicId);
            }
            else
            {
                _logger.LogError("Tenant creation failed for school: {SchoolName} ({SchoolId}). Error: {Error}",
                    school.SchoolName, message.Data.SchoolPublicId, message.Data.ErrorMessage);

                var errorEmailMessage = new EmailMessage
                {
                    Email = school.EmailAddress,
                    Title = "School Setup Issue",
                    Name = school.SchoolName,
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
