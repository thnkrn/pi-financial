using Microsoft.Extensions.Options;
using Pi.Client.FundMarketData.Api;
using Pi.MarketData.Search.API.Interfaces;
using Pi.MarketData.Search.API.Services;
using Pi.MarketData.Search.Application.Configs;
using Pi.MarketData.Search.Application.Services;
using Pi.MarketData.Search.Infrastructure.Services;
using StackExchange.Redis;

namespace Pi.MarketData.Search.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDistributedMemoryCache();
            services.AddOptions<InstrumentOrderOptions>()
                .Bind(configuration.GetSection(InstrumentOrderOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<FundMarketDataOptions>()
                .Bind(configuration.GetSection(FundMarketDataOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<IInstrumentSearchService, InstrumentSearchService>();
            services.AddScoped<IOrderBookIdMapperService, OrderBookIdMapperService>();
            services.AddScoped<IOrderBookGetterService, OrderBookGetterService>();
            services.AddScoped<IUserFavoriteAndPositionService, UserFavoriteAndPositionService>();
            services.AddScoped<IStreamingDataCacheService>(sp =>
            {
                var redisConfig = configuration.GetSection("Redis");
                var cacheKeyPrefix = redisConfig["CacheKeyPrefix"] ?? "marketdata::streaming-body-";
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                var redisDb = multiplexer.GetDatabase();
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

            services.AddScoped<IFundApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<FundMarketDataOptions>>();
                return new FundApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("FundMarketDataApi"), options.Value.Host);
            });
            services.AddScoped<IFundMarketDataService, FundMarketDataService>();

            return services;
        }
    }
}

