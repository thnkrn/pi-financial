using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;

namespace Pi.WalletService.Application.StateMachines.GlobalTransfer;

public class StateDefinition : SagaDefinition<GlobalTransferState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<GlobalTransferState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
