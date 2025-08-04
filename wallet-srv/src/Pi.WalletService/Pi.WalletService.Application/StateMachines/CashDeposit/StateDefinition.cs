using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;

namespace Pi.WalletService.Application.StateMachines.CashDeposit;

public class StateDefinition : SagaDefinition<CashDepositState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CashDepositState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}