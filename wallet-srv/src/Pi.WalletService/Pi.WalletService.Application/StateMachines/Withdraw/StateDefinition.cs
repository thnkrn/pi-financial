using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
namespace Pi.WalletService.Application.StateMachines.Withdraw;

public class StateDefinition : SagaDefinition<WithdrawState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<WithdrawState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}