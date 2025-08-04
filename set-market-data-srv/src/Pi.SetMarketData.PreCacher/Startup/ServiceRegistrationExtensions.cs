using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.SetMarketData.Application.Interfaces.Holiday;
using Pi.SetMarketData.Application.Queries.Holiday;
using Pi.SetMarketData.Domain.AggregatesModels.HolidayAggregate;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Infrastructure.Handlers.MarketTicker;
using Pi.SetMarketData.Infrastructure.Interfaces.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Repositories;
using Pi.SetMarketData.Infrastructure.Services.EntityCacheService;
using Pi.SetMarketData.Infrastructure.Services.Helpers;
using Pi.SetMarketData.PreCacher.Services;

namespace Pi.SetMarketData.PreCacher.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Dependencies of Controllers
        services.AddScoped<IInstrumentHelper, InstrumentHelper>();
        services.AddSingleton<EntityCacheServiceDependencies>();
        services.AddSingleton<IEntityCacheService, EntityCacheService>();
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

        var rankingEnable = configuration.GetValue(ConfigurationKeys.PreCacheRankingEnable, true);
        if (rankingEnable)
            services.AddHostedService<RankingCacheService>();
        else
            services.AddHostedService<PreCacheService>();

        return services;
    }
}