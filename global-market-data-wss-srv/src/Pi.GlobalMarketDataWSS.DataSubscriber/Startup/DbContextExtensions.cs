namespace Pi.GlobalMarketDataWSS.DataSubscriber.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}