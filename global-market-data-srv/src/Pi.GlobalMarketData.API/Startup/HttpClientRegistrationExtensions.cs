using Pi.GlobalMarketData.API.constants;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;

namespace Pi.GlobalMarketData.API.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services
            .AddHttpClient(HttpClientKeys.VelexaHttpApi)
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    client.BaseAddress = new Uri(
                        configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiBaseUrl)
                        ?? ""
                    );
                }
            );


        return services;
    }
}