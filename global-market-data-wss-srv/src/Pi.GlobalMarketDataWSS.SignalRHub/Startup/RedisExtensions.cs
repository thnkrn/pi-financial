using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.GlobalMarketDataWSS.SignalRHub.Helpers;
using Pi.GlobalMarketDataWSS.SignalRHub.Services;
using Polly;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Startup;

public static class RedisExtensions
{
    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = RedisConnectionBuilder.Build(configuration);

        // Check if pooling is enabled in configuration
        var poolingEnabled = GetOptionalBoolValue(configuration, "Redis:PoolingEnabled", true);
        if (poolingEnabled)
            // Configure Redis with connection pooling
            ConfigureRedisWithPooling(services, configuration, redisConfig);
        else
            // Use the traditional single connection approach
            ConfigureRedisTraditional(services, redisConfig);

        // Add circuit breaker policy (regardless of pooling approach)
        services.AddSingleton<IAsyncPolicy>(_ => Policy
            .Handle<RedisConnectionException>()
            .CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(20)));
    }

    private static void ConfigureRedisWithPooling(
        IServiceCollection services,
        IConfiguration configuration,
        ConfigurationOptions redisConfig)
    {
        // Get pooling configuration
        var minPoolSize = GetOptionalIntValue(configuration, "Redis:MinPoolSize", 5);
        var maxPoolSize = GetOptionalIntValue(configuration, "Redis:MaxPoolSize", 20);

        // Create a base connection for SignalR backplane
        var baseConnection = ConnectionMultiplexer.Connect(redisConfig);

        // Create and register the connection pool manager
        services.AddSingleton<RedisConnectionPoolManager>(serviceProvider =>
        {
            var logger = serviceProvider.GetRequiredService<ILogger<RedisConnectionPoolManager>>();
            return new RedisConnectionPoolManager(redisConfig, logger, minPoolSize, maxPoolSize);
        });

        // Also register a single connection multiplexer for backward compatibility
        services.AddSingleton<IConnectionMultiplexer>(baseConnection);

        // Configure SignalR
        ConfigureSignalR(services, baseConnection);

        // Add health check for Redis pools
        services.AddHealthChecks()
            .AddCheck<RedisPoolHealthCheck>("redis-pool", HealthStatus.Unhealthy);
    }

    private static void ConfigureRedisTraditional(
        IServiceCollection services,
        ConfigurationOptions redisConfig)
    {
        // Use the traditional single connection approach
        var redis = ConnectionMultiplexer.Connect(redisConfig);
        services.AddSingleton<IConnectionMultiplexer>(redis);

        // Configure SignalR
        ConfigureSignalR(services, redis);

        // Add Redis health check for traditional connection
        services.AddHealthChecks()
            .AddRedis(redis, "redis", HealthStatus.Unhealthy);
    }

    private static void ConfigureSignalR(
        IServiceCollection services,
        IConnectionMultiplexer redis)
    {
        // Configure SignalR with Redis backplane
        services.AddSignalR(options =>
        {
            options.MaximumReceiveMessageSize = 1024 * 1024; // 1MB
            options.EnableDetailedErrors = false; // Turn off in production
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            options.KeepAliveInterval = TimeSpan.FromSeconds(30);
            options.StreamBufferCapacity = 20;
            options.MaximumParallelInvocationsPerClient = 4; // Default is 1
        }).AddStackExchangeRedis(redis.Configuration,
            options => { options.Configuration.ChannelPrefix = RedisChannel.Literal("GlobalSignalR"); });
    }

    private static int GetOptionalIntValue(IConfiguration configuration, string key, int defaultValue)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value)) return defaultValue;
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    private static bool GetOptionalBoolValue(IConfiguration configuration, string key, bool defaultValue)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value)) return defaultValue;
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }
}