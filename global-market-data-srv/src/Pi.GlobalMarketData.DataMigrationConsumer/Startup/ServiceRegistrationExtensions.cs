using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.GlobalMarketData.Application.Services.SomeExternal;
using Pi.GlobalMarketData.DataMigrationConsumer.Handlers;
using Pi.GlobalMarketData.DataMigrationConsumer.Helpers;
using Pi.GlobalMarketData.DataMigrationConsumer.Interfaces;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Infrastructure.Services;
using Pi.GlobalMarketData.Infrastructure.Services.Kafka;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        services.AddSingleton<IKafkaPublisher<string, string>>(_ =>
            new KafkaPublisher<string, string>(
                configuration
            )
        );

        services.AddSingleton<IKafkaMessageHandler<string, string>, KafkaMessageHandler>();


        List<string> topics =
        [
            configuration.GetValue<string>(ConfigurationKeys.KafkaMigrationJobTopicName) ?? string.Empty
        ];

        services.AddSingleton<IKafkaSubscriber<string, string>>(provider =>
            new KafkaSubscriber<string, string>(
                configuration,
                provider.GetRequiredService<ILogger<KafkaSubscriber<string, string>>>(),
                topics,
                provider.GetRequiredService<IKafkaMessageHandler<string, string>>()
            )
        );

        services.AddSingleton<IKafkaPublisher<string, string>, KafkaPublisher<string, string>>();
        services.AddSingleton<IConsumerHelper, ConsumerHelper>();

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