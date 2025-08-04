using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Redis;

public class RedisPublisher : IRedisPublisher, IDisposable
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly string _keyspace;
    private bool _disposed;

    public RedisPublisher(IConfiguration configuration)
    {
        var redisHost = configuration[ConfigurationKeys.RedisHost] ??
                        throw new ArgumentNullException(nameof(configuration),
                            $"{ConfigurationKeys.RedisHost} is not configured");
        var redisPort = int.Parse(configuration[ConfigurationKeys.RedisPort] ??
                                  throw new ArgumentNullException(nameof(configuration),
                                      $"{ConfigurationKeys.RedisPort} is not configured"));
        var redisUser = configuration[ConfigurationKeys.RedisUser];
        var redisPassword = configuration[ConfigurationKeys.RedisPassword];
        var redisDb = int.Parse(configuration[ConfigurationKeys.RedisDatabase] ??
                                throw new ArgumentNullException(nameof(configuration),
                                    $"{ConfigurationKeys.RedisDatabase} is not configured"));
        var clientName = configuration[ConfigurationKeys.RedisClientName];
        var connectTimeout = int.Parse(configuration[ConfigurationKeys.RedisConnectTimeout] ??
                                       throw new ArgumentNullException(nameof(configuration),
                                           $"{ConfigurationKeys.RedisConnectTimeout} is not configured"));
        var syncTimeout = int.Parse(configuration[ConfigurationKeys.RedisSyncTimeout] ??
                                    throw new ArgumentNullException(nameof(configuration),
                                        $"{ConfigurationKeys.RedisSyncTimeout} is not configured"));
        var ssl = bool.Parse(configuration[ConfigurationKeys.RedisSsl] ??
                             throw new ArgumentNullException(nameof(configuration),
                                 $"{ConfigurationKeys.RedisSsl} is not configured"));
        var abortOnConnectFail = bool.Parse(configuration[ConfigurationKeys.RedisAbortOnConnectFail] ??
                                            throw new ArgumentNullException(nameof(configuration),
                                                $"{ConfigurationKeys.RedisAbortOnConnectFail} is not configured"));
        var connectRetry = int.Parse(configuration[ConfigurationKeys.RedisConnectRetry] ??
                                     throw new ArgumentNullException(nameof(configuration),
                                         $"{ConfigurationKeys.RedisConnectRetry} is not configured"));
        var tieBreaker = configuration[ConfigurationKeys.RedisTieBreaker] ?? string.Empty;

        _keyspace = configuration[ConfigurationKeys.RedisKeyspace] ?? string.Empty;

        var config = new ConfigurationOptions
        {
            EndPoints = { $"{redisHost}:{redisPort}" },
            User = redisUser,
            Password = redisPassword,
            Ssl = ssl,
            ConnectTimeout = connectTimeout,
            SyncTimeout = syncTimeout,
            AbortOnConnectFail = abortOnConnectFail,
            ConnectRetry = connectRetry,
            ClientName = clientName,
            //, DefaultDatabase = redisDb
            TieBreaker = tieBreaker
        };

        _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
        _database = _connectionMultiplexer.GetDatabase();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishAsync<T>(string channel, T message)
    {
        if (string.IsNullOrEmpty(channel))
            throw new ArgumentException("Channel cannot be null or empty", nameof(channel));

        var fullChannel = string.IsNullOrEmpty(_keyspace) ? channel : $"{_keyspace}{channel}";
        var json = JsonConvert.SerializeObject(message);

        await _database.PublishAsync(RedisChannel.Literal(fullChannel), json);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing) _connectionMultiplexer.Dispose();

        _disposed = true;
    }

    ~RedisPublisher()
    {
        Dispose(false);
    }
}