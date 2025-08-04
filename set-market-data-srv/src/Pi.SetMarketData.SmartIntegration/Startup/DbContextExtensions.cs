using Microsoft.EntityFrameworkCore;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.SqlServer;
using Pi.SetMarketData.Infrastructure.Services.Mongo;
using Pi.SetMarketData.Infrastructure.Services.SqlServer;

namespace Pi.SetMarketData.SmartIntegration.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SqlServerContext>(options =>
            options.UseSqlServer(configuration.GetSection(ConfigurationKeys.SqlServerConnection).Value,
                b => b.MigrationsAssembly("Pi.SetMarketData.Infrastructure")), ServiceLifetime.Singleton);
        services.AddSingleton(typeof(ISqlServerService<>), typeof(SqlServerService<>));
        services.AddSingleton(typeof(ISqlServerRepository<>), typeof(SqlServerRepository<>));

        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        return services;
    }
}