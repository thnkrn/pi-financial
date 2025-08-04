using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Services.ItchMapper;
using Pi.SetMarketData.DataProcessingService.Handlers;
using Pi.SetMarketData.DataProcessingService.Helpers;
using Pi.SetMarketData.DataProcessingService.Interface;
using Pi.SetMarketData.DataProcessingService.Services;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Services.Kafka;
using Pi.SetMarketData.Infrastructure.Services.Mongo;
using Pi.SetMarketData.Infrastructure.Services.Redis;

namespace Pi.SetMarketData.DataProcessingService.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();
        services.AddSingleton<IRedisConnectionProvider, RedisConnectionProvider>();
        services.AddSingleton<IRedisV2Publisher, RedisV2Publisher>();
        services.AddSingleton<ICacheServiceHelper, CacheServiceHelper>();

        // Register IKafkaMessageHandler, KafkaMessageHandler and KafkaMessageHandlerDependencies
        services.AddSingleton<KafkaMessageHandlerDependencies>();
        services.AddSingleton<KafkaMessageHandlerMoreDependencies>();

        // Register IKafkaMessageHandler and KafkaMessageHandler
        services.AddSingleton<IKafkaMessageV2Handler<Message<string, string>>, KafkaMessageHandler>();

        // Register IKafkaMessageRecovery and KafkaMessageRecovery
        services.AddSingleton<IKafkaMessageRecovery<Message<string, string>>, KafkaMessageRecovery>();

        // Register IHealthCheckReporter and LoggingHealthCheckReporter
        services.AddSingleton<IHealthCheckReporter, LoggingHealthCheckReporter>();

        // Register IKafkaSubscriber and KafkaSubscriber
        var topics = ConfigurationHelper.GetTopicList(configuration, ConfigurationKeys.KafkaTopic);
        services.AddSingleton<IKafkaV2Subscriber>(provider =>
            new KafkaV2Subscriber<string, string>(configuration,
                topics,
                provider.GetRequiredService<IKafkaMessageV2Handler<Message<string, string>>>(),
                provider.GetRequiredService<ILogger<KafkaV2Subscriber<string, string>>>()));

        services.AddSingleton<IMongoContext, MongoContext>();
        services.AddSingleton(typeof(IMongoService<>), typeof(MongoService<>));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        // Register IItchMapperService
        services.AddSingleton<IItchMapperService, ItchMapperService>();

        // Register IItchPriceInfoMapperService
        services.AddSingleton<IItchPriceInfoMapperService, ItchPriceInfoMapperService>();

        // Register ItchOrderBookMapperService
        services.AddSingleton<IItchOrderBookMapperService, ItchOrderBookMapperService>();

        // Register ItchOrderBookMapperService
        services.AddSingleton<IItchOrderBookDirectoryMapperService, ItchOrderBookDirectoryService>();

        // Register ITickerSizeTable
        services.AddSingleton<IItchTickSizeTableEntryMapperService, ItchTickSizeTableEntryMapperService>();

        return services;
    }
}