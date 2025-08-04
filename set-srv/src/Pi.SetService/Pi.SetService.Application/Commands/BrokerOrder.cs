using MassTransit;
using Pi.OnePort.IntegrationEvents;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;

namespace Pi.SetService.Application.Commands;

public class BrokerOrderConsumer(IEquityOrderStateRepository repository) :
    IConsumer<OnePortOrderMatched>,
    IConsumer<OnePortOrderChanged>,
    IConsumer<OnePortOrderCanceled>,
    IConsumer<OnePortOrderRejected>
{
    private const int AttemptLimit = 10;
    private const int RetryDelay = 3;

    public async Task Consume(ConsumeContext<OnePortOrderMatched> context)
    {
        var orderState = await GetEquityOrderState(context.Message.FisOrderId, context.Message.TransactionDateTime, context.CancellationToken);
        await context.Publish(new OrderMatched
        {
            CorrelationId = orderState.CorrelationId,
            Symbol = context.Message.Symbol,
            Volume = context.Message.Volume,
            Price = context.Message.Price,
            TransactionTime = context.Message.TransactionDateTime,
        });
    }

    public async Task Consume(ConsumeContext<OnePortOrderChanged> context)
    {
        var orderState = await GetEquityOrderState(context.Message.FisOrderId, context.Message.TransactionDateTime, context.CancellationToken);
        await context.Publish(new OrderChanged
        {
            CorrelationId = orderState.CorrelationId,
            Price = context.Message.Price,
            Volume = decimal.ToInt32(context.Message.Volume),
            TransactionTime = context.Message.TransactionDateTime,
            Ttf = IntegrationEventFactory.NewTtf(context.Message.Ttf)
        });
    }

    public async Task Consume(ConsumeContext<OnePortOrderCanceled> context)
    {
        var orderState = await GetEquityOrderState(context.Message.FisOrderId, context.Message.TransactionDateTime, context.CancellationToken);
        await context.Publish(new OrderCancelled
        {
            CorrelationId = orderState.CorrelationId,
            CancelledVolume = context.Message.CancelVolume,
            Symbol = context.Message.Symbol,
            Source = DomainFactory.NewSource(context.Message.Source),
            TransactionTime = context.Message.TransactionDateTime,
        });
    }

    public async Task Consume(ConsumeContext<OnePortOrderRejected> context)
    {
        var orderState = await GetEquityOrderState(context.Message.FisOrderId, context.Message.TransactionDateTime, context.CancellationToken);
        await context.Publish(new OrderRejected
        {
            CorrelationId = orderState.CorrelationId,
            Source = DomainFactory.NewSource(context.Message.Source),
            TransactionTime = context.Message.TransactionDateTime,
            Reason = context.Message.Reason
        });
    }

    private async Task<EquityOrderState> GetEquityOrderState(string fisOrderId, DateTime transactionDateTime, CancellationToken ct = default)
    {
        for (var attempt = 0; attempt < AttemptLimit; attempt++)
        {
            var filters = new EquityOrderStateFilters
            {
                BrokerOrderId = fisOrderId,
                CreatedDate = DateOnly.FromDateTime(transactionDateTime)
            };
            var orderStates = await repository.GetEquityOrderStates(filters);
            var orderState = orderStates.FirstOrDefault(q => q.BrokerOrderId == fisOrderId);

            if (orderState != null) return orderState;

            await Task.Delay(TimeSpan.FromSeconds(RetryDelay), ct);
        }

        throw new SetException(SetErrorCode.SE117);
    }
}
