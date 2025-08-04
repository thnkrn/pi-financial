using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;

namespace Pi.WalletService.Application.StateMachines.AtsDeposit;

public class StateDefinition : SagaDefinition<AtsDepositState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AtsDepositState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
