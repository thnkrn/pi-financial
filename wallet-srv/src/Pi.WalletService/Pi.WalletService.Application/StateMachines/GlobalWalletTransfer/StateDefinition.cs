using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;

namespace Pi.WalletService.Application.StateMachines.GlobalWalletTransfer;

public class StateDefinition : SagaDefinition<GlobalWalletTransferState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<GlobalWalletTransferState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
