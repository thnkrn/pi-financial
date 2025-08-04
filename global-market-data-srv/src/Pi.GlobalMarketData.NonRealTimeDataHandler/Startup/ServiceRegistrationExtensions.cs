using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.GlobalMarketData.Application.Services.SomeExternal;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Services;
using Pi.GlobalMarketData.Infrastructure.Services.Mongo;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Helpers;
using Pi.GlobalMarketData.NonRealTimeDataHandler.interfaces;
using Pi.GlobalMarketData.NonRealTimeDataHandler.Repository;

namespace Pi.GlobalMarketData.NonRealTimeDataHandler.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        // Example Init Service
        services.AddScoped<ISomeExternalService, SomeExternalService>();

        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        services.AddSingleton<IGeDataRepository, GeDataRepository>();
        services.AddSingleton<MongoDbServices>();

        services.AddSingleton<IVelexaHttpApiJwtTokenGenerator>(
            provider => new VelexaHttpApiJwtTokenGenerator(
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtSecret) ?? "",
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtClientId) ?? "",
                configuration.GetValue<string>(ConfigurationKeys.VelexaHttpApiJwtApptId) ?? ""
            )
        );
        return services;
    }
}
