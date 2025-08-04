using Microsoft.EntityFrameworkCore.DataEncryption;
using Pi.Client.ItBackOffice.Api;
using Pi.Client.Sirius.Api;
using Pi.Client.UserService.Api;
using Pi.Client.UserSrvV2.Api;
using Pi.Client.WalletService.Api;
using Pi.Common.Features;
using Pi.Financial.Client.SetTradeOms.Api;
using Pi.TfexService.API.Filters;
using Pi.TfexService.Application.Options;
using Pi.TfexService.Application.Providers;
using Pi.TfexService.Application.Queries.Account;
using Pi.TfexService.Application.Queries.Margin;
using Pi.TfexService.Application.Queries.Market;
using Pi.TfexService.Application.Queries.Order;
using Pi.TfexService.Application.Services.It;
using Pi.TfexService.Application.Services.MarketData;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Application.Services.UserService;
using Pi.TfexService.Application.Services.Wallet;
using Pi.TfexService.Domain.Models.ActivitiesLog;
using Pi.TfexService.Domain.Models.InitialMargin;
using Pi.TfexService.Infrastructure.Options;
using Pi.TfexService.Infrastructure.Repositories;
using Pi.TfexService.Infrastructure.Services;
using StackExchange.Redis;
using ItOptions = Pi.TfexService.Infrastructure.Options.ItOptions;
using IUserApi = Pi.Client.UserService.Api.IUserApi;
using SetTradeOptions = Pi.TfexService.Infrastructure.Options.SetTradeOptions;
using UserApi = Pi.Client.UserService.Api.UserApi;

namespace Pi.TfexService.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services,
            ConfigurationManager configuration)
        {
            if (configuration.GetValue<bool>("Redis:Enabled"))
            {
                // Add cache service
                services.AddStackExchangeRedisCache(options =>
                {
                    options.ConfigurationOptions = new ConfigurationOptions
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
                    options.ConnectionMultiplexerFactory = async () =>
                    {
                        var connection = await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions);
                        return connection;
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            // Queries
            services.AddScoped<ISetTradeOrderQueries, SetTradeOrderQueries>();
            services.AddScoped<ISetTradeAccountQueries, SetTradeAccountQueries>();
            services.AddScoped<IInitialMarginQueries, InitialMarginQueries>();
            services.AddScoped<IItOrderTradeQueries, ItOrderTradeQueries>();
            services.AddScoped<IMarketDataQueries, MarketDataQueries>();

            // Services
            services.AddScoped<ISetTradeService, SetTradeService>();
            services.AddScoped<IEncryptionCryptoProvider, EncryptionService>();
            services.AddScoped<IMarketDataService, MarketDataService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserV2Service, UserV2Service>();
            services.AddScoped<IItService, ItService>();
            services.AddScoped<IWalletService, WalletService>();

            // Providers
            services.AddScoped<IDateTimeProvider, DateTimeProvider>();

            // Repositories
            services.AddScoped<IActivitiesLogRepository, ActivitiesLogRepository>();
            services.AddScoped<IInitialMarginRepository, InitialMarginRepository>();

            // APIs
            services.AddScoped<ISetTradeOmsApi>(sp =>
                new SetTradeOmsApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("SetTrade"),
                    configuration.GetValue<string>("SetTrade:Host") ?? string.Empty));
            services.AddScoped<ISiriusApi>(sp =>
                new SiriusApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Sirius"),
                    configuration.GetValue<string>("Sirius:Host") ?? string.Empty));
            services.AddScoped<IMarketDataApi>(sp =>
                new MarketDataApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("MarketData"),
                    configuration.GetValue<string>("MarketData:Host") ?? string.Empty));
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
            services.AddScoped<IBackOfficeApi>(sp =>
                new BackOfficeApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("It"),
                    configuration.GetValue<string>("It:Host") ?? string.Empty));
            services.AddScoped<IWalletApiAsync>(sp =>
                new WalletApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("WalletService"),
                    configuration.GetValue<string>("WalletService:Host") ?? string.Empty));

            // Filters
            services.AddScoped<SecureAuthorizationFilter>();

            // Third Party Service
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
                    attributes: headers != null
                        ? FeatureService.GetAttributes(headers!)
                        : new Dictionary<string, string>()
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
            services.AddOptions<SymbolOptions>()
                .Bind(configuration.GetSection(SymbolOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<ItOptions>()
                .Bind(configuration.GetSection(ItOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}