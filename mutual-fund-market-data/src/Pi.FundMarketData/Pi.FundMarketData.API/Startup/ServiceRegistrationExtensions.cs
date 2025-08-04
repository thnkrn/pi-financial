using MongoDB.Driver;
using Pi.FundMarketData.API.Configs;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.Repositories;

namespace Pi.FundMarketData.API.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<MongoDbConfig>(configuration.GetSection("FundMarketDatabase"));
        services.Configure<DataCacheConfig>(configuration.GetSection("DataCacheConfig"));
        services.Configure<SecMarketOptions>(configuration.GetSection(SecMarketOptions.Name));
        services.Configure<PiResourceOptions>(configuration.GetSection(PiResourceOptions.Name));
        services.Configure<PiConfig>(configuration.GetSection(PiConfig.Name));

        var piResource = configuration
            .GetSection(PiResourceOptions.Name)
            .Get<PiResourceOptions>()!;
        var secMarket = configuration
            .GetSection(SecMarketOptions.Name)
            .Get<SecMarketOptions>()!;
        StaticUrl.Init(secMarket.FactSheetServerUrl, piResource.AmcLogoCdnServerUrl);
        var piConfig = configuration
            .GetSection(PiConfig.Name)
            .Get<PiConfig>()!;
        StaticConfig.Init(piConfig.CutOffTimeDeduction);

        MongoDbConfig.IgnoreExtraElements();
        string connectionString = configuration.GetSection("FundMarketDatabase").GetSection("ConnectionString").Value;
        services.AddSingleton<IMongoClient, MongoClient>(serviceProvider =>
            new MongoClient(connectionString));

        services.AddSingleton<IFundRepository, FundRepository>();
        services.AddSingleton<IFundWebRepository, FundWebRepository>();
        services.AddSingleton<ITradeDataRepository, TradeDataRepository>();
        services.AddSingleton<IAmcRepository, AmcRepository>();
        services.AddSingleton<IFundHistoricalNavRepository, FundHistoricalNavRepository>();
        services.AddSingleton<ICommonDataRepository, CommonDataRepository>();
        services.AddSingleton<ICache, MemCache>();

        return services;
    }
}
