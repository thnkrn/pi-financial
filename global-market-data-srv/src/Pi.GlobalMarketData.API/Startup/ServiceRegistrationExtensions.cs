using Pi.GlobalMarketData.API.Infrastructure.Services;
using Pi.GlobalMarketData.Application.Abstractions;
using Pi.GlobalMarketData.Application.Mediator;
using Pi.GlobalMarketData.Application.Services.SomeExternal;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Handlers.MarketTicker;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Utils;
using Pi.GlobalMarketData.Infrastructure.Services;
using Pi.GlobalMarketData.Infrastructure.Services.EntityCacheService;
using Pi.GlobalMarketData.Infrastructure.Services.Mongo;
using Pi.GlobalMarketData.Infrastructure.Utils;

namespace Pi.GlobalMarketData.API.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        // Example Init Service
        services.AddScoped<ISomeExternalService, SomeExternalService>();

        // Register IMongoContext, IMongoService, and IMongoRepository
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        // Dependencies of Controllers
        services.AddScoped<IControllerRequestHelper, ControllerRequestHelper>();

        // MassTransitRequestBus
        services.AddScoped<IRequestBus, MassTransitRequestBus>();

        // VelexaHttp
        services.AddSingleton<IVelexaHttpApiJwtTokenGenerator>(
            _ => new VelexaHttpApiJwtTokenGenerator(
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtSecret) ?? "",
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtClientId) ?? "",
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtApptId) ?? ""
            )
        );
        services.AddScoped<IVelexaApiHelper, VelexaApiHelper>();
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

        services.AddScoped<IFileUtils, FileUtils>();
        return services;
    }
}