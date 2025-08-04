using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using StackExchange.Redis;

namespace Pi.SetMarketDataWSS.SignalRHub.Helpers;

public static class RedisConnectionBuilder
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger(typeof(RedisConnectionBuilder));

    public static ConfigurationOptions Build(IConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration), "Configuration cannot be null");

        try
        {
            var config = new ConfigurationOptions
            {
                EndPoints =
                {
                    GetRequiredValue(configuration, ConfigurationKeys.RedisHost, ConfigurationKeys.RedisPort)
                },
                User = GetRequiredValue(configuration, ConfigurationKeys.RedisUser),
                Password = GetRequiredValue(configuration, ConfigurationKeys.RedisPassword),
                ClientName = GetRequiredValue(configuration, ConfigurationKeys.RedisClientName),
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                ConnectRetry = GetRequiredIntValue(configuration, ConfigurationKeys.RedisConnectRetry),
                AbortOnConnectFail = GetRequiredBoolValue(configuration, ConfigurationKeys.RedisAbortOnConnectFail),
                Ssl = GetRequiredBoolValue(configuration, ConfigurationKeys.RedisSsl),
                TieBreaker = string.Empty,

                // Additional optimized settings
                KeepAlive = 60,
                AsyncTimeout = 5000,
                AllowAdmin = false,

                // Configure connection resilience
                ReconnectRetryPolicy = new ExponentialRetry(3000),
                CommandMap = CommandMap.Default
            };

            Logger.LogDebug("Redis configuration built successfully.");
            return config;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to build Redis configuration.");
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    private static string GetRequiredValue(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (!string.IsNullOrEmpty(value)) return value;
        var exception = new InvalidOperationException($"Required configuration value '{key}' is missing or empty.");
        Logger.LogError(exception, "Configuration error");
        throw exception;
    }

    private static string GetRequiredValue(IConfiguration configuration, string hostKey, string portKey)
    {
        var host = GetRequiredValue(configuration, hostKey);
        var port = GetRequiredIntValue(configuration, portKey);
        return $"{host}:{port}";
    }

    private static int GetRequiredIntValue(IConfiguration configuration, string key)
    {
        var value = GetRequiredValue(configuration, key);
        if (int.TryParse(value, out var result)) return result;
        var exception = new InvalidOperationException($"Configuration value '{key}' is not a valid integer.");
        Logger.LogError(exception, "Configuration error");
        throw exception;
    }

    private static bool GetRequiredBoolValue(IConfiguration configuration, string key)
    {
        var value = GetRequiredValue(configuration, key);
        if (bool.TryParse(value, out var result)) return result;
        var exception = new InvalidOperationException($"Configuration value '{key}' is not a valid boolean.");
        Logger.LogError(exception, "Configuration error");
        throw exception;
    }
}