using Pi.SetMarketData.SmartIntegration.Services;
using Pi.SetMarketData.SmartIntegration.Interfaces;
using Pi.SetMarketData.Application.Interfaces.BrokerIdMapperService;
using Pi.SetMarketData.Application.Services.BrokerId;

namespace Pi.SetMarketData.SmartIntegration.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHostedService<CronJobService>();
        services.AddSingleton<IDatabaseTaskService, DatabaseTaskService>();
        services.AddScoped<IBrokerIdMapperService, BrokerIdMapperService>();
        return services;
    }
}