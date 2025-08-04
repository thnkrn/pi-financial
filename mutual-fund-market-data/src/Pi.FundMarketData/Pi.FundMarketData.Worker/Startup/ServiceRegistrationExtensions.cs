using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Pi.Common.HealthCheck;
using Pi.FundMarketData.Repositories;
using Pi.FundMarketData.Repositories.SqlDatabase;
using Pi.FundMarketData.Worker.Services;
using Pi.FundMarketData.Worker.Services.FundMarket.FundConnext;
using Pi.FundMarketData.Worker.Services.FundMarket.Morningstar;

namespace Pi.FundMarketData.Worker.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddHostedService<SyncFundProfileService>();

        services.Configure<FundConnextOptions>(configuration.GetSection(FundConnextOptions.Name));
        services.Configure<MorningstarOptions>(configuration.GetSection(MorningstarOptions.Name));
        services.Configure<DataCacheConfig>(configuration.GetSection("DataCacheConfig"));
        services.Configure<MongoDbConfig>(configuration.GetSection("FundMarketDatabase"));

        MongoDbConfig.IgnoreExtraElements();
        var connectionString = configuration.GetSection("FundMarketDatabase").GetSection("ConnectionString").Value;
        connectionString += "?maxPoolSize=20";
        services.AddSingleton<IMongoClient, MongoClient>(serviceProvider =>
            new MongoClient(connectionString));
        services.AddTransient<DbInitializeService>();
        services.AddSingleton<IFundRepository, FundRepository>();
        services.AddSingleton<IFundWebRepository, FundWebRepository>();
        services.AddSingleton<ITradeDataRepository, TradeDataRepository>();
        services.AddSingleton<IFundHistoricalNavRepository, FundHistoricalNavRepository>();
        services.AddSingleton<ICommonDataRepository, CommonDataRepository>();
        services.AddSingleton<IHolidayRepository, HolidayRepository>();
        services.AddSingleton<ICache, MemCache>();

        services.AddHttpClient<IFundConnextService, FundConnextService>(client =>
        {
            var options = configuration
                .GetSection(FundConnextOptions.Name)
                .Get<FundConnextOptions>()!;
            client.BaseAddress = new Uri(options.ServerUrl);
        });

        services.AddHttpClient<IMorningstarService, MorningstarService>(client =>
        {
            var options = configuration
                .GetSection(MorningstarOptions.Name)
                .Get<MorningstarOptions>()!;
            client.BaseAddress = new Uri(options.ServerUrl);
        });

        return services;
    }
}
