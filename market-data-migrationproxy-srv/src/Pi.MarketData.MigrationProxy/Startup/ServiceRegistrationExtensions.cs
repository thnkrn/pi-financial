using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Application.Services;
using Pi.MarketData.MigrationProxy.API.Handlers;
using Pi.MarketData.MigrationProxy.API.Helpers;
using Pi.MarketData.MigrationProxy.API.Interfaces;
using Pi.MarketData.MigrationProxy.API.Options;

namespace Pi.MarketData.MigrationProxy.API.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Add Transient service
        services.AddTransient<IWebSocketService, WebSocketService>();
        services.AddTransient<IProxyService, ProxyService>();

        // Add Scoped service
        services.AddScoped<IHttpRequestHelper, HttpRequestHelper>();
        services.AddScoped<IHttpResponseHelper, HttpResponseHelper>();
        services.AddScoped(provider =>
            new HandleExceptionFilter(provider.GetRequiredService<ILogger<HandleExceptionFilter>>())
        );

        // Add Controllers
        services.AddControllers(options => { options.Filters.Add<HandleExceptionFilter>(); });
        services.AddSingleton<ICuratedFilterService>(provider =>
            new CuratedFilterService("curated-filter.json")
        );

        // Add Options
        services.AddOptions<HttpForwarderOptions>()
            .Bind(configuration.GetSection(HttpForwarderOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }
}
