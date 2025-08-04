using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.GlobalMarketData.Application.Services.SomeExternal;
using Pi.GlobalMarketData.DataProcessingService.Handlers;
using Pi.GlobalMarketData.DataProcessingService.Services;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Services;
using Pi.GlobalMarketData.Infrastructure.Services.Kafka;
using Pi.GlobalMarketData.Infrastructure.Services.Redis;

namespace Pi.GlobalMarketData.DataProcessingService.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        // Register IRedisConnectionProvider and RedisConnectionProvider
        services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
        
        // Register IRedisPublisher and RedisPublisher
        services.AddSingleton<IRedisV2Publisher, RedisV2Publisher>();

        // Example Init Service
        services.AddScoped<ISomeExternalService, SomeExternalService>();

        // Register IHealthCheckReporter and LoggingHealthCheckReporter
        services.AddSingleton<IHealthCheckReporter, LoggingHealthCheckReporter>();
        
        services.AddSingleton<IMarketScheduleDataService, MarketScheduleDataService>();

        // Register IKafkaMessageHandler and KafkaMessageHandler
        services.AddSingleton<IKafkaMessageV2Handler<Message<string, string>>, KafkaMessageHandler>();
        
        // Register IKafkaV2Subscriber and KafkaV2Subscriber
        var topics = ConfigurationHelper.GetTopicList(configuration, ConfigurationKeys.KafkaTopic);
        services.AddSingleton<IKafkaV2Subscriber>(provider =>
            new KafkaV2Subscriber<string, string>(
                configuration,
                topics,
                provider.GetRequiredService<IKafkaMessageV2Handler<Message<string, string>>>(),
                provider.GetRequiredService<ILogger<KafkaV2Subscriber<string, string>>>()));

        return services;
    }
}