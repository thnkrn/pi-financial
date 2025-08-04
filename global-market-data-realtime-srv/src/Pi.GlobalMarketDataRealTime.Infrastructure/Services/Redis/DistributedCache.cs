using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Redis;

public class DistributedCache : IDistributedCache
{
    private readonly IDatabase _db;

    public DistributedCache(IConfiguration configuration)
    {
        IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(
            configuration[ConfigurationKeys.RedisConnectionString] ?? string.Empty
        );
        _db = connectionMultiplexer.GetDatabase();
    }

    public string? GetString(string? key)
    {
        var value = _db.StringGet(key);
        return value.HasValue ? value.ToString() : null;
    }

    public void SetString(string key, string value)
    {
        _db.StringSet(key, value);
    }
}