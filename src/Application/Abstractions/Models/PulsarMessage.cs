namespace Application.Abstractions.Models;

public class PulsarMessage<T>
{
    public required string MessageType { get; set; }
    public required T Data { get; set; }
}