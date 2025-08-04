using Pi.SetMarketData.API.Infrastructure.Services;
using Pi.SetMarketData.Application.Abstractions;
using Pi.SetMarketData.Application.Interfaces.Holiday;
using Pi.SetMarketData.Application.Mediator;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Queries.Holiday;
using Pi.SetMarketData.Domain.AggregatesModels.HolidayAggregate;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Handlers.MarketTicker;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Utils;
using Pi.SetMarketData.Infrastructure.Queries;
using Pi.SetMarketData.Infrastructure.Repositories;
using Pi.SetMarketData.Infrastructure.Services.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Services.Helpers;
using Pi.SetMarketData.Infrastructure.Utils;

namespace Pi.SetMarketData.API.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        // Dependencies of Controllers
        services.AddScoped<IControllerRequestHelper, ControllerRequestHelper>();
        services.AddScoped<IInstrumentHelper, InstrumentHelper>();
        services.AddScoped<IInstrumentQuery, InstrumentQuery>();
        services.AddSingleton<EntityCacheServiceDependencies>();
        services.AddSingleton<IEntityCacheService, EntityCacheService>();

        // MassTransitRequestBus
        services.AddScoped<IRequestBus, MassTransitRequestBus>();
        services.AddScoped<MongoDbServices>(provider =>
        {
            var exchangeTimezone = provider.GetRequiredService<IMongoService<ExchangeTimezone>>();
            var instrumentDetailService = provider.GetRequiredService<IMongoService<InstrumentDetail>>();
            var instrumentService = provider.GetRequiredService<IMongoService<Instrument>>();
            var morningstarStocksService = provider.GetRequiredService<IMongoService<MorningStarStocks>>();

            return new MongoDbServices(exchangeTimezone, instrumentDetailService, instrumentService,
                morningstarStocksService);
        });

        // Register IHolidayApiRepository and HolidayApiRepository
        services.AddSingleton<IHolidayApiRepository, HolidayApiRepository>();

        // Register IDateTimeProvider and SystemDateTimeProvider
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // Register IHolidayQuery and HolidayQuery
        services.AddSingleton<IHolidayApiQuery, HolidayApiQuery>();

        services.AddScoped<IFileUtils, FileUtils>();

        return services;
    }
}
