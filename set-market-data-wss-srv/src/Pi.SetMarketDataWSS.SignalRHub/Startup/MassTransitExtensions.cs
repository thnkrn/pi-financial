namespace Pi.SetMarketDataWSS.SignalRHub.Startup;

public static class MassTransitExtensions
{
    public static IServiceCollection SetupMassTransit(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        return services;
    }
}