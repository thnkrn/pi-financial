using System.Net.Http.Headers;
using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Application.Utils;
using Pi.GlobalMarketData.DataMigrationConsumer.Constants;
using Pi.GlobalMarketData.DataMigrationConsumer.Interfaces;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Models.DataMigration;
using Pi.GlobalMarketData.Domain.Models.Request.Velexa;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Services;

public class ConsumerService : BackgroundService
{
    private readonly IKafkaSubscriber<string, string> _kafkaSubscriber;

    public ConsumerService(
        IKafkaSubscriber<string, string> kafkaSubscriber
    )
    {
        _kafkaSubscriber = kafkaSubscriber;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _kafkaSubscriber.SubscribeAsync(stoppingToken);
        }
    }
}