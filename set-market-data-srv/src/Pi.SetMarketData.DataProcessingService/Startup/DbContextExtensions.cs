using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.SetMarketData.DataProcessingService.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        // Using EFCore for Postgresql
        services.AddDbContextFactory<TimescaleContext>(options =>
            options.UseNpgsql(configuration.GetSection(ConfigurationKeys.TimescaleConnection).Value,
                b => b.MigrationsAssembly("Pi.SetMarketData.Infrastructure")), ServiceLifetime.Scoped);
        services.AddScoped(typeof(ITimescaleService<>), typeof(TimescaleService<>));
        services.AddScoped(typeof(ITimescaleRepository<>), typeof(TimescaleRepository<>));

        return services;
    }
}