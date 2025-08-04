using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;

namespace Pi.WalletService.Application.StateMachines.CashWithdraw;

public class StateDefinition : SagaDefinition<CashWithdrawState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<CashWithdrawState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}