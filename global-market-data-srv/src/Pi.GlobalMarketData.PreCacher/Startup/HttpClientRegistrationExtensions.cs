using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Pi.GlobalMarketData.PreCacher.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        return services;
    }
}