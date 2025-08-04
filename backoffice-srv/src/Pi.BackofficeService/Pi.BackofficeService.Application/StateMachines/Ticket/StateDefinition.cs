using MassTransit;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Application.StateMachines.Ticket;

public class StateDefinition : SagaDefinition<TicketState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<TicketState> sagaConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
    }
}
