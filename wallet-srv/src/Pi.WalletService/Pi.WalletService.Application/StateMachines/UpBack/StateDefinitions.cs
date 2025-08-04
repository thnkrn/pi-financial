using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;

namespace Pi.WalletService.Application.StateMachines.UpBack;

public class StateDefinition : SagaDefinition<UpBackState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<UpBackState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
