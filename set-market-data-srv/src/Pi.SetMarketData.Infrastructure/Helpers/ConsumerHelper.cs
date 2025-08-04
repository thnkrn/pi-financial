using Confluent.Kafka;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;

namespace Pi.SetMarketData.Infrastructure.Helpers;

public class ConsumerHelper : IConsumerHelper
{
    public IConsumer<Ignore, string> GetConsumer(string GroupId, string BootstrapServers)
    {
        var consumerConfig = new ConsumerConfig
        {
            GroupId = GroupId,
            BootstrapServers = BootstrapServers,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        return new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
    }

    public IProducer<string, string> GetProducer(string BootstrapServers)
    {
        var producerConfig = new ProducerConfig { BootstrapServers = BootstrapServers };
        return new ProducerBuilder<string, string>(producerConfig).Build();
    }
}
