using MassTransit;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Application.StateMachines
{
    public class FundAccountOpeningStateDefinition : SagaDefinition<FundAccountOpeningState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<FundAccountOpeningState> sagaConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseInMemoryOutbox(context);
        }
    }
}
