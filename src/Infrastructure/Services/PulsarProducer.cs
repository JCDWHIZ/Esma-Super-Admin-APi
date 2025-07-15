using System;
using System.Text;
using System.Text.Json;
using admin_service.Application.Common.Interfaces;
using Application.Abstractions.Models;
using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace admin_service.Infrastructure.Services;

public class PulsarProducer : IMessageProducer, IDisposable
{
    private readonly IPulsarClient _pulsarClient;
    private readonly ILogger<PulsarProducer> _logger;
    private readonly PulsarSettings _settings;
    private bool _disposed = false;

    public PulsarProducer(IPulsarClient pulsarClient, IOptions<PulsarSettings> settings, ILogger<PulsarProducer> logger)
    {
        _pulsarClient = pulsarClient ?? throw new ArgumentNullException(nameof(pulsarClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logger.LogInformation("Creating producer for topic: {Topic}", actualTopic);

            // Create a producer for the specified topic
            var producer = _pulsarClient.NewProducer()
                .Topic(actualTopic)
                // Disable batching to ensure messages are sent as complete units
                // .EnableBatching(false)
                // Set a reasonable timeout
                // .SendTimeout(TimeSpan.FromSeconds(5))
                .Create();

            await using (producer)
            {
                // Create the message wrapper
                var messageWrapper = new PulsarMessage<T>
                {
                    MessageType = messageType,
                    Data = data
                };

                // Serialize the message to JSON

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string messageJson = JsonSerializer.Serialize(messageWrapper, options);

                byte[] messageBytes = Encoding.UTF8.GetBytes(messageJson);

                _logger.LogDebug("Sending message: {Message}", messageJson);

                // Send the message to Pulsar
                var messageId = await producer.Send(messageBytes);

                _logger.LogInformation("Message sent successfully with ID: {MessageId}", messageId);

                return messageId.ToString();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to topic {Topic}", actualTopic);
            throw;
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
                // No need to dispose _pulsarClient here as it should be managed by DI
            }

            _disposed = true;
        }
    }
}

public class PulsarSettings
{
    public required string ServiceUrl { get; set; }
    public required string DefaultTopic { get; set; }
    // public bool UseTls { get; set; }
    // public required string TlsCertificatePath { get; set; }
}
