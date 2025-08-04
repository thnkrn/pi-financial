using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Pi.Client.GlobalMarketData.Api;
using Pi.Client.NotificationService.Api;
using Pi.Client.Sirius.Api;
using Pi.Client.UserService.Api;
using Pi.Client.UserSrvV2.Api;
using Pi.Common.HealthCheck;
using Pi.GlobalEquities.Repositories;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Services;
using Pi.GlobalEquities.Services.Configs;
using Pi.GlobalEquities.Services.Velexa;
using Pi.GlobalEquities.Services.Wallet;
using Pi.GlobalEquities.Worker.Configs;
using Pi.GlobalEquities.Worker.ExternalServices.Notification;
using Pi.GlobalEquities.Worker.Services;
using FeatureService = Pi.GlobalEquities.Worker.ExternalServices.FeatureFlags.FeatureService;
using IFeatureService = Pi.GlobalEquities.Worker.ExternalServices.FeatureFlags.IFeatureService;

namespace Pi.GlobalEquities.Worker.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        services.Configure<MongoDbConfig>(configuration.GetSection(MongoDbConfig.Name));
        services.Configure<VelexaApiConfig>(configuration.GetSection("VelexaApi"));
        services.Configure<NotificationApiConfig>(configuration.GetSection(NotificationApiConfig.Name));
        services.Configure<FeatureApiConfig>(configuration.GetSection(FeatureApiConfig.Name));

        services.AddSingleton<IMarketDataCache, MarketDataCache>();

        MongoDbConfig.IgnoreExtraElements();
        services.AddSingleton<IMongoClient, MongoClient>(serviceProvider =>
        {
            var mongoConfig = configuration.GetSection(MongoDbConfig.Name).Get<MongoDbConfig>();
            var settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConfig.ConnectionString));
            settings.ConnectTimeout = mongoConfig.Timeout;
            return new MongoClient(settings);
        });

        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddSingleton<IAccountService, AccountService>();
        services.AddTransient<DbInitializeService>();
        services.AddSingleton<IWorkerOrderRepository, WorkerOrderRepository>();
        services.AddSingleton<IWorkerAccountRepository, WorkerAccountRepository>();
        services.AddSingleton<IWorkerJobRepository, WorkerJobRepository>();

        AddWalletService(services, configuration);
        AddUserService(services, configuration);
        AddFeatureSwitchConfig(services, configuration);
        AddOrderReferenceConfig(services, configuration);
        AddMarketData(services, configuration);
        AddUserServiceV2(services, configuration);

        services.AddExternalServices(configuration);
        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<INotificationService, NotificationService>();

        services.AddHttpClient<INotificationApi, NotificationApi>(client =>
        {
            var notificationConfig = configuration.GetSection(NotificationApiConfig.Name).Get<NotificationApiConfig>();
            client.Timeout = notificationConfig.Timeout;
            return new NotificationApi(client, notificationConfig.Url);
        });

        services.AddVelexaServices(configuration);
        services.AddHostedService<SyncOrderStateService>();
        services.AddHostedService<DbMigrationService>();

        return services;
    }

    private static IServiceCollection AddVelexaServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<VelexaClient>();
        services.AddSingleton<ITradingReadService, VelexaTradingReadService>();
        services.AddHttpClient<VelexaClient>(client =>
        {
            var velexaConfig = configuration.GetSection("VelexaApi").Get<VelexaApiConfig>();

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", velexaConfig.Token);
            client.BaseAddress = new Uri(velexaConfig.Url);
            client.Timeout = velexaConfig.Timeout;
        });

        return services;
    }

    private static IServiceCollection AddWalletService(IServiceCollection services, IConfiguration config)
    {
        var walletConfig = config.GetSection("WalletApi").Get<WalletApiConfig>();

        services.AddHttpClient<IWalletService, WalletService>(client =>
        {
            client.Timeout = walletConfig.Timeout;
            client.BaseAddress = new Uri(walletConfig.Url);
        });

        return services;
    }

    private static IServiceCollection AddUserService(IServiceCollection services, IConfiguration config)
    {
        var userConfig = config.GetSection("UserApi").Get<UserApiConfig>();
        services.AddHttpClient<IUserMigrationApi, UserMigrationApi>(client =>
        {
            client.Timeout = userConfig.Timeout;
            return new UserMigrationApi(client, userConfig.Url);
        });

        return services;
    }

    private static IServiceCollection AddUserServiceV2(IServiceCollection services, IConfiguration config)
    {
        var userV2Config = config.GetSection("UserApiV2").Get<UserV2ApiConfig>();
        services.AddHttpClient<ITradingAccountApi, TradingAccountApi>(client =>
        {
            client.Timeout = userV2Config.Timeout;
            return new TradingAccountApi(client, userV2Config.Url);
        });

        return services;
    }

    private static IServiceCollection AddFeatureSwitchConfig(IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IFeatureService, FeatureService>();
        services.AddHttpClient<IFeatureService, FeatureService>();

        var featureSwitchConfig = config.GetSection("GrowthBook").Get<FeatureSwitchConfig>();
        services
            .AddHttpClient("GrowthBook")
            .ConfigureHttpClient(
                (_, client) =>
                {
                    client.BaseAddress = new Uri(
                        featureSwitchConfig.Host
                    );
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            );

        services.AddHttpContextAccessor();
        services.AddSingleton<Pi.Common.Features.IFeatureService>(sp =>
        {
            var context = sp.GetRequiredService<IHttpContextAccessor>();
            var headers = context
                .HttpContext
                ?.Request
                .Headers
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.ToArray(),
                    StringComparer.OrdinalIgnoreCase
                );

            return new Pi.Common.Features.FeatureService(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrowthBook"),
                sp.GetRequiredService<ILogger<Pi.Common.Features.FeatureService>>(),
                featureSwitchConfig.ApiKey ?? string.Empty,
                featureSwitchConfig.ProjectId ?? string.Empty,
                attributes: headers != null
                    ? Pi.Common.Features.FeatureService.GetAttributes(headers!)
                    : new Dictionary<string, string>()
            );
        });

        return services;
    }


    private static IServiceCollection AddMarketData(IServiceCollection services, IConfiguration config)
    {
        var marketDataConfig = config.GetSection("PiMarketDataApi").Get<PiMarketDataApiConfig>();
        services.AddHttpClient<IMarketDataApi, MarketDataApi>(client =>
        {
            client.Timeout = marketDataConfig.Timeout;
            return new MarketDataApi(client, marketDataConfig.Url);
        });

        var siriusMarketDataConfig = config.GetSection("SiriusApi").Get<SiriusMarketDataApiConfig>();
        services.AddHttpClient<ISiriusApi, SiriusApi>(client =>
        {
            client.Timeout = siriusMarketDataConfig.Timeout;
            return new SiriusApi(client, siriusMarketDataConfig.Url);
        });

        return services;
    }

    private static IServiceCollection AddOrderReferenceConfig(IServiceCollection services, IConfiguration config)
    {
        var rsaPublicKey = config["orderSignatureValidationKey"];
        services.AddSingleton<IOrderReferenceValidator>(new OrderReferenceValidator(rsaPublicKey));

        return services;
    }
}
