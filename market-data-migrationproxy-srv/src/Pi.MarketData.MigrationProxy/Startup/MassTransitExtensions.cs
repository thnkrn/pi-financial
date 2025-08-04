namespace Pi.MarketData.MigrationProxy.API.Startup;

public static class MassTransitExtensions
{
    public static IServiceCollection SetupMassTransit(this IServiceCollection services,
        ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        return services;
    }
}