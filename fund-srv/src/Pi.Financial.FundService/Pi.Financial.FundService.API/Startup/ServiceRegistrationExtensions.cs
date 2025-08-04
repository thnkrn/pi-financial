using Microsoft.Extensions.Options;
using Pi.Client.FundMarketData.Api;
using Pi.Client.OnboardService.Api;
using Pi.Client.UserService.Api;
using Pi.Common.Database.Repositories;
using Pi.Common.Domain.AggregatesModel.BankAggregate;
using Pi.Common.Features;
using Pi.Common.Utilities;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.FundConnext.Api;
using Pi.Financial.Client.PiItBackoffice.Api;
using Pi.Financial.FundService.Application.Models.Metric;
using Pi.Financial.FundService.Application.Queries;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.ItBackofficeService;
using Pi.Financial.FundService.Application.Services.MarketService;
using Pi.Financial.FundService.Application.Services.Metric;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Infrastructure.Options;
using Pi.Financial.FundService.Infrastructure.Repositories;
using Pi.Financial.FundService.Infrastructure.Services;
using StackExchange.Redis;
using DocumentApi = Pi.Client.UserService.Api.DocumentApi;
using IDocumentApi = Pi.Client.UserService.Api.IDocumentApi;

namespace Pi.Financial.FundService.API.Startup
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
                    options.ConnectionMultiplexerFactory = async () => await ConnectionMultiplexer.ConnectAsync(options.ConfigurationOptions);
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddSingleton<DateTimeProvider>();
            services.AddSingleton<IMetrics, Metrics>();
            services.AddSingleton<IMetricService, MetricService>();
            services.AddScoped<ICustomerService, FreewillCustomerService>();
            services.AddScoped<IOnboardService, Infrastructure.Services.OnboardService>();
            services.AddScoped<IMarketService, MarketService>();
            services.AddScoped<ICustomerModuleApi>(sp =>
                new CustomerModuleApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("Freewill"),
                    configuration.GetValue<string>("Freewill:Host")!));
            services.AddScoped<INdidQueries, NdidQueries>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IBankInfoRepository, BankInfoRepository>();
            services.AddScoped<IFundQueries, FundQueries>();
            services.AddScoped<FundConnextService>();
            services.AddScoped<IFundConnextService, MaintenanceFundConnextService>();
            services.AddScoped<IFundAccountOpeningStateQueries, FundAccountOpeningStateQueries>();
            services.AddScoped<IFundAccountOpeningStateRepository, FundAccountOpeningStateDbRepository>();
            services.AddScoped<IFundOrderRepository, FundOrderRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IItBackofficeService, ItBackofficeService>();

            services.AddOptions<UserServiceOptions>()
                .Bind(configuration.GetSection(UserServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<OnboardServiceOptions>()
                .Bind(configuration.GetSection(OnboardServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<MarketServiceOptions>()
                .Bind(configuration.GetSection(MarketServiceOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddOptions<ItBackofficeOptions>()
                .Bind(configuration.GetSection(ItBackofficeOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<IBackOfficeApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ItBackofficeOptions>>();
                return new BackOfficeApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ItBackoffice"), options.Value.Host);
            });
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
            services.AddScoped<IOpenAccountApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new OpenAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
            });
            services.AddScoped<ICrsApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new CrsApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
            });
            services.AddScoped<IBankAccountApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new BankAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
            });
            services.AddScoped<Pi.Client.OnboardService.Api.IUserDocumentApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new Pi.Client.OnboardService.Api.UserDocumentApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
            });
            services.AddScoped<IFundConnextApi>((sp) =>
                new FundConnextApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("FundConnext"),
                    configuration.GetValue<string>("FundConnext:Host")));
            services.AddScoped<IAccountOpeningV5Api>((sp) =>
                new AccountOpeningV5Api(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("FundConnext"),
                    configuration.GetValue<string>("FundConnext:Host")));
            services.AddScoped<IUserApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
                return new UserApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), options.Value.Host);
            });
            services.AddScoped<IUserMigrationApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
                return new UserMigrationApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), options.Value.Host);
            });
            services.AddScoped<ITradingAccountApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new TradingAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
            });
            services.AddScoped<IFundApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MarketServiceOptions>>();
                return new FundApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Market"), options.Value.Host);
            });

            services.AddOptions<DatabaseOptions>()
                .Bind(configuration.GetSection(DatabaseOptions.Options))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<ICustomerDataSyncHistoryRepository, CustomerDataSyncHistoryDbRepository>();
            services.AddScoped<IDocumentApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
                return new DocumentApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), options.Value.Host);
            });

            services.AddScoped<IDopaApi>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
                return new DopaApi(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
            });

            services.AddScoped<Pi.Client.UserSrvV2.Api.IUserApi>(sp =>
                new Pi.Client.UserSrvV2.Api.UserApi(
                    sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient("UserServiceV2"),
                    configuration.GetValue<string>("UserV2:Host") ?? string.Empty)
            );
            services.AddScoped<Pi.Client.UserSrvV2.Api.IBankAccountApi>(sp =>
                new Pi.Client.UserSrvV2.Api.BankAccountApi(
                    sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient("UserServiceV2"),
                    configuration.GetValue<string>("UserV2:Host") ?? string.Empty)
            );

            return services;
        }
    }
}
