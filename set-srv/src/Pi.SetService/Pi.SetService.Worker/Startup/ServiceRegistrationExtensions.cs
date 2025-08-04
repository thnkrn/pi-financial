using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Pi.Client.BondApi.Api;
using Pi.Client.NotificationService.Api;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnePort.GW.DB2.Api;
using Pi.Client.PiInternal.Api;
using Pi.Client.UserService.Api;
using Pi.Common.Features;
using Pi.Common.Generators.Number;
using Pi.Common.HealthCheck;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Options;
using Pi.SetService.Application.Queries;
using Pi.SetService.Application.Services.FeatureService;
using Pi.SetService.Application.Services.MarketService;
using Pi.SetService.Application.Services.NotificationService;
using Pi.SetService.Application.Services.NumberGeneratorService;
using Pi.SetService.Application.Services.OnboardService;
using Pi.SetService.Application.Services.OneportService;
using Pi.SetService.Application.Services.PiInternalService;
using Pi.SetService.Application.Services.SbaService;
using Pi.SetService.Application.Services.SblService;
using Pi.SetService.Application.Services.UserService;
using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Infrastructure.Handlers;
using Pi.SetService.Infrastructure.Options;
using Pi.SetService.Infrastructure.Repositories;
using Pi.SetService.Infrastructure.Services;
using IDb2TradingApi = Pi.Client.OnePort.GW.DB2.Api.ITradingApi;
using Db2TradingApi = Pi.Client.OnePort.GW.DB2.Api.TradingApi;
using ITcpTradingApi = Pi.Client.OnePort.GW.TCP.Api.ITradingApi;
using TcpTradingApi = Pi.Client.OnePort.GW.TCP.Api.TradingApi;

