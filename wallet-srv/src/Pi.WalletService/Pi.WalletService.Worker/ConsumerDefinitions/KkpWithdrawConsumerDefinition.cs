using MassTransit;
using MassTransit.Serialization;
using Pi.WalletService.Application.Commands.Withdraw;
namespace Pi.WalletService.Worker.ConsumerDefinitions;

public class KkpWithdrawConsumerDefinition : ConsumerDefinition<ProcessKkpWithdrawCallbackConsumer>
{
    public KkpWithdrawConsumerDefinition(IConfiguration configuration)
    {
        var bankSrvEndpointNameFormatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:BankSrvEndpointNamePrefix"), false);
        Endpoint(x =>
        {
            x.Name = $"{bankSrvEndpointNameFormatter.Consumer<ProcessKkpWithdrawCallbackConsumer>()}.fifo";
            x.ConcurrentMessageLimit = 1;
            x.PrefetchCount = 1;
        });

    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<ProcessKkpWithdrawCallbackConsumer> consumerConfigurator)
    {
        var sqsEndpointConfigurator = (IAmazonSqsReceiveEndpointConfigurator)endpointConfigurator;
        sqsEndpointConfigurator.QueueAttributes["FifoQueue"] = "true";
        sqsEndpointConfigurator.QueueAttributes["ContentBasedDeduplication"] = "true";
        sqsEndpointConfigurator.ClearSerialization();
        sqsEndpointConfigurator.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
    }
}