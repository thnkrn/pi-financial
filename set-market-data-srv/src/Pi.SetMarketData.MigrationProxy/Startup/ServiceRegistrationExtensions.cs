using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.Services;
using Pi.SetMarketData.MigrationProxy.Helpers;
using Pi.SetMarketData.MigrationProxy.Handlers;

namespace Pi.SetMarketData.MigrationProxy.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Example Init Service
        services.AddScoped<IWebSocketService, WebSocketService>();
        services.AddScoped<IProxyService, ProxyService>();

        services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();

        services.AddScoped(provider =>
            new HandleExceptionFilter(provider.GetRequiredService<ILogger<HandleExceptionFilter>>())
        );

        return services;
    }
}