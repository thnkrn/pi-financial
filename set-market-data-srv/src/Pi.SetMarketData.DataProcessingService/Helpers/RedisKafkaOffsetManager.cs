using Confluent.Kafka;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.DataProcessingService.Helpers;

public interface IKafkaOffsetManager
{
    Task<TopicPartitionOffset?> GetLastCommittedOffsetAsync(string topic, int partition);
    Task SaveOffsetAsync(string topic, int partition, long offset);

    Task<bool> IsOffsetWithinTimeRangeAsync(string topic, int partition, long offset,
        IConsumer<string, string> consumer);
}

public class RedisKafkaOffsetManager : IKafkaOffsetManager
{
    private const string OffsetKeyPrefix = "kafka:offset:";
    private readonly string _consumerGroupId;
    private readonly ILogger<RedisKafkaOffsetManager> _logger;
    private readonly TimeSpan _offsetKeyExpiration;
    private readonly DateTimeOffset _recoveryEndTime;
    private readonly DateTimeOffset _recoveryStartTime;
    private readonly IRedisV2Publisher _redisService;

    public RedisKafkaOffsetManager(
        IRedisV2Publisher redisService,
        ILogger<RedisKafkaOffsetManager> logger,
        string consumerGroupId,
        DateTimeOffset recoveryStartTime,
        DateTimeOffset recoveryEndTime,
        int offsetKeyExpirationDays = 30)
    {
        _redisService = redisService ?? throw new ArgumentNullException(nameof(redisService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _consumerGroupId = consumerGroupId ?? throw new ArgumentNullException(nameof(consumerGroupId));
        _recoveryStartTime = recoveryStartTime;
        _recoveryEndTime = recoveryEndTime;
        _offsetKeyExpiration = TimeSpan.FromDays(offsetKeyExpirationDays);

        _logger.LogInformation(
            "Redis Kafka Offset Manager initialized for group {GroupId} with date range {StartDate:yyyy-MM-dd} - {EndDate:yyyy-MM-dd}",
            consumerGroupId, recoveryStartTime, recoveryEndTime);
    }

    public async Task<TopicPartitionOffset?> GetLastCommittedOffsetAsync(string topic, int partition)
    {
        try
        {
            var key = GetOffsetKey(topic, partition);
            var offsetStr = await _redisService.GetAsync<string>(key);

            if (long.TryParse(offsetStr, out var offset))
            {
                _logger.LogDebug("Retrieved offset {Offset} for {Topic}:{Partition}", offset, topic, partition);
                return new TopicPartitionOffset(
                    new TopicPartition(topic, new Partition(partition)),
                    new Offset(offset));
            }

            _logger.LogDebug("No saved offset found for {Topic}:{Partition}", topic, partition);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving offset for {Topic}:{Partition}", topic, partition);
            return null;
        }
    }

    public async Task SaveOffsetAsync(string topic, int partition, long offset)
    {
        try
        {
            var key = GetOffsetKey(topic, partition);
            await _redisService.SetAsync(key, offset.ToString(), false, _offsetKeyExpiration);
            _logger.LogDebug("Saved offset {Offset} for {Topic}:{Partition}", offset, topic, partition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving offset {Offset} for {Topic}:{Partition}",
                offset, topic, partition);
        }
    }

    public async Task<bool> IsOffsetWithinTimeRangeAsync(
        string topic,
        int partition,
        long offset,
        IConsumer<string, string> consumer)
    {
        await Task.Yield();

        try
        {
            consumer.Assign(new TopicPartitionOffset(
                new TopicPartition(topic, new Partition(partition)),
                new Offset(offset)));

            var result = consumer.Consume(TimeSpan.FromSeconds(5));
            if (result != null)
            {
                var messageTimestamp = result.Message.Timestamp.UtcDateTime;
                var isWithinRange = messageTimestamp >= _recoveryStartTime.UtcDateTime &&
                                    messageTimestamp <= _recoveryEndTime.UtcDateTime;

                _logger.LogDebug("Offset {Offset} timestamp {Timestamp} is {Status} range {Start}-{End}",
                    offset, messageTimestamp, isWithinRange ? "within" : "outside",
                    _recoveryStartTime, _recoveryEndTime);

                return isWithinRange;
            }

            _logger.LogWarning("Could not determine timestamp for offset {Offset}", offset);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if offset {Offset} is within time range", offset);
            return false;
        }
    }

    private string GetOffsetKey(string topic, int partition)
    {
        return
            $"{OffsetKeyPrefix}{_consumerGroupId}:{topic}:{partition}:{_recoveryStartTime:yyyyMMdd}-{_recoveryEndTime:yyyyMMdd}";
    }
}