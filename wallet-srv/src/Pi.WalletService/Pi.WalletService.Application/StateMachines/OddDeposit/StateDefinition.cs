using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;

namespace Pi.WalletService.Application.StateMachines.OddDeposit;

public class StateDefinition : SagaDefinition<OddDepositState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OddDepositState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
