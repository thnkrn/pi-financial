using MassTransit;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.Financial.FundService.Application.Commands;
using Pi.OnboardService.IntegrationEvents;

namespace Pi.Financial.FundService.Application.IntegrationEventHandlers.TradingAccountBankAccountChanged;

public class SyncDataWhenBankAccountChangedConsumer : IConsumer<TradingAccountBankAccountChangedEvent>
{
    public async Task Consume(ConsumeContext<TradingAccountBankAccountChangedEvent> context)
    {
        if (context.Message.ProductName != ProductName.Funds)
        {
            return;
        }

        await context.Send(new SyncCustomerData(context.Message.CustomerCode, context.CorrelationId ?? Guid.NewGuid()), context.CancellationToken);
    }
}
