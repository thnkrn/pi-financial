using Pi.GlobalMarketData.DataMigrationDBWorker.Handlers;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Infrastructure.Services.Kafka;

namespace Pi.GlobalMarketData.DataMigrationDBWorker.Startup;

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
