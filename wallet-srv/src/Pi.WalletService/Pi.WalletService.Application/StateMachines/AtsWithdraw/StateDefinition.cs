using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;

namespace Pi.WalletService.Application.StateMachines.AtsWithdraw;

public class StateDefinition : SagaDefinition<AtsWithdrawState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AtsWithdrawState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}