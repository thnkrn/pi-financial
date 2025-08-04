using Amazon.SQS;
using MassTransit;
using Pi.User.Application.Commands;
namespace Pi.User.Worker.Consumers;

public class CreateOrUpdateDeviceConsumerDefinition
    : ConsumerDefinition<CreateOrUpdateDeviceConsumer>
{
    public CreateOrUpdateDeviceConsumerDefinition(IConfiguration configuration)
    {
        var formatter = new KebabCaseEndpointNameFormatter(configuration.GetValue<string>("MassTransit:EndpointNamePrefix"), false);
        Endpoint(x =>
        {
            x.Name = $"{formatter.Consumer<CreateOrUpdateDeviceConsumer>()}.fifo";
            x.ConcurrentMessageLimit = 1;
            x.PrefetchCount = 1;
        });
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateOrUpdateDeviceConsumer> consumerConfigurator, IRegistrationContext context)
    {
        var sqsEndpointConfigurator = (IAmazonSqsReceiveEndpointConfigurator)endpointConfigurator;
        sqsEndpointConfigurator.QueueAttributes[QueueAttributeName.FifoQueue] = true;
    }
}