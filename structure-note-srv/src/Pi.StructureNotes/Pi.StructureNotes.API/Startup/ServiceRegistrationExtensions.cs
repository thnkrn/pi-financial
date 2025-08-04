using Pi.Client.GlobalMarketData.Api;
using Pi.Client.OnboardService.Api;
using Pi.Client.Sirius.Api;
using Pi.Client.UserService.Api;
using Pi.Common.Features;
using Pi.StructureNotes.Infrastructure;

namespace Pi.StructureNotes.API.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddMemoryCache();

        services.Configure<DataCacheConfig>(configuration.GetSection("DataCacheConfig"));
        services.AddSingleton<IUserDataCache, UserDataCache>();
        services.AddSingleton<IMarketDataCache, MarketDataCache>();

        services.AddScoped<IAccountService, AccountService>();

        HttpClient exClient = new HttpClient
        {
            BaseAddress = new Uri(configuration.GetSection("ExanteClientConfig:Url").Value)
        };

        exClient.DefaultRequestHeaders.Add("Authorization",
            configuration.GetSection("ExanteClientConfig:JwtBearer").Value);
        services.AddTransient<IMarketService>(p =>
            new ExanteMarketService(exClient, p.GetRequiredService<IMarketDataCache>()));

        services.AddScoped<IUserApi>(sp =>
            new UserApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("UserApi"),
                configuration.GetValue<string>("UserApiConfig:Url") ?? string.Empty)
        );
        services.AddScoped<ITradingAccountApi>(sp =>
            new TradingAccountApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("TradingAccountApi"),
                configuration.GetValue<string>("OnboardingApiConfig:Url") ?? string.Empty)
        );
        services.AddScoped<ICustomerInfoApi>(sp =>
            new CustomerInfoApi(sp.GetRequiredService<IHttpClientFactory>().CreateClient("CustomerInfoApi"),
                configuration.GetValue<string>("OnboardingApiConfig:Url") ?? string.Empty)
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

        services.AddScoped<INoteRepository, NoteRepository>();

        AddMarketData(services, configuration);

        return services;
    }

    static void AddMarketData(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IMarketDataService, MarketDataService>();
        var piMarketDataUrl = config.GetSection("PiMarketDataConfig:Url").Value ?? string.Empty;
        services.AddScoped<IMarketDataApi>(sp =>
            new MarketDataApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("PiMarketData"),
                piMarketDataUrl)
        );

        var siriusMarketData = config.GetSection("SiriusConfig:Url").Value ?? string.Empty;
        services.AddScoped<ISiriusApi>(sp =>
            new SiriusApi(
                sp.GetRequiredService<IHttpClientFactory>().CreateClient("SiriusMarketData"),
                siriusMarketData)
        );
    }
}
