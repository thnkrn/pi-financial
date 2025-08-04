using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace  Pi.SetMarketData.DeprecateCleaner.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<Services.DeprecateCleaner>();
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        return services;
    }
}
