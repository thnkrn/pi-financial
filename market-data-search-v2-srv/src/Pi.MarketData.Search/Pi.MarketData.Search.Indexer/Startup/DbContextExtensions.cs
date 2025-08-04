using Microsoft.Extensions.DependencyInjection;
using Pi.MarketData.Search.Infrastructure.Interfaces.Mongo;
using Pi.MarketData.Search.Infrastructure.Services.Mongo;

namespace Pi.MarketData.Search.API.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services)
    {
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        return services;
    }
}