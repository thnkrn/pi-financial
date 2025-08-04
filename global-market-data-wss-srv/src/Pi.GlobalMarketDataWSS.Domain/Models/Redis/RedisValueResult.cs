namespace Pi.GlobalMarketDataWSS.Domain.Models.Redis;

public class RedisValueResult
{
    public RedisChannel RedisChannel { get; set; }
    public object? RedisValue { get; set; }
}