using System;

namespace admin_service.Application.Common.Interfaces;

public interface INotificationService
{
    Task PublishMessageAsync(string topic, string message);
}
