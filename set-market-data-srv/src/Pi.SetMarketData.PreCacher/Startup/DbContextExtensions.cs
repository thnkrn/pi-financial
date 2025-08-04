using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Services.Mongo;
using Pi.SetMarketData.Infrastructure.Services.TimescaleEf;

namespace Pi.SetMarketData.PreCacher.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        // Using EFCore for Postgresql
        services.AddDbContextFactory<TimescaleContext>(options =>
            options.UseNpgsql(configuration.GetSection(ConfigurationKeys.TimescaleConnection).Value,
                b => b.MigrationsAssembly("Pi.SetMarketData.Infrastructure")));
        services.AddSingleton(typeof(ITimescaleService<>), typeof(TimescaleService<>));
        services.AddSingleton(typeof(ITimescaleRepository<>), typeof(TimescaleRepository<>));
        return services;
    }
}