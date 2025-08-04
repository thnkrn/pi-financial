using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;

namespace Pi.SetMarketDataRealTime.DataServer.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        return services;
    }
}