using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pi.GlobalMarketData.PreCacher.Startup;

public static class MassTransitExtensions
{
    public static IServiceCollection SetupMassTransit(
        this IServiceCollection services,
        ConfigurationManager configuration,
        IWebHostEnvironment environment
    )
    {
        return services;
    }
}