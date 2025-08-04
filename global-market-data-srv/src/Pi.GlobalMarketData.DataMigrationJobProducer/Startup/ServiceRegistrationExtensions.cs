using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Infrastructure.Services.Kafka;

namespace Pi.GlobalMarketData.DataMigrationJobProducer.Startup;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IKafkaPublisher<string, string>>(provider =>
            new KafkaPublisher<string, string>(
                configuration
            )
        );
        return services;
    }
}