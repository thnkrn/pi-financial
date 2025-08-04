using MassTransit;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.StateMachines;

public class EquityOrderStateDefinition : SagaDefinition<EquityOrderState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<EquityOrderState> sagaConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseInMemoryOutbox(context);
    }
}
