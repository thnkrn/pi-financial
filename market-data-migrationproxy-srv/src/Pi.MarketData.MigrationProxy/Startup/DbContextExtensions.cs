using Pi.MarketData.Infrastructure.Interfaces.Mongo;
using Pi.MarketData.Infrastructure.Services.Mongo;

namespace Pi.MarketData.MigrationProxy.API.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        return services;
    }
}