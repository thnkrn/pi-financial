using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
namespace Pi.WalletService.Application.StateMachines.Deposit;

public class StateDefinition : SagaDefinition<DepositState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<DepositState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}