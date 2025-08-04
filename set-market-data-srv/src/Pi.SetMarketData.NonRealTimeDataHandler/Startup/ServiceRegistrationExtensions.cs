using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.SetMarketData.Application.Services.SomeExternal;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Services;
using Pi.SetMarketData.Infrastructure.Services.Mongo;
using Pi.SetMarketData.NonRealTimeDataHandler.Helpers;
using Pi.SetMarketData.NonRealTimeDataHandler.interfaces;

namespace Pi.SetMarketData.NonRealTimeDataHandler.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        return services;
    }
}
