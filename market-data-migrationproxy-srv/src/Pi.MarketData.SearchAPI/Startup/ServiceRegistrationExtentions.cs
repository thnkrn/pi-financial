using Pi.MarketData.SearchAPI.Services;
using StackExchange.Redis;
namespace Pi.MarketData.SearchAPI.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddScoped<IInstrumentSearchService, InstrumentSearchService>();
        services.AddScoped<IOrderBookGetterService, OrderBookGetterService>();
        services.AddScoped<IUserFavoriteAndPositionService, UserFavoriteAndPositionService>();
        services.AddScoped<IStreamingDataCacheService>(sp =>
        {
            var redisConfig = configuration.GetSection("Redis");
            var cacheKeyPrefix = redisConfig["CacheKeyPrefix"] ?? "marketdata::streaming-body-";
            var database = redisConfig["Database"] ?? throw new InvalidOperationException("Redis database not configured");
            var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
            var redisDb = multiplexer.GetDatabase(int.Parse(database));
            var logger = sp.GetRequiredService<ILogger<StreamingDataCacheService>>();
            return new StreamingDataCacheService(redisDb, logger, cacheKeyPrefix);
        });
        services.AddScoped<IOrderBookGetterService, OrderBookGetterService>();
        services.AddScoped<ILogoService>(sp =>
        {
            var logoConfig = configuration.GetSection("Logo");
            var baseUrl = logoConfig["BaseUrl"] ?? throw new InvalidOperationException("Logo base URL not configured");
            return new LogoService(baseUrl);
        });
        return services;
    }
}