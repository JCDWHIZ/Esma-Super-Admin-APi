namespace Application.Abstractions.Models;

public class KafkaMessage<T>
{
    public required string MessageType { get; set; }
    public required T Data { get; set; }
    public DateTime Timestamp { get; set; }
    public required string MessageId { get; set; }
}
