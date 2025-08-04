using Pi.GlobalMarketData.Application.Services.SomeExternal;
using Pi.GlobalMarketData.Infrastructure.Services;

namespace Pi.GlobalMarketData.MigrationProxy.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Example Init Service
        services.AddScoped<ISomeExternalService, SomeExternalService>();

        return services;
    }
}