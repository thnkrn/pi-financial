using Pi.SetMarketData.DataMigrationDBWorker.Handlers;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Services.Kafka;

namespace Pi.SetMarketData.DataMigrationDBWorker.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IKafkaMessageHandler<string, string>, KafkaMessageHandler>();

        var topics = ConfigurationHelper.GetTopicList(configuration, ConfigurationKeys.KafkaTopic);

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
