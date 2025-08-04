namespace Pi.StructureNotes.API.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddHttpClient("UserApi")
            .ConfigureHttpClient((_, client) => { client.Timeout = TimeSpan.FromSeconds(30); });

        services.AddHttpClient("CustomerInfoApi")
            .ConfigureHttpClient((_, client) => { client.Timeout = TimeSpan.FromSeconds(30); });

        services.AddHttpClient("TradingAccountApi")
            .ConfigureHttpClient((_, client) => { client.Timeout = TimeSpan.FromSeconds(30); });

        services
            .AddHttpClient("GrowthBook")
            .ConfigureHttpClient(
                (_, client) =>
                {
                    client.BaseAddress = new Uri(
                        configuration.GetValue<string>("GrowthBook:Host")!
                    );
                    client.Timeout = TimeSpan.FromSeconds(30);
                }
            );

        return services;
    }
}
