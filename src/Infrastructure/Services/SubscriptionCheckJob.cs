using Application.Abstractions.Models;
using Application.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedKernel.Enums;

namespace Infrastructure.Services;
public class SubscriptionCheckJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMessageProducer _messageProducer;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<SubscriptionCheckJob> _logger;

    public SubscriptionCheckJob(
        ApplicationDbContext dbContext,
        IMessageProducer messageProducer,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<SubscriptionCheckJob> logger)
    {
        _dbContext = dbContext;
        _messageProducer = messageProducer;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task CheckSubscriptionsAsync()
    {
        _logger.LogInformation("Starting subscription check job at {Time}", DateTime.UtcNow);

        try
        {
            List<Domain.Schools.Schools> schools = await _dbContext.Schools
                .Include(s => s.Subscriptions)
                .Where(s => s.Subscribed)
                .ToListAsync();

            DateTime now = DateTime.UtcNow;
            var notificationIntervals = new List<int> { 50, 30, 15, 5, 4, 3, 2, 1 };

            foreach (Domain.Schools.Schools school in schools)
            {
                if (school.Subscriptions == null || !school.Subscriptions.EndDate.HasValue)
                {
                    _logger.LogWarning("School {SchoolName} (ID: {SchoolId}) has no valid subscription",
                        school.SchoolName, school.Id);
                    continue;
                }

                DateTime endDate = school.Subscriptions.EndDate.Value;
                double daysRemaining = (endDate - now).TotalDays;

                if (daysRemaining <= 0)
                {
                    _logger.LogInformation("Subscription for school {SchoolName} (ID: {SchoolId}) has expired",
                        school.SchoolName, school.Id);

                    if (school.Status != SchoolStatus.INACTIVE)
                    {
                        school.Status = SchoolStatus.INACTIVE;
                        await PublishTenantStatusUpdateAsync(school.PublicId, school.TenantId, TenantUpdateAction.DEACTIVATE);
                    }
                }
                else if (school.Status == SchoolStatus.INACTIVE)
                {
                    school.Status = SchoolStatus.ACTIVE;
                    await PublishTenantStatusUpdateAsync(school.PublicId, school.TenantId, TenantUpdateAction.ACTIVATE);
                }
                else if (notificationIntervals.Any(interval => Math.Abs(daysRemaining - interval) < 0.5))
                {
                    int days = (int)Math.Round(daysRemaining);
                    _logger.LogInformation("Sending reminder for school {SchoolName} (ID: {SchoolId}): {Days} days remaining",
                        school.SchoolName, school.Id, days);
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Completed subscription check job at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during subscription check job");
        }
    }

    private async Task PublishTenantStatusUpdateAsync(Guid schoolPublicId, string? tenantId, TenantUpdateAction action)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            _logger.LogWarning("Skipping UpdateTenantStatus for school {SchoolPublicId} because TenantId is empty", schoolPublicId);
            return;
        }

        var message = new UpdateTenantStatusMessage
        {
            SchoolPublicId = schoolPublicId.ToString(),
            TenantId = tenantId,
            Action = action
        };

        await _messageProducer.SendMessageAsync("UpdateTenantStatus", message, _kafkaSettings.CreateTenantTopic);
    }
}
