using MassTransit;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;

namespace Pi.WalletService.Application.StateMachines.Refund;

public class StateDefinition : SagaDefinition<RefundState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<RefundState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
