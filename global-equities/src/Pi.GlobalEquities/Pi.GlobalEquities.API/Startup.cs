using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Pi.Client.GlobalMarketData.Api;
using Pi.Client.Sirius.Api;
using Pi.Client.UserService.Api;
using Pi.Client.UserSrvV2.Api;
using Pi.Client.WalletService.Api;
using Pi.Common.Features;
using Pi.Common.HealthCheck;
using Pi.Common.Web.ActionFilters;
using Pi.GlobalEquities.API.Middlewares;
using Pi.GlobalEquities.Application.Commands;
using Pi.GlobalEquities.Application.Queries;
using Pi.GlobalEquities.Application.Queries.Wallet;
using Pi.GlobalEquities.Application.Services.User;
using Pi.GlobalEquities.Application.Services.Velexa;
using Pi.GlobalEquities.Infrastructure.Services.User;
using Pi.GlobalEquities.Infrastructure.Services.Velexa;
using Pi.GlobalEquities.Infrastructure.Services.Wallet.Cache;
using Pi.GlobalEquities.Infrastructures.Services.Wallet.Cache;
using Pi.GlobalEquities.Repositories.Configs;
using Pi.GlobalEquities.Services;
using Pi.GlobalEquities.Services.Configs;
using Pi.GlobalEquities.Services.Options;
using Pi.GlobalEquities.Services.Velexa;
using Pi.GlobalEquities.Services.Wallet;

namespace Pi.GlobalEquities.API;

