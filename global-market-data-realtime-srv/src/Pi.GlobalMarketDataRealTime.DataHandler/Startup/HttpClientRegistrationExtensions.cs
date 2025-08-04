namespace Pi.GlobalMarketDataRealTime.DataHandler.Startup;

public static class HttpClientRegistrationExtensions
{
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        services.AddHttpClient("MarketSession");
        return services;
    }
}