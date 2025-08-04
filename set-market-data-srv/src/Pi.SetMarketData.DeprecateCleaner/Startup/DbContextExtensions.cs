using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.SetMarketData.DeprecateCleaner.Models;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Services.Mongo;

namespace  Pi.SetMarketData.DeprecateCleaner.Startup;

public static class DbContextExtensions
{
    public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        
        // Configure MongoDB Settings
        services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.Options));
        services.AddOptions<MongoDbOptions>()
            .Bind(configuration.GetSection(MongoDbOptions.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }
}
