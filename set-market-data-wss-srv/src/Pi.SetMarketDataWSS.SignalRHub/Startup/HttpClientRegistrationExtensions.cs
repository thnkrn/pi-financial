namespace Pi.SetMarketDataWSS.SignalRHub.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}