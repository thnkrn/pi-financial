using Microsoft.Extensions.Diagnostics.HealthChecks;
using Pi.Common.HealthCheck;
using Pi.SetMarketData.DataMigrationConsumer.Handlers;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Services.Kafka;

namespace Pi.SetMarketData.DataMigrationConsumer.Startup;

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
        return services;
    }
}