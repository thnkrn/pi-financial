using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Services.Kafka;

namespace Pi.SetMarketData.DataMigrationJobProducer.Startup;

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