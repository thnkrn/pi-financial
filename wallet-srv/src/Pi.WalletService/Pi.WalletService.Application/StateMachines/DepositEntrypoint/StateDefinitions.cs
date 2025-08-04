using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;

namespace Pi.WalletService.Application.StateMachines.DepositEntrypoint;

public class StateDefinition : SagaDefinition<DepositEntrypointState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<DepositEntrypointState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
