using Microsoft.EntityFrameworkCore;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.GlobalMarketData.Infrastructure.Services.Mongo;
using Pi.GlobalMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.GlobalMarketData.DataProcessingService.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        // Using EFCore for Postgresql
        services.AddDbContextFactory<TimescaleContext>(options =>
            options.UseNpgsql(configuration.GetSection(ConfigurationKeys.TimescaleConnection).Value,
                b => b.MigrationsAssembly("Pi.GlobalMarketData.Infrastructure")), ServiceLifetime.Scoped);
        services.AddScoped(typeof(ITimescaleService<>), typeof(TimescaleService<>));
        services.AddScoped(typeof(ITimescaleRepository<>), typeof(TimescaleRepository<>));

        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        return services;
    }
}