public class Startup
{
    private IConfiguration _config;
    private IHostEnvironment _env;
    public Startup(IConfiguration config, IHostEnvironment env)
    {
        _config = config;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddMemoryCache();

        MongoDbConfig.IgnoreExtraElements();
        services.AddSingleton<IMarketDataCache, MarketDataCache>();
        services.Configure<MongoDbConfig>(_config.GetSection(MongoDbConfig.Name));

        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddSingleton<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITradingService, VelexaTradingService>();
        services.AddSingleton<IOrderRepository, OrderRepository>();
        services.AddScoped<IMarketDataService, VelexaMarketDataService>();

        services.AddScoped<IAccountQueries, AccountQueries>();
        services.AddSingleton<IWalletQueries, WalletQueries>();

        services
            .AddSingleton<Pi.GlobalEquities.Application.Repositories.IAccountRepository,
                Pi.GlobalEquities.Infrastructure.Repositories.AccountRepository>();
        services
            .AddSingleton<Pi.GlobalEquities.Application.Repositories.IOrderRepository,
                Pi.GlobalEquities.Infrastructure.Repositories.OrderRepository>();

        services.AddScoped<IOrderQueries, OrderQueries>();
        services.AddScoped<IOrderCommands, OrderCommands>();

        services.AddSingleton<IExchangeRateCacheService, ExchangeRateCacheService>();
        services
            .AddSingleton<Pi.GlobalEquities.Application.Services.MarketData.IMarketDataService,
                Pi.GlobalEquities.Infrastructure.Services.MarketData.MarketDataService>();

        AddMongoConfig(services);
        AddWalletService(services);
        AddUserService(services);
        AddVelexaService(services);
        AddMarketData(services);
        AddOrderReferenceConfig(services);
        AddLogoConfig();
        AddFeatureSwitchConfig(services);
        AddUserServiceV2(services);

        services.AddOptions<CacheOptions>()
            .Bind(configuration.GetSection(CacheOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddControllers(options => options.Filters.Add<ApiResponseFilter>());
        services.AddTransient<ProblemDetailsFactory, CustomProblemDetailsFactory>();
    }

    private void AddMongoConfig(IServiceCollection services)
    {
        var mongoConfig = _config.GetSection(MongoDbConfig.Name).Get<MongoDbConfig>();
        var settings = MongoClientSettings.FromUrl(new MongoUrl(mongoConfig.ConnectionString));
        settings.ConnectTimeout = mongoConfig.Timeout;
        services.AddSingleton<IMongoClient, MongoClient>(serviceProvider => new MongoClient(settings));
    }

    private void AddWalletService(IServiceCollection services)
    {
        var walletConfig = _config.GetSection("WalletApi").Get<WalletApiConfig>();
        services.AddHttpClient<IWalletService, WalletService>(client =>
        {
            client.BaseAddress = new Uri(walletConfig.Url);
            client.Timeout = walletConfig.Timeout;
        });

        services
            .AddSingleton<Pi.GlobalEquities.Application.Services.Wallet.IWalletService,
                Pi.GlobalEquities.Infrastructure.Services.Wallet.WalletService>();

        services.AddHttpClient<IWalletApi, WalletApi>(client =>
        {
            client.Timeout = walletConfig.Timeout;
            return new WalletApi(client, walletConfig.Url);
        });
    }

    private void AddUserService(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        var userConfig = _config.GetSection("UserApi").Get<UserApiConfig>();
        services.AddHttpClient<IUserMigrationApi, UserMigrationApi>(client =>
        {
            client.Timeout = userConfig.Timeout;
            return new UserMigrationApi(client, userConfig.Url);
        });
    }

    private void AddUserServiceV2(IServiceCollection services)
    {
        var userV2Config = _config.GetSection("UserApiV2").Get<UserV2ApiConfig>();
        services.AddHttpClient<ITradingAccountApi, TradingAccountApi>(client =>
        {
            client.Timeout = userV2Config.Timeout;
            return new TradingAccountApi(client, userV2Config.Url);
        });
    }

    private void AddVelexaService(IServiceCollection services)
    {
        services.AddSingleton<VelexaClient>();
        var velexaConfig = _config.GetSection("VelexaApi").Get<VelexaApiConfig>();
        services.AddHttpClient<VelexaClient>(client =>
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", velexaConfig.Token);
            client.BaseAddress = new Uri(velexaConfig.Url);
            client.Timeout = velexaConfig.Timeout;
        });

        services.AddHttpClient<IVelexaService, VelexaService>(client =>
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", velexaConfig.Token);
            client.BaseAddress = new Uri(velexaConfig.Url);
            client.Timeout = velexaConfig.Timeout;
        });

        services.AddHttpClient<IVelexaReadService, VelexaReadService>(client =>
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", velexaConfig.Token);
            client.BaseAddress = new Uri(velexaConfig.Url);
            client.Timeout = velexaConfig.Timeout;
        });
    }

    private void AddMarketData(IServiceCollection services)
    {
        var marketDataConfig = _config.GetSection("PiMarketDataApi").Get<PiMarketDataApiConfig>();
        services.AddHttpClient<IMarketDataApi, MarketDataApi>(client =>
        {
            client.Timeout = marketDataConfig.Timeout;
            return new MarketDataApi(client, marketDataConfig.Url);
        });
    }

    private void AddOrderReferenceConfig(IServiceCollection services)
    {
        var rsaPrivateKey = _config["orderSignatureKey"];
        services.AddSingleton<IOrderReferenceIssuer>(x => new OrderReferenceIssuer(rsaPrivateKey));
        services.AddSingleton<Infrastructure.Services.Velexa.OrderReferences.IOrderReferenceIssuer>(x =>
            new Infrastructure.Services.Velexa.OrderReferences.OrderReferenceIssuer(rsaPrivateKey));

        var rsaPublicKey = _config["orderSignatureValidationKey"];
        services.AddSingleton<IOrderReferenceValidator>(x => new OrderReferenceValidator(rsaPublicKey));
        services.AddSingleton<Infrastructure.Services.Velexa.OrderReferences.IOrderReferenceValidator>(x =>
            new Infrastructure.Services.Velexa.OrderReferences.OrderReferenceValidator(rsaPublicKey));
    }

    private void AddLogoConfig()
    {
        var piResource = _config.GetSection(PiResourceOptions.Name).Get<PiResourceOptions>();
        StaticUrl.Init(piResource.StockLogoCdnUrl);
    }

    private void AddFeatureSwitchConfig(IServiceCollection services)
    {
        var featureSwitchConfig = _config.GetSection("GrowthBook").Get<FeatureSwitchConfig>();
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

        services.AddScoped<IFeatureService>(sp =>
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

            return new FeatureService(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrowthBook"),
                sp.GetRequiredService<ILogger<FeatureService>>(),
                featureSwitchConfig.ApiKey ?? string.Empty,
                featureSwitchConfig.ProjectId ?? string.Empty,
                attributes: headers != null
                    ? FeatureService.GetAttributes(headers!)
                    : new Dictionary<string, string>()
            );
        });
    }

    public void ConfigureApp(WebApplication app)
    {
        app.UseMiddleware<LogEnricher>();
        app.UseMiddleware<RequestBodyLogger>();
        app.UseExceptionHandler();

        if (!app.Environment.IsProduction())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapHealthChecks("/");
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
