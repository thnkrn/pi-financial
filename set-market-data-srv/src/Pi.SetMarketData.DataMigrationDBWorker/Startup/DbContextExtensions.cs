using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.SetMarketData.DataMigrationDBWorker.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetSection(ConfigurationKeys.TimescaleConnection).Value;

        // Register regular DbContext
        services.AddDbContext<TimescaleContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly("Pi.SetMarketData.Infrastructure")),
            ServiceLifetime.Singleton);

        // Add DbContextFactory
        services.AddDbContextFactory<TimescaleContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("Pi.SetMarketData.Infrastructure")));

        services.AddSingleton(typeof(ITimescaleService<>), typeof(TimescaleService<>));
        services.AddSingleton(typeof(ITimescaleRepository<>), typeof(TimescaleRepository<>));

        return services;
    }
}