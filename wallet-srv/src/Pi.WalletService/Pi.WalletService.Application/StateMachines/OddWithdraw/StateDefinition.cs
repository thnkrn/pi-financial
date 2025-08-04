using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;

namespace Pi.WalletService.Application.StateMachines.OddWithdraw;

public class StateDefinition : SagaDefinition<OddWithdrawState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OddWithdrawState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
