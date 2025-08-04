using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;

namespace Pi.WalletService.Application.StateMachines.Recovery;

public class StateDefinition : SagaDefinition<RecoveryState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<RecoveryState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
