using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Pi.SetMarketDataWSS.DataStreamer;

public class RedisPublisher
{
    private readonly IDatabase _db;
    private readonly ILogger<RedisPublisher> _logger;

    public RedisPublisher()
    {
        _logger = new Logger<RedisPublisher>(new LoggerFactory());

        var ref1 = ConfigurationHelper.GetConfiguration().GetValue<string>("REDIS:REF_1");
        var ref2 = ConfigurationHelper.GetConfiguration().GetValue<string>("REDIS:REF_2");
        var options = new ConfigurationOptions
        {
            EndPoints = { "pi-redis-nonprod-001.pi-redis-nonprod.pbikfr.apse1.cache.amazonaws.com:6379" },
            User = ref1,
            Password = ref2,
            Ssl = true,
            AbortOnConnectFail = false,
            ConnectTimeout = 5000,
            SyncTimeout = 5000,
            TieBreaker = string.Empty
        };

        try
        {
            var redis = ConnectionMultiplexer.Connect(options);
            _db = redis.GetDatabase(1);
            _logger.LogDebug("Connected to Redis successfully.");
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection error.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error.");
            throw;
        }
    }

    public async Task PublishAsync(string channel, object message)
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            await _db.PublishAsync(channel, jsonMessage);
            _logger.LogDebug("Published message to channel {Channel}", channel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error publishing message to channel {channel}");
        }
    }

    public string? GetString(string key)
    {
        try
        {
            var value = _db.StringGet(key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving value for key {key}");
            return null;
        }
    }

    public void SetString(string key, string value)
    {
        try
        {
            _db.StringSet(key, value);
            _logger.LogDebug("Set value for key {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error setting value for key {key}");
        }
    }
}