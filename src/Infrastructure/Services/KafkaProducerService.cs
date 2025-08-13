using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Application.Abstractions.Models;
using Application.Interfaces;

namespace Infrastructure.Services;

public class KafkaProducer : IMessageProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly KafkaSettings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private bool _disposed;


    public KafkaProducer(IOptions<KafkaSettings> settings, ILogger<KafkaProducer> logger)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.All,
            RetryBackoffMs = 1000,
            MessageSendMaxRetries = 3
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<string> SendMessageAsync<T>(string messageType, T data, string? topic = null)
    {
        if (string.IsNullOrEmpty(messageType))
        {
            throw new ArgumentException("Message type cannot be null or empty", nameof(messageType));
        }

        string actualTopic = topic ?? _settings.DefaultTopic;

        if (string.IsNullOrEmpty(actualTopic))
        {
            throw new InvalidOperationException("No topic specified and no default topic configured");
        }

        try
        {
            var messageWrapper = new KafkaMessage<T>
            {
                MessageType = messageType,
                Data = data,
                Timestamp = DateTime.UtcNow,
                MessageId = Guid.NewGuid().ToString()
            };

            string messageJson = JsonSerializer.Serialize(messageWrapper, JsonOptions);

            _logger.LogDebug("Sending message to topic {Topic}: {Message}", actualTopic, messageJson);

            var message = new Message<string, string>
            {
                Key = messageWrapper.MessageId,
                Value = messageJson,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            DeliveryResult<string, string> result = await _producer.ProduceAsync(actualTopic, message);

            _logger.LogInformation("Message sent successfully to topic {Topic} with offset {Offset}",
                actualTopic, result.Offset);

            return result.Offset.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to topic {Topic}", actualTopic);
            throw new Exception($"Failed to send message to topic {actualTopic}", ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _producer?.Dispose();
            }
            _disposed = true;
        }
    }
}
public class KafkaSettings
{
    public required string BootstrapServers { get; set; }
    public required string DefaultTopic { get; set; }
    public required string CreateOrganizationTopic { get; set; }
    public required string CreateTenantTopic { get; set; }
    public required string EmailTopic { get; set; }
    public required string TenantResponseTopic { get; set; }
    public required string HelpRequestGetTopic { get; set; }
    public required string HelpRequestRespondTopic { get; set; }
}
