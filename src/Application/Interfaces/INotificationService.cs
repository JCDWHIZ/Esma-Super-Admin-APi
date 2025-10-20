namespace Application.Interfaces;

public interface INotificationService
{
    Task PublishMessageAsync(string topic, string message);
}
