using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.GlobalMarketDataWSS.Application.Interfaces.FixMapper;
using Pi.GlobalMarketDataWSS.Application.Services.FixMapper;
using Pi.GlobalMarketDataWSS.DataSubscriber.Handlers;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketDataWSS.Infrastructure.Services.Kafka;
using Pi.GlobalMarketDataWSS.Infrastructure.Services.Mongo;
using Pi.GlobalMarketDataWSS.Infrastructure.Services.Redis;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Startup;

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
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddSingleton<IPriceInfoMapperService, PriceInfoMapperService>();
        services.AddSingleton<IMarketScheduleDataService, MarketScheduleDataService>();
        services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
        services.AddSingleton<IRedisV2Publisher, RedisV2Publisher>();
        services.AddSingleton<IKafkaMessageHandler<Message<string, string>>, KafkaMessageHandler>();
        services.AddSingleton<IHealthCheckReporter, LoggingHealthCheckReporter>();
        services.AddSingleton<IKafkaSubscriber>(provider =>
            new KafkaSubscriber<string, string>(
                configuration,
                configuration[ConfigurationKeys.KafkaTopic] ?? string.Empty,
                provider.GetRequiredService<IKafkaMessageHandler<Message<string, string>>>(),
                provider.GetRequiredService<ILogger<KafkaSubscriber<string, string>>>()));
        
        services.AddSingleton<MessageSamplerService>();
        services.AddHostedService(sp => sp.GetRequiredService<MessageSamplerService>());

        return services;
    }
}