using System;
using admin_service.Application.Common.Interfaces;

namespace admin_service.Application.Common.BackgroundJobs;

public class NotificationJob
{
    private readonly INotificationService _notificationService;

    public NotificationJob(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Execute(string topic, string message)
    {
        await _notificationService.PublishMessageAsync(topic, message);
    }
}
