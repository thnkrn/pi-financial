using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
namespace Pi.WalletService.Application.StateMachines.GlobalWalletManualAllocation;

public class StateDefinition : SagaDefinition<GlobalManualAllocationState>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<GlobalManualAllocationState> sagaConfigurator
    )
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}