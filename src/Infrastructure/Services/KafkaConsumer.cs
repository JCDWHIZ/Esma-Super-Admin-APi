//using System.Text.Json;
//using Application.Abstractions.Models;
//using Confluent.Kafka;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace Infrastructure.Services;

//public class KafkaConsumer : IDisposable
//{
//    private readonly IConsumer<string, string> _consumer;
//    private readonly ILogger<KafkaConsumer> _logger;
//    private readonly KafkaSettings _settings;
//    private static readonly JsonSerializerOptions JsonOptions = new()
//    {
//        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//    };
//    private bool _disposed;

//    public KafkaConsumer(IOptions<KafkaSettings> settings, ILogger<KafkaConsumer> logger)
//    {
//        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
//        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

//        var config = new ConsumerConfig
//        {
//            BootstrapServers = _settings.BootstrapServers,
//            GroupId = "help-request-consumer-group",
//            AutoOffsetReset = AutoOffsetReset.Earliest,
//            EnableAutoCommit = false
//        };

//        _consumer = new ConsumerBuilder<string, string>(config).Build();
//    }

//    public async Task<T> ConsumeMessageAsync<T>(string topic, CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            _consumer.Subscribe(topic);
//            _logger.LogInformation("Subscribed to topic: {Topic}", topic);

//            ConsumeResult<string, string> consumeResult = _consumer.Consume(cancellationToken);

//            if (consumeResult?.Message?.Value != null)
//            {
//                _logger.LogDebug("Received message from topic {Topic}: {Message}", topic, consumeResult.Message.Value);

//                KafkaMessage<T>? kafkaMessage = JsonSerializer.Deserialize<KafkaMessage<T>>(consumeResult.Message.Value, JsonOptions);

//                _consumer.Commit(consumeResult);
//                _logger.LogInformation("Successfully processed and committed message from topic {Topic}", topic);

//                return kafkaMessage != null ? kafkaMessage.Data : default!;
//            }

//            return default!;
//        }
//        catch (OperationCanceledException ex)
//        {
//            _logger.LogInformation(ex, "Consumer operation was cancelled for topic {Topic}", topic);
//            return default!;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "Error consuming message from topic {Topic}", topic);
//            return default!;
//        }
//    }

//    public void Dispose()
//    {
//        Dispose(true);
//        GC.SuppressFinalize(this);
//    }

//    protected virtual void Dispose(bool disposing)
//    {
//        if (!_disposed)
//        {
//            if (disposing)
//            {
//                _consumer?.Close();
//                _consumer?.Dispose();
//            }
//            _disposed = true;
//        }
//    }
//}
