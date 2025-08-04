using Confluent.Kafka;

namespace Pi.SetMarketData.Infrastructure.Interfaces.Helpers;

public interface IConsumerHelper
{
    public IConsumer<Ignore, string> GetConsumer(string GroupId, string BootstrapServers);
    public IProducer<string, string> GetProducer(string BootstrapServers);
}