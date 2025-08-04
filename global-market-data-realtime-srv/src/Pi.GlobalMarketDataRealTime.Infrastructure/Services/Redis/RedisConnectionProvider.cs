using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Redis;

public interface IRedisConnectionProvider
{
    ConnectionMultiplexer GetConnection();
    IDatabase GetDatabase();
}

public class RedisConnectionProvider : IRedisConnectionProvider
{
    private readonly ConfigurationOptions _configOptions;
    private readonly object _lock = new();
    private readonly TimeSpan? _ttl;
    private Lazy<ConnectionMultiplexer> _connectionMultiplexer;
    private DateTime _createdAt;

    public RedisConnectionProvider(IConfiguration configuration, TimeSpan? ttl = null)
    {
        _ttl = ttl ?? TimeSpan.FromMinutes(30);
        _configOptions = ConfigureRedisOptions(configuration);
        _createdAt = DateTime.UtcNow;
        _connectionMultiplexer = CreateLazyConnection();
    }

    public ConnectionMultiplexer GetConnection()
    {
        if (IsExpired() || !_connectionMultiplexer.Value.IsConnected)
            lock (_lock)
            {
                if (IsExpired() || !_connectionMultiplexer.Value.IsConnected)
                {
                    if (_connectionMultiplexer.IsValueCreated)
                    {
                        _connectionMultiplexer.Value.Close();
                        _connectionMultiplexer.Value.Dispose();
                    }

                    _connectionMultiplexer = CreateLazyConnection();
                    _createdAt = DateTime.UtcNow;
                }
            }

        return _connectionMultiplexer.Value;
    }

    public IDatabase GetDatabase()
    {
        return GetConnection().GetDatabase();
    }

    private Lazy<ConnectionMultiplexer> CreateLazyConnection()
    {
        return new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_configOptions));
    }

    private bool IsExpired()
    {
        return _ttl.HasValue && DateTime.UtcNow - _createdAt > _ttl;
    }

    private ConfigurationOptions ConfigureRedisOptions(IConfiguration configuration)
    {
        var redisHost = configuration.GetValue<string>(ConfigurationKeys.RedisHost);
        var redisPort = configuration.GetValue<int>(ConfigurationKeys.RedisPort);
        return new ConfigurationOptions
        {
            AbortOnConnectFail = configuration.GetValue<bool>(ConfigurationKeys.RedisAbortOnConnectFail),
            Ssl = configuration.GetValue<bool>(ConfigurationKeys.RedisSsl),
            ClientName = configuration.GetValue<string>(ConfigurationKeys.RedisClientName),
            ConnectRetry = configuration.GetValue<int>(ConfigurationKeys.RedisConnectRetry),
            ConnectTimeout = configuration.GetValue<int>(ConfigurationKeys.RedisConnectTimeout),
            SyncTimeout = configuration.GetValue<int>(ConfigurationKeys.RedisSyncTimeout),
            EndPoints = { $"{redisHost}:{redisPort}" },
            User = configuration.GetValue<string>(ConfigurationKeys.RedisUser),
            Password = configuration.GetValue<string>(ConfigurationKeys.RedisPassword),
            TieBreaker = configuration.GetValue<string>(ConfigurationKeys.RedisTieBreaker) ?? string.Empty,
            KeepAlive = 180,
            AsyncTimeout = 15000,
            AllowAdmin = false
        };
    }
}