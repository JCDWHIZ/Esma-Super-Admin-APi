using System.Text.Json;
using Application.Abstractions.Data;
using Application.Abstractions.Models;
using Application.BackgroundJobs;
using Application.Interfaces;
using Confluent.Kafka;
using Domain.Schools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Enums;

namespace Infrastructure.Services;

public class TenantResponseHandlerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<TenantResponseHandlerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = null
    };
    private readonly IKafkaAdminService _kafkaAdminService;

    public TenantResponseHandlerService(
        IServiceProvider serviceProvider,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<TenantResponseHandlerService> logger,
        IKafkaAdminService kafkaAdminService)
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
                    ConsumeResult<string, string>? consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult?.Message?.Value != null)
                    {
                        _logger.LogInformation("Received message from topic {Topic}", consumeResult.Topic);
                        await ProcessTenantResponse(consumeResult.Message.Value, stoppingToken);
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

    private async Task ProcessTenantResponse(string messageJson, CancellationToken cancellationToken)
    {
        try
        {
            KafkaMessage<JsonElement>? message = JsonSerializer.Deserialize<KafkaMessage<JsonElement>>(messageJson, JsonOptions);
            if (message == null)
            {
                return;
            }

            switch (message.MessageType)
            {
                case "TenantStatusUpdated":
                    await HandleTenantStatusUpdatedAsync(message.Data, cancellationToken);
                    break;
                case "TenantCreationFailed":
                    await HandleTenantFailure(message.Data, cancellationToken);
                    break;
                default:
                    await HandleTenantCreatedResponseAsync(message.Data, cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process tenant response");
        }
    }

    private async Task HandleTenantCreatedResponseAsync(JsonElement data, CancellationToken cancellationToken)
    {
        TenantCreatedResponse? tenantCreated = data.Deserialize<TenantCreatedResponse>(JsonOptions);
        if (tenantCreated == null)
        {
            return;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>(); 
        IKeycloakOrganizationService keycloakOrgService = scope.ServiceProvider.GetRequiredService<IKeycloakOrganizationService>(); 
        ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

        var schoolId = Guid.Parse(tenantCreated.SchoolPublicId);
        Domain.Schools.Schools? school = await dbContext.Schools
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.PublicId == schoolId, cancellationToken);

        if (school == null)
        {
            _logger.LogWarning("School not found for ID: {SchoolId}", tenantCreated.SchoolPublicId);
            return;
        }

        if (tenantCreated.Success)
        {

            try
            {
                await keycloakOrgService.CreateKeycloackSchool(school, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating school in keycloack ({SchoolName}), {SchoolPublicId}", school.SchoolName, tenantCreated.SchoolPublicId);
            }

            school.TenantId = tenantCreated.TenantId;
            school.Status = SchoolStatus.ACTIVE;

            await dbContext.SaveChangesAsync(cancellationToken);
            var payload = new Dictionary<string, object>
            {
                { "schoolId", school.Id },
                { "schoolPublicId", school.PublicId },
                { "organizationId", school.OrganizationId ?? string.Empty },
                { "schoolName", school.SchoolName },
                { "email", school.User.Email },
                { "firstName", school.User.FirstName },
                { "lastName", school.User.LastName },
                { "role", school.User.Role.ToString() },
                { "username", school.User.Username },
                { "phoneNumber", school.User?.PhoneNumber ?? string.Empty },
                { "tenantId", school.TenantId }
            };

            string token = tokenService.GenerateToken(payload);

            var emailMessage = new EmailMessage
            {
                Email = school.User?.Email ?? school.EmailAddress,
                Title = "Your School Organization is Ready",
                Name = school.SchoolName,
                Description = "We've successfully onboarded your school to our platform. We're excited to share that your school has been successfully added to our platform! This marks the beginning of a seamless, integrated experience designed to empower your institution with the tools and support needed to thrive. Welcome aboard-we're looking forward to growing with you.",
                EmailButton = true,
                ButtonLink = $"{school.ShortCode}.esma.dev.elsoft.ng/auth/set-password?token={token}",
                ButtonText = "Complete Your Setup"
            };

            await emailService.SendEmailAsync(emailMessage);

            _logger.LogInformation("Welcome email sent for school: {SchoolName} ({SchoolId})",
                school.SchoolName, tenantCreated.SchoolPublicId);
        }
        else
        {
            _logger.LogError("Tenant creation failed for school: {SchoolName} ({SchoolId}). Error: {Error}",
                school.SchoolName, tenantCreated.SchoolPublicId, tenantCreated.ErrorMessage);
        }
    }

    private async Task HandleTenantStatusUpdatedAsync(JsonElement data, CancellationToken cancellationToken)
    {
        TenantStatusUpdatedResponse? statusUpdated = data.Deserialize<TenantStatusUpdatedResponse>(JsonOptions);
        if (statusUpdated == null)
        {
            return;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        var schoolId = Guid.Parse(statusUpdated.SchoolPublicId);
        Domain.Schools.Schools? school = await dbContext.Schools
            .FirstOrDefaultAsync(s => s.PublicId == schoolId, cancellationToken);

        if (school == null)
        {
            _logger.LogWarning("School not found for tenant status update. SchoolPublicId: {SchoolPublicId}", statusUpdated.SchoolPublicId);
            return;
        }

        if (!statusUpdated.Success)
        {
            _logger.LogError("Tenant status update failed for school {SchoolPublicId} and tenant {TenantId}. Error: {Error}",
                statusUpdated.SchoolPublicId, statusUpdated.TenantId, statusUpdated.ErrorMessage);
            return;
        }

        school.Status = statusUpdated.Action == TenantUpdateAction.DEACTIVATE
            ? SchoolStatus.INACTIVE
            : SchoolStatus.ACTIVE;

        await dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Tenant status updated for school {SchoolPublicId}. Action: {Action}",
            statusUpdated.SchoolPublicId, statusUpdated.Action);
    }

    private async Task HandleTenantFailure(JsonElement data, CancellationToken cancellationToken)
    {
        TenantCreationFailureResponse? tenantFailure = data.Deserialize<TenantCreationFailureResponse>(JsonOptions);
        if (tenantFailure == null)
        {
            return;
        }

        using IServiceScope scope = _serviceProvider.CreateScope();
        IApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        if(tenantFailure.SchoolPublicId == null)
        {
            _logger.LogWarning("SchoolPublicId is null in tenant creation failure response. Error: {Error}", tenantFailure.ErrorMessage);
            return;
        }

        var schoolId = Guid.Parse(tenantFailure.SchoolPublicId);
        Domain.Schools.Schools? school = await dbContext.Schools
            .FirstOrDefaultAsync(s => s.PublicId == schoolId, cancellationToken);

        if (school == null)
        {
            _logger.LogWarning("School not found for tenant creation failure. SchoolPublicId: {SchoolPublicId}", tenantFailure.SchoolPublicId);
            return;
        }
        school.Status = SchoolStatus.FAILED;
        await dbContext.SaveChangesAsync(cancellationToken);

        string recipientEmail = string.Empty;
        string recipientName = string.Empty;

        if (school.CreatedBy.HasValue)
        {
            Domain.Users.User? creator = await dbContext.Users
                .FirstOrDefaultAsync(u => u.PublicId == school.CreatedBy.Value, cancellationToken);

            if (creator != null && !string.IsNullOrWhiteSpace(creator.Email))
            {
                recipientEmail = creator.Email;
                recipientName = string.IsNullOrWhiteSpace(creator.FirstName) && string.IsNullOrWhiteSpace(creator.LastName)
                    ? creator.Username ?? school.SchoolName
                    : $"{creator.FirstName} {creator.LastName}".Trim();
            }
        }

        var errorEmailMessage = new EmailMessage
        {
            Email = recipientEmail,
            Title = "Action Required: Issue Creating Your School Tenant",
            Name = recipientName,
            Description = $"Hello {recipientName},\n\n" +
                   $"We encountered an error while creating the tenant for \"{school.SchoolName}\" (PublicId: {school.PublicId}).\n\n" +
                   $"Error details: {tenantFailure.ErrorMessage ?? "Unavailable"}\n\n" +
                   "Our engineering team has been notified and is investigating the issue. We will update you as soon as the setup is complete. " +
                   "If you have any additional information that may help, please reply to this email.\n\n" +
                   "Regards,\nThe ESMA Team",
            EmailButton = false
        };

        await emailService.SendEmailAsync(errorEmailMessage);
        _logger.LogError("Tenant creation failed for school {SchoolPublicId}. Error: {Error}",
            tenantFailure.SchoolPublicId, tenantFailure.ErrorMessage);
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
