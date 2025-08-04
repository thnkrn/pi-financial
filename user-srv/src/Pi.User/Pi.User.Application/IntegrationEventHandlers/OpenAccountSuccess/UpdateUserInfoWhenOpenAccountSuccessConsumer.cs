using MassTransit;
using Pi.OnboardService.IntegrationEvents;
using Pi.User.Application.Commands;

namespace Pi.User.Application.IntegrationEventHandlers.OpenAccountSuccess;

public class UpdateUserInfoWhenOpenAccountSuccessConsumer : IConsumer<OpenAccountSuccessEvent>
{
    public Task Consume(ConsumeContext<OpenAccountSuccessEvent> context)
    {
        // TODO: Implement this method to insert trading accounts to trading_accounts_v2 table.
        return Task.CompletedTask;
    }
}
