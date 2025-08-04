using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Services.Redis;

namespace Pi.GlobalMarketData.API.Startup;

public static class DistributedCacheExtensions
{
    public static IServiceCollection AddCacheService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (configuration.GetValue<bool>(ConfigurationKeys.RedisEnabled))
        {
            services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
            services.AddSingleton<ICacheService, RedisCacheService>();
        }
        else
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}