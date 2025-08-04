using System.Collections.Concurrent;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.SetMarketDataWSS.Application.Services.SomeExternal;
using Pi.SetMarketDataWSS.Domain.Models.Request;
using Pi.SetMarketDataWSS.Infrastructure.Services;
using Pi.SetMarketDataWSS.SignalRHub.Interfaces;
using Pi.SetMarketDataWSS.SignalRHub.Services;

namespace Pi.SetMarketDataWSS.SignalRHub.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // Example Init Service
        services.AddScoped<ISomeExternalService, SomeExternalService>();

        // This dictionary will be used to store connection-specific requests
        services.AddSingleton<ConcurrentDictionary<string, MarketStreamingRequest>>();
        
        // Register the tuned implementation as the main interface
        services.AddSingleton<IStreamingMarketDataSubscriberGroupFilterTuned, StreamingMarketDataSubscriberGroupFilterTuned>();

        // Register it as itself for direct access (for hosted service)
        services.AddSingleton<StreamingMarketDataSubscriberGroupFilterTuned>(sp => 
            (StreamingMarketDataSubscriberGroupFilterTuned)sp.GetRequiredService<IStreamingMarketDataSubscriberGroupFilterTuned>());

        return services;
    }
}