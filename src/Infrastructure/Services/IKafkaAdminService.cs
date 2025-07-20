using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public interface IKafkaAdminService
{
    Task EnsureTopicsExistAsync(params string[] topicNames);
}

public sealed class KafkaAdminService : IKafkaAdminService
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaAdminService> _logger;

    public KafkaAdminService(IOptions<KafkaSettings> kafkaSettings, ILogger<KafkaAdminService> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task EnsureTopicsExistAsync(params string[] topicNames)
    {
        var config = new AdminClientConfig { BootstrapServers = _kafkaSettings.BootstrapServers };

        using IAdminClient adminClient = new AdminClientBuilder(config).Build();

        try
        {
            Metadata metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            var existingTopics = metadata.Topics
                .Where(t => t.Error.Code == ErrorCode.NoError)
                .Select(t => t.Topic)
                .ToHashSet();

            var topicsToCreate = topicNames
                .Where(topic => !existingTopics.Contains(topic))
                .Select(topic => new TopicSpecification
                {
                    Name = topic,
                    NumPartitions = 1,
                    ReplicationFactor = 1
                })
                .ToList();

            if (topicsToCreate.Any())
            {
                await adminClient.CreateTopicsAsync(topicsToCreate);
                foreach (TopicSpecification? topic in topicsToCreate)
                {
                    _logger.LogInformation("Kafka topic '{Topic}' created successfully.", topic.Name);
                }
            }
            else
            {
                _logger.LogInformation("All Kafka topics already exist.");
            }
        }
        catch (CreateTopicsException ex)
        {
            foreach (CreateTopicReport? result in ex.Results)
            {
                if (result.Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    _logger.LogError(ex, "Failed to create Kafka topic '{Topic}': {Reason}", result.Topic, result.Error.Reason);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while ensuring Kafka topics exist.");
        }
    }
}