namespace Pi.SetService.Worker.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddScoped<HttpMessageLoggingHandler>();
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddScoped<ISetQueries, SetQueries>();
        services.AddScoped<IOnboardService, OnboardService>();
        services.AddScoped<IMarketService, MarketService>();
        services.AddScoped<OnePortService>();
        services.AddScoped<MaintenanceOnePortService>();
        services.AddScoped<IOnePortService>(q =>
        {
            var featureService = q.GetRequiredService<IFeatureService>();
            if (featureService.IsOn(Features.OnePortMaintenance))
            {
                return q.GetRequiredService<MaintenanceOnePortService>();
            }

            return q.GetRequiredService<OnePortService>();
        });
        services.AddScoped<IEquityNumberGeneratorService, EquityNumberGeneratorService>();
        services.AddScoped<INumberGeneratorRepository, NumberGeneratorRepository>();
        services.AddScoped<IEquityOrderStateRepository, EquityOrderStateRepository>();
        services.AddScoped<IInstrumentRepository, InstrumentRepository>();
        services.AddScoped<ISblOrderRepository, SblOrderRepository>();
        services.AddScoped<ISbaService, SbaService>();
        services.AddScoped<ISblService, SblService>();
        services.AddScoped<IPiInternalService, PiInternalService>();
        services.AddScoped<INotificationService, NotificationService>();
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
        services.AddScoped<OnboardService>();
        services.AddScoped<IOnboardService>(q =>
        {
            var featureService = q.GetRequiredService<IFeatureService>();
            if (featureService.IsOn(Features.OnboardMigration))
            {
                return q.GetRequiredService<UserServiceV2>();
            }

            return q.GetRequiredService<OnboardService>();
        });

        if (environment.IsDevelopment())
        {
            var region = configuration.GetValue<string>("AWS:Region");
            var accessKey = configuration.GetValue<string>("AWS:AccessKey");
            var secretKey = configuration.GetValue<string>("AWS:SecretKey");
            var serviceUrl = configuration.GetValue<string>("AWS:ServiceUrl");
            services.AddScoped<IAmazonS3, AmazonS3Client>(_ =>
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(region),
                    ServiceURL = serviceUrl,
                    ForcePathStyle = true
                };
                var credentials = new BasicAWSCredentials(accessKey, secretKey);

                {
                    config.ServiceURL = serviceUrl;
                    config.ForcePathStyle = true;
                }

                return new AmazonS3Client(credentials, config);
            });
        }
        else
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();
        }

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
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<OnePortTcpServiceOptions>()
            .Bind(configuration.GetSection(OnePortTcpServiceOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<OnePortDb2ServiceOptions>()
            .Bind(configuration.GetSection(OnePortDb2ServiceOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<PiInternalServiceOptions>()
            .Bind(configuration.GetSection(PiInternalServiceOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<SetTradingOptions>()
            .Bind(configuration.GetSection(SetTradingOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<NotificationServiceOptions>()
            .Bind(configuration.GetSection(NotificationServiceOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<NotificationIconOptions>()
            .Bind(configuration.GetSection(NotificationIconOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<BondMarketServiceOptions>()
            .Bind(configuration.GetSection(BondMarketServiceOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddOptions<UserServiceV2Options>()
            .Bind(configuration.GetSection(UserServiceV2Options.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<ITradingAccountApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OnboardServiceOptions>>();
            return new TradingAccountApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Onboard"), options.Value.Host);
        });
        services.AddScoped<IUserApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<UserServiceOptions>>();
            return new UserApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("User"), options.Value.Host);
        });
        services.AddScoped<ITcpTradingApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OnePortTcpServiceOptions>>();
            return new TcpTradingApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("OnePortTcp"), options.Value.Host);
        });
        services.AddScoped<IDb2TradingApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OnePortDb2ServiceOptions>>();
            return new Db2TradingApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("OnePortDb2"), options.Value.Host);
        });
        services.AddScoped<IAccountApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OnePortDb2ServiceOptions>>();
            return new AccountApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("OnePort"), options.Value.Host);
        });
        services.AddScoped<IBackOfficeApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PiInternalServiceOptions>>();
            return new BackOfficeApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("PiInternal"), options.Value.Host);
        });
        services.AddScoped<INotificationApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<NotificationServiceOptions>>();
            return new NotificationApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Notification"), options.Value.Host);
        });
        services.AddScoped<IMarketDataApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BondMarketServiceOptions>>();
            return new MarketDataApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("Bond"), options.Value.Host);
        });
        services.AddScoped<Client.UserSrvV2.Api.IUserApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<UserServiceV2Options>>();
            return new Client.UserSrvV2.Api.UserApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserServiceV2"), options.Value.Host);
        });
        services.AddScoped<Client.UserSrvV2.Api.ITradingAccountApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<UserServiceV2Options>>();
            return new Client.UserSrvV2.Api.TradingAccountApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserServiceV2"), options.Value.Host);
        });
        services.AddScoped<Client.SetMarketData.Api.IMarketDataApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<MarketServiceOptions>>();
            return new Client.SetMarketData.Api.MarketDataApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("MarketData"), options.Value.Host);
        });
        services.AddScoped<IFeatureService>(sp =>
        {
            return new FeatureService(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrowthBook"),
                sp.GetRequiredService<ILogger<FeatureService>>(),
                configuration.GetValue<string>("GrowthBook:ApiKey") ?? string.Empty,
                configuration.GetValue<string>("GrowthBook:ProjectId") ?? string.Empty,
                attributes: new Dictionary<string, string>()
            );
        });
        services.AddScoped<ICustomFeatureService>(sp =>
        {
            return new CustomFeatureService(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("GrowthBook"),
                sp.GetRequiredService<ILogger<CustomFeatureService>>(),
                configuration.GetValue<string>("GrowthBook:ApiKey") ?? string.Empty,
                configuration.GetValue<string>("GrowthBook:ProjectId") ?? string.Empty,
                attributes: new Dictionary<string, string>()
            );
        });

        return services;
    }
}
