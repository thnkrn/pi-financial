using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;

namespace Pi.GlobalMarketData.Infrastructure.Services.Redis;

public class RedisPublisher : IRedisPublisher, IDisposable
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;
    private bool _disposed;

    public RedisPublisher(IConfiguration configuration)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(
            configuration[ConfigurationKeys.RedisConnectionString] ?? string.Empty
        );
        _database = _connectionMultiplexer.GetDatabase();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishAsync<T>(string channel, T message)
    {
        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var json = JsonSerializer.Serialize(message, options);

        await _database.PublishAsync(channel, json);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing) _connectionMultiplexer.Dispose();

        _disposed = true;
    }

    ~RedisPublisher()
    {
        Dispose(false);
    }
}