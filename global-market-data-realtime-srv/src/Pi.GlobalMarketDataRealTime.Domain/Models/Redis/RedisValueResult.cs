namespace Pi.GlobalMarketDataRealTime.Domain.Models.Redis;

// ReSharper disable InconsistentNaming
public class RedisValueResult
{
    public RedisChannel RedisChannel { get; set; }
    public object? RedisValue { get; set; }
}