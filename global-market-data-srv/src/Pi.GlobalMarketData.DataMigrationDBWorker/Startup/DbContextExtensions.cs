using Microsoft.EntityFrameworkCore;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.GlobalMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.GlobalMarketData.DataMigrationDBWorker.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Using EFCore for Postgresql
        services.AddDbContextFactory<TimescaleContext>(options =>
            options.UseNpgsql(configuration.GetSection(ConfigurationKeys.TimescaleConnection).Value,
                b => b.MigrationsAssembly("Pi.GlobalMarketData.Infrastructure")));
        services.AddScoped(typeof(ITimescaleService<>), typeof(TimescaleService<>));
        services.AddScoped(typeof(ITimescaleRepository<>), typeof(TimescaleRepository<>));
        return services;
    }
}
