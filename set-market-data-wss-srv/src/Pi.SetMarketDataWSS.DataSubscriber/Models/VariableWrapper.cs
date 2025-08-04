using Confluent.Kafka;
using Pi.SetMarketDataWSS.Domain.Models.Redis;

namespace Pi.SetMarketDataWSS.DataSubscriber.Models;

public class VariableWrapper
{
    public RedisValue[]? RedisValue { get; set; }
    public RedisChannel RedisChannel { get; set; }
    public string? CachingKey { get; set; }
    public long Timestamp { get; set; }
    public ulong SequenceNumber { get; set; }
    public Timestamp CreationTime { get; set; }
    public string? MessageType { get; set; }
}