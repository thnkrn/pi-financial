using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Handlers.MarketTicker;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.GlobalMarketData.Infrastructure.Services.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Services.Mongo;
using Pi.GlobalMarketData.PreCacher.Services;

namespace Pi.GlobalMarketData.PreCacher.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Register IMongoContext, IMongoService, and IMongoRepository
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddSingleton<IEntityCacheService, EntityCacheService>();
        services.AddSingleton<IVenueMappingHelper, VenueMappingHelper>();
        services.AddScoped(serviceProvider => new MarketTickerServices
        {
            InstrumentService = serviceProvider.GetRequiredService<IMongoService<GeInstrument>>(),
            ExchangeTimezoneService = serviceProvider.GetRequiredService<IMongoService<ExchangeTimezone>>(),
            MorningStarStocksService = serviceProvider.GetRequiredService<IMongoService<MorningStarStocks>>(),
            MorningStarEtfsService = serviceProvider.GetRequiredService<IMongoService<MorningStarEtfs>>(),
            VenueMappingService = serviceProvider.GetRequiredService<IMongoService<GeVenueMapping>>(),
            MarketScheduleService = serviceProvider.GetRequiredService<IMongoService<MarketSchedule>>(),
            TimescaleService = serviceProvider.GetRequiredService<ITimescaleService<RealtimeMarketData>>(),
            CacheService = serviceProvider.GetRequiredService<ICacheService>()
        });

        var rankingEnable = configuration.GetValue(ConfigurationKeys.PreCacheRankingEnable, true);
        if (rankingEnable)
        {
            services.AddHostedService<RankingCacheService>();
        }
        else
        {
            services.AddHostedService<PreCacheService>();
        }

        return services;
    }
}