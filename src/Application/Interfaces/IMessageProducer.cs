namespace Application.Interfaces;

public interface IMessageProducer
{
    Task<string> SendMessageAsync<T>(string messageType, T data, string? topic = null);
}