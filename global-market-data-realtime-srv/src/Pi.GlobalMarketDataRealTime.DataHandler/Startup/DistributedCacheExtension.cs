using Microsoft.Extensions.Caching.StackExchangeRedis;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Redis;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Startup;

public static class DistributedCacheExtension
{
    public static IServiceCollection AddCacheService(this IServiceCollection services,
        IConfiguration configuration)
    {
        if (configuration.GetValue<bool>(ConfigurationKeys.RedisEnabled))
        {
            // Register ICacheService and RedisCacheService
            services.AddStackExchangeRedisCache(options => ConfigureRedisOptions(options, configuration));
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            // Register ICacheService and MemoryCacheService
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }

    private static void ConfigureRedisOptions(RedisCacheOptions options, IConfiguration configuration)
    {
        var redisHost = configuration.GetValue<string>(ConfigurationKeys.RedisHost);
        var redisPort = configuration.GetValue<int>(ConfigurationKeys.RedisPort);
        options.ConfigurationOptions = new ConfigurationOptions
        {
            AbortOnConnectFail = configuration.GetValue<bool>(ConfigurationKeys.RedisAbortOnConnectFail),
            Ssl = configuration.GetValue<bool>(ConfigurationKeys.RedisSsl),
            ClientName = configuration.GetValue<string>(ConfigurationKeys.RedisClientName),
            ConnectRetry = configuration.GetValue<int>(ConfigurationKeys.RedisConnectRetry),
            ConnectTimeout = configuration.GetValue<int>(ConfigurationKeys.RedisConnectTimeout),
            SyncTimeout = configuration.GetValue<int>(ConfigurationKeys.RedisSyncTimeout),
            // DefaultDatabase = configuration.GetValue<int>(ConfigurationKeys.RedisDatabase),
            EndPoints = { $"{redisHost}:{redisPort}" },
            User = configuration.GetValue<string>(ConfigurationKeys.RedisUser),
            Password = configuration.GetValue<string>(ConfigurationKeys.RedisPassword),
            TieBreaker = configuration.GetValue<string>(ConfigurationKeys.RedisTieBreaker) ?? string.Empty
        };

        options.ConnectionMultiplexerFactory = async () =>
            await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions);
    }
}