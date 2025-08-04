using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.GlobalMarketDataWSS.SignalRHub.Helpers;
using Polly;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Startup;

public static class RedisV2Extensions
{
    private static readonly ILogger Logger = LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger(typeof(RedisV2Extensions));

    public static IServiceCollection ConfigureV2Redis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfig = RedisConnectionBuilder.Build(configuration);

        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var multiplexer = ConnectionMultiplexer.Connect(redisConfig);

            multiplexer.ConnectionFailed += (_, args) =>
            {
                Logger.LogError("Redis connection failed: {ExceptionMessage}", args.Exception?.Message);
            };

            multiplexer.ConnectionRestored += (_, _) => { Logger.LogDebug("Redis connection restored"); };

            return multiplexer;
        });

        services.AddSingleton<Func<IDatabase>>(provider =>
        {
            var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
            return () => multiplexer.GetDatabase();
        });

        // Create a base connection for SignalR backplane
        var baseConnection = ConnectionMultiplexer.Connect(redisConfig);
        services.AddSignalR(hubOptions =>
        {
            hubOptions.MaximumReceiveMessageSize = 128 * 1024;
            hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(15);
            hubOptions.StreamBufferCapacity = 20;
            hubOptions.MaximumParallelInvocationsPerClient = 5;
        }).AddStackExchangeRedis(baseConnection.Configuration,
            options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal("GlobalSignalR");
                options.Configuration.HeartbeatInterval = TimeSpan.FromSeconds(15);
                options.ConnectionFactory = async writer =>
                {
                    var connection = await ConnectionMultiplexer.ConnectAsync(redisConfig, writer);
                    connection.ConnectionFailed += (_, e) =>
                    {
                        Logger.LogError(e.Exception, "Redis connection failed: {ExceptionMessage}",
                            e.Exception?.Message);
                    };

                    return connection;
                };
            });

        // Add Redis health check
        services.AddHealthChecks()
            .AddRedis(baseConnection, "redis", HealthStatus.Unhealthy);

        // Circuit Breaker for Redis
        services.AddSingleton<IAsyncPolicy>(_ => Policy
            .Handle<RedisConnectionException>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(20)));

        return services;
    }
}