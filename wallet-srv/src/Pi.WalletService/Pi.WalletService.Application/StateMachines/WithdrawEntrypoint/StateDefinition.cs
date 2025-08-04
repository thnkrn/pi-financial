using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
namespace Pi.WalletService.Application.StateMachines.WithdrawEntrypoint;

public class StateDefinition : SagaDefinition<WithdrawEntrypointState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<WithdrawEntrypointState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
