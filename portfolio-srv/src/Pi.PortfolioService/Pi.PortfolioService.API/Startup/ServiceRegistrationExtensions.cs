using Pi.Client.FundService.Api;
using Pi.Client.GlobalEquities.Api;
using Pi.Client.SetService.Api;
using Pi.Client.Sirius.Api;
using Pi.Client.StructureNotes.Api;
using Pi.Client.UserService.Api;
using Pi.Common.Features;
using Pi.PortfolioService.API.Services;
using Pi.PortfolioService.Application.Models;
using Pi.PortfolioService.DomainServices;
using Pi.PortfolioService.Services.Options;
using Pi.PortfolioService.Services.Set;
using StackExchange.Redis;

namespace Pi.PortfolioService.API.Startup
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddServices(
            this IServiceCollection services,
            ConfigurationManager configuration
        )
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
                    options.ConnectionMultiplexerFactory = async () => await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions);
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddScoped<IPortfolioSummaryQueries, PortfolioManager>();
            services.AddScoped<ISiriusService, SiriusService>();
            services.AddScoped<IStructureNoteService, StructureNoteService>();
            services.AddScoped<IFundService, FundService>();
            services.AddScoped<ISetService, SetService>();
            services.AddScoped<IGeService, GeService>();
            services.AddScoped<ITfexService, TfexService>();
            services.AddScoped<UserService>();
            services.AddScoped<UserServiceV2>();
            services.AddScoped<IUserService>(q =>
            {
                var featureService = q.GetRequiredService<IFeatureService>();
                if (featureService.IsOn(Features.UserV2Migration))
                {
                    return q.GetRequiredService<UserServiceV2>();
                }

                return q.GetRequiredService<UserService>();
            });
            services.AddOptions<CacheOptions>()
                .Bind(configuration.GetSection(CacheOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<ISiriusApi>(
                sp =>
                    new SiriusApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("Sirius"),
                        configuration.GetValue<string>("Sirius:Host") ?? string.Empty
                    )
            );

            services.AddScoped<INoteApi>(
                sp =>
                    new NoteApi(
                        sp.GetRequiredService<IHttpClientFactory>()
                            .CreateClient("StructureNoteAPI"),
                        configuration.GetValue<string>("StructureNote:Host") ?? string.Empty
                    )
            );

            services.AddScoped<Client.TfexService.Api.IAccountApi>(
                sp =>
                    new Client.TfexService.Api.AccountApi(
                        sp.GetRequiredService<IHttpClientFactory>()
                            .CreateClient("Tfex"),
                        configuration.GetValue<string>("Tfex:Host") ?? string.Empty
                    )
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
                    configuration.GetValue<string>("GrowthBook:ApiKey") ?? string.Empty,
                    configuration.GetValue<string>("GrowthBook:ProjectId") ?? string.Empty,
                    attributes: headers != null
                        ? FeatureService.GetAttributes(headers!)
                        : new Dictionary<string, string>()
                );
            });
            services.AddScoped<IFundTradingApi>(
                sp =>
                    new FundTradingApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("Fund"),
                        configuration.GetValue<string>("Fund:Host") ?? string.Empty
                    )
            );

            services.AddScoped<ISetTradingApi>(
                sp =>
                    new SetTradingApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("Set"),
                        configuration.GetValue<string>("Set:Host") ?? string.Empty
                    )
            );

            services.AddScoped<IFeatureService>(sp =>
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

            services.AddScoped<IAccountApi>(
                sp =>
                    new AccountApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("GlobalEquities"),
                        configuration.GetValue<string>("GlobalEquities:Host") ?? string.Empty
                    )
            );

            services.AddScoped<IUserTradingAccountApi>(
                sp =>
                    new UserTradingAccountApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"),
                        configuration.GetValue<string>("User:Host") ?? string.Empty
                    )
            );

            services.AddScoped<IUserApi>(
                sp =>
                    new UserApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"),
                        configuration.GetValue<string>("User:Host") ?? string.Empty
                    )
            );

            services.AddScoped<IUserMigrationApi>(
                sp =>
                    new UserMigrationApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"),
                        configuration.GetValue<string>("User:Host") ?? string.Empty
                    )
            );
            services.AddScoped<Client.UserSrvV2.Api.ITradingAccountApi>(
                sp =>
                    new Client.UserSrvV2.Api.TradingAccountApi(
                        sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserV2"),
                        configuration.GetValue<string>("UserV2:Host") ?? string.Empty
                    )
            );

            var bondApiUrl = configuration.GetValue<string>("Bond:Host") ?? string.Empty;
            services.AddHttpClient<IBondService, BondService>(client =>
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(bondApiUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            });

            return services;
        }
    }
}
