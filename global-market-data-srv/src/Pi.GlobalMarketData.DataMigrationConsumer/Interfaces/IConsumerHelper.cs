using Confluent.Kafka;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Interfaces;

public interface IConsumerHelper
{
    public IConsumer<Ignore, string> GetConsumer(string GroupId, string BootstrapServers);
    public IProducer<string, string> GetProducer(string BootstrapServers);
}