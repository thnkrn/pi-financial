using MassTransit;
using MassTransit.Serialization;
using Pi.WalletService.Application.Commands.Deposit;
namespace Pi.WalletService.Worker.ConsumerDefinitions;

class KkpDepositConsumerDefinition : ConsumerDefinition<ProcessKkpDepositCallbackConsumer>
{
    public KkpDepositConsumerDefinition(IConfiguration configuration)
    {
        var bankSrvEndpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:BankSrvEndpointNamePrefix"), false);
        Endpoint(x =>
        {
            x.Name = $"{bankSrvEndpointNameFormatter.Consumer<ProcessKkpDepositCallbackConsumer>()}.fifo";
            x.ConcurrentMessageLimit = 1;
            x.PrefetchCount = 1;
        });

    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProcessKkpDepositCallbackConsumer> consumerConfigurator)
    {
        var sqsEndpointConfigurator = (IAmazonSqsReceiveEndpointConfigurator)endpointConfigurator;
        sqsEndpointConfigurator.QueueAttributes["FifoQueue"] = "true";
        sqsEndpointConfigurator.QueueAttributes["ContentBasedDeduplication"] = "true";
        sqsEndpointConfigurator.ClearSerialization();
        sqsEndpointConfigurator.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
    }
}