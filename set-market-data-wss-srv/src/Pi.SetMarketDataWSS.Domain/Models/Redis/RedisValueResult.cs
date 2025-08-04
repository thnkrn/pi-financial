namespace Pi.SetMarketDataWSS.Domain.Models.Redis;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class RedisValueResult
{
    public RedisChannel RedisChannel { get; set; }
    public RedisValue[]? RedisValue { get; set; }
}

public class RedisValue
{
    public string? Key { get; set; }
    public object? Value { get; set; }
}