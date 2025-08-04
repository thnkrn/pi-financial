using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Infrastructure.Helpers;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.SetMarketDataWSS.Infrastructure.Services.Redis;

public sealed class RedisPublisher : IRedisPublisher, IDisposable
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private readonly string _keyspace;

    private readonly JsonSerializerOptions _options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private bool _disposed;

    public RedisPublisher(IConfiguration configuration)
    {
        var redisHost = configuration[ConfigurationKeys.RedisHost];
        var redisPort = int.Parse(configuration[ConfigurationKeys.RedisPort] ?? "6379");
        var redisUser = configuration[ConfigurationKeys.RedisUser];
        var redisPassword = configuration[ConfigurationKeys.RedisPassword];
        var redisDb = int.Parse(configuration[ConfigurationKeys.RedisDatabase] ?? "0");
        var clientName = configuration[ConfigurationKeys.RedisClientName];
        var connectTimeout = int.Parse(configuration[ConfigurationKeys.RedisConnectTimeout] ?? "1000");
        var syncTimeout = int.Parse(configuration[ConfigurationKeys.RedisSyncTimeout] ?? "1000");
        var ssl = bool.Parse(configuration[ConfigurationKeys.RedisSsl] ?? string.Empty);
        var abortOnConnectFail = bool.Parse(configuration[ConfigurationKeys.RedisAbortOnConnectFail] ?? "false");
        var connectRetry = int.Parse(configuration[ConfigurationKeys.RedisConnectRetry] ?? "3");
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
            // DefaultDatabase = redisDb,
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


    public async Task PublishAsync<T>(string channel, T message, bool compress = false)
    {
        if (!string.IsNullOrEmpty(_keyspace))
            channel = $"{_keyspace}{channel}";

        var json = JsonSerializer.Serialize(message, _options);

        if (compress)
        {
            var compressedBytes = CompressionHelper.CompressString(json);
            await _database.PublishAsync(channel, compressedBytes);
        }
        else
        {
            await _database.PublishAsync(channel, json);
        }
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
            _connectionMultiplexer.Dispose();

        _disposed = true;
    }

    ~RedisPublisher()
    {
        Dispose(false);
    }
}