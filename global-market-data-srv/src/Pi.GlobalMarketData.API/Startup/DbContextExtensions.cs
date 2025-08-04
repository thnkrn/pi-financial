using Microsoft.EntityFrameworkCore;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.GlobalMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.GlobalMarketData.API.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Using EFCore for Postgresql
        services.AddDbContextFactory<TimescaleContext>(options =>
            options.UseNpgsql(configuration.GetSection(ConfigurationKeys.TimescaleConnection).Value,
                b => b.MigrationsAssembly("Pi.GlobalMarketData.Infrastructure")));
        services.AddSingleton(typeof(ITimescaleService<>), typeof(TimescaleService<>));
        services.AddSingleton(typeof(ITimescaleRepository<>), typeof(TimescaleRepository<>));
        return services;
    }
}