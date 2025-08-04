using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Pi.Client.NotificationService.Api;
using Pi.Client.Sirius.Api;
using Pi.Common.Features;
using Pi.Client.UserService.Api;
using Pi.Client.UserSrvV2.Api;
using Pi.Common.HealthCheck;
using Pi.Financial.Client.SetTradeOms.Api;
using Pi.TfexService.Application.Queries.Margin;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Services.DistributedLock;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Application.Services.Notification;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Domain.Models.ActivitiesLog;
using Pi.TfexService.Domain.Models.InitialMargin;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Infrastructure.Repositories;
using Pi.TfexService.Infrastructure.Services;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using IUserApi = Pi.Client.UserService.Api.IUserApi;
using UserApi = Pi.Client.UserService.Api.UserApi;

namespace Pi.TfexService.Worker.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("Redis:Enabled"))
            {
                var configurationOptions = new ConfigurationOptions
                {
                    AbortOnConnectFail = configuration.GetValue<bool>("Redis:AbortOnConnectFail"),
                    Ssl = configuration.GetValue<bool>("Redis:Ssl"),
                    ClientName = configuration.GetValue<string>("Redis:ClientName"),
                    ConnectRetry = configuration.GetValue<int>("Redis:ConnectRetry"),
                    ConnectTimeout = configuration.GetValue<int>("Redis:ConnectTimeout"),
                    SyncTimeout = configuration.GetValue<int>("Redis:SyncTimeout"),
                    DefaultDatabase = configuration.GetValue<int>("Redis:Database"),
                    EndPoints =
                    {
                        {
                            configuration.GetValue<string>("Redis:Host")!, configuration.GetValue<int>("Redis:Port")
                        }
                    },
                    User = configuration.GetValue<string>("Redis:Username"),
                    Password = configuration.GetValue<string>("Redis:Password")
                };

                // config redis cache
                var connection = ConnectionMultiplexer.Connect(configurationOptions);
                services.AddSingleton<IConnectionMultiplexer>(connection);
                services.AddStackExchangeRedisCache(options =>
                {
                    options.ConfigurationOptions = configurationOptions;
                    options.ConnectionMultiplexerFactory = async () => await Task.FromResult(connection);
                });

                // config red lock
                var redLockConnection = new List<RedLockMultiplexer> { new(connection) };
                var redLockFactory = RedLockFactory.Create(redLockConnection);
                services.AddSingleton<IDistributedLockFactory>(redLockFactory);
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

            // Services
            services.AddScoped<ISetTradeService, SetTradeService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserV2Service, UserV2Service>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IMarketDataService, MarketDataService>();
            services.AddScoped<IEncryptionCryptoProvider, EncryptionService>();
            services.AddSingleton<IDistributedLockService>(serviceProvider =>
            {
                var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
                var lockFactory = serviceProvider.GetRequiredService<IDistributedLockFactory>();
                var featureOptions = serviceProvider.GetRequiredService<IOptionsMonitor<FeaturesOptions>>();
                return new DistributedLockService(lockFactory, distributedCache, featureOptions);
            });
            services.AddScoped<IInitialMarginQueries, InitialMarginQueries>();
            services.AddScoped<IMarketDataQueries, MarketDataQueries>();

            // Repositories
            services.AddScoped<IActivitiesLogRepository, ActivitiesLogRepository>();
            services.AddScoped<IInitialMarginRepository, InitialMarginRepository>();

            // APIs
            services.AddScoped<ISetTradeOmsApi>(sp =>
                new SetTradeOmsApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SetTrade"),
                    configuration.GetValue<string>("SetTrade:Host") ?? string.Empty));
            services.AddScoped<IUserApi>(sp =>
                new UserApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserService"),
                    configuration.GetValue<string>("UserService:Host") ?? string.Empty));
            services.AddScoped<Client.UserSrvV2.Api.IUserApi>(sp =>
                new Client.UserSrvV2.Api.UserApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserV2"),
                    configuration.GetValue<string>("UserV2:Host") ?? string.Empty));
            services.AddScoped<ITradingAccountApi>(sp =>
                new TradingAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserV2"),
                    configuration.GetValue<string>("UserV2:Host") ?? string.Empty));
            services.AddScoped<IUserMigrationApi>(sp =>
                new UserMigrationApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserService"),
                    configuration.GetValue<string>("UserService:Host") ?? string.Empty));
            services.AddScoped<INotificationApi>(sp =>
                new NotificationApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("NotificationService"),
                    configuration.GetValue<string>("NotificationService:Host") ?? string.Empty));
            services.AddScoped<IMarketDataApi>(sp =>
                new MarketDataApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("MarketData"),
                    configuration.GetValue<string>("MarketData:Host") ?? string.Empty));
            services.AddScoped<ISiriusApi>(sp =>
                new SiriusApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Sirius"),
                    configuration.GetValue<string>("Sirius:Host") ?? string.Empty));
            services.AddScoped<IUserMigrationApi>(sp =>
                new UserMigrationApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserService"),
                    configuration.GetValue<string>("UserService:Host") ?? string.Empty));

            // Third party
            services.AddScoped<IFeatureService>(sp
                =>
            {
                var context = sp.GetRequiredService<IHttpContextAccessor>();
                var headers = context.HttpContext?.Request.Headers.ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value.ToArray(),
                    StringComparer.OrdinalIgnoreCase
                );

                return new FeatureService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrowthBook"),
                    sp.GetRequiredService<ILogger<FeatureService>>(),
                    configuration.GetValue<string>("GrowthBook:ApiKey") ?? string.Empty,
                    configuration.GetValue<string>("GrowthBook:ProjectId") ?? string.Empty,
                    attributes: headers != null ? FeatureService.GetAttributes(headers!) : new Dictionary<string, string>()
                );
            });

            // Options
            services.AddOptions<SetTradeOptions>()
                .Bind(configuration.GetSection(SetTradeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<FeaturesOptions>()
                .Bind(configuration.GetSection(FeaturesOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
