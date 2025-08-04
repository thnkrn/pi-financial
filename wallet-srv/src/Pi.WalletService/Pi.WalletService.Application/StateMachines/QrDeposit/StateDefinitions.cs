using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;

namespace Pi.WalletService.Application.StateMachines.QrDeposit;

public class StateDefinition : SagaDefinition<QrDepositState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<QrDepositState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
