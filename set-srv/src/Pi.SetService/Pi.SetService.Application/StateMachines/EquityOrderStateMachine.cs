using System.Linq.Expressions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.OnePort.IntegrationEvents;
using Pi.SetService.Application.Commands;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Factories;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
using Pi.SetService.Domain.Events;
using Pi.SetService.IntegrationEvents;

namespace Pi.SetService.Application.StateMachines;

public class EquityOrderStateMachine : MassTransitStateMachine<EquityOrderState>
{
    public EquityOrderStateMachine(ILogger<EquityOrderStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);
        Event(() => OrderRequestReceived, e =>
        {
            e.InsertOnInitial = true;
            e.CorrelateById(context => context.Message.CorrelationId);
            e.SetSagaFactory(context =>
            {
                return new EquityOrderState(context.Message.CorrelationId,
                    context.Message.TradingAccountId,
                    context.Message.TradingAccountNo,
                    context.Message.TradingAccountType,
                    context.Message.CustomerCode,
                    context.Message.SecSymbol);
            });
        });
        Event(() => SyncCreateOrderReceived, e =>
        {
            e.InsertOnInitial = true;
            e.CorrelateById(context => context.Message.CorrelationId);
            e.SetSagaFactory(context => DomainFactory.NewEquityOrderState(context.Message));
        });
        Event(() => OrderChangedReceived, e => { e.CorrelateById(context => context.Message.CorrelationId); });
        Event(() => OrderMatchedReceived, e => { e.CorrelateById(context => context.Message.CorrelationId); });
        Event(() => OrderRejectedReceived, e => { e.CorrelateById(context => context.Message.CorrelationId); });
        Event(() => OrderCancelledReceived, e => { e.CorrelateById(context => context.Message.CorrelationId); });
        Request(() => SendOrderRequest, x => { x.Timeout = TimeSpan.Zero; });

        State(() => SendingOrderToBroker);
        State(() => OrderPlaced);
        State(() => PlaceOrderFailed);
        State(() => OrderSynced);
        State(() => OrderChanged);
        State(() => OrderMatched);
        State(() => OrderRejected);
        State(() => OrderCancelled);

        Initially(
            When(OrderRequestReceived)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.TradingAccountId = context.Message.TradingAccountId;
                    context.Saga.TradingAccountNo = context.Message.TradingAccountNo;
                    context.Saga.CustomerCode = context.Message.CustomerCode;
                    context.Saga.ConditionPrice = context.Message.ConditionPrice;
                    context.Saga.Price = context.Message.Price;
                    context.Saga.Volume = context.Message.Volume;
                    context.Saga.PubVolume = context.Message.Volume;
                    context.Saga.OrderAction = context.Message.Action;
                    context.Saga.OrderType = context.Message.Type;
                    context.Saga.OrderSide = context.Message.Side;
                    context.Saga.SecSymbol = context.Message.SecSymbol;
                    context.Saga.Condition = context.Message.Condition;
                    context.Saga.Ttf = context.Message.Ttf;
                    context.Saga.Lending = context.Message.Lending;
                    context.Saga.ResponseAddress = context.ResponseAddress?.ToString()!;
                    context.Saga.RequestId = context.RequestId;
                })
                .TransitionTo(SendingOrderToBroker)
                .Request(SendOrderRequest, ctx => new SendOrderRequest
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    UserId = ctx.Message.UserId,
                    TradingAccountNo = ctx.Saga.TradingAccountNo,
                    TradingAccountType = ctx.Saga.TradingAccountType,
                    ConditionPrice = ctx.Saga.ConditionPrice,
                    Volume = ctx.Saga.Volume,
                    PubVolume = ctx.Saga.PubVolume,
                    Action = ctx.Saga.OrderAction,
                    OrderType = (OrderType)ctx.Saga.OrderType!,
                    OrderSide = ctx.Saga.OrderSide,
                    SecSymbol = ctx.Saga.SecSymbol,
                    Condition = ctx.Saga.Condition,
                    Ttf = ctx.Saga.Ttf,
                    Lending = ctx.Saga.Lending,
                    Price = ctx.Saga.Price
                }),
            When(SyncCreateOrderReceived)
                .Then(q =>
                {
                    q.Saga.OrderDateTime = q.Message.OrderDateTime ?? DateTime.UtcNow;
                })
                .TransitionTo(OrderPlaced)
        );

        During(SendingOrderToBroker,
            When(SendOrderRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.BrokerOrderId = context.Message.BrokerOrderNo;
                    context.Saga.OrderNo = context.Message.OrderNo;
                    context.Saga.EnterId = context.Message.EnterId;
                    context.Saga.ServiceType = context.Message.ServiceType;
                    context.Saga.OrderDateTime = DateTime.UtcNow;
                })
                .Publish(ctx => new SetOrderPlaced
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    TradingAccountNo = ctx.Saga.TradingAccountNo,
                    OrderNo = ctx.Saga.OrderNo!
                })
                .ThenAsync(async ctx =>
                {
                    await ctx.SendResponseAsync(new PlaceOrderSuccess(ctx.Message.OrderNo,
                        ctx.Message.BrokerOrderNo));
                })
                .TransitionTo(OrderPlaced),
            When(SendOrderRequest?.Faulted).SendPlaceOrderFailed(logger).TransitionTo(PlaceOrderFailed)
        );

        foreach (var state in new[] { OrderPlaced, OrderRejected })
        {
            During(state,
                When(OrderChangedReceived)
                    .Then(context =>
                    {
                        context.Saga.OrderStatus = OrderStatus.Pending;
                        context.Saga.Price = context.Message.Price;
                        context.Saga.Volume = context.Message.Volume;
                        context.Saga.Ttf = context.Message.Ttf;
                    }).TransitionTo(OrderPlaced),
                When(OrderMatchedReceived)
                    .Then(context =>
                    {
                        context.Saga.OrderStatus = OrderStatus.Matched;
                        var totalVolume = context.Message.Volume + (context.Saga.MatchedVolume ?? 0);
                        var weightMatched = context.Message.Volume * context.Message.Price;
                        var weightSagaMatched = (context.Saga.MatchedVolume ?? 0) * (context.Saga.MatchedPrice ?? 0);
                        context.Saga.MatchedPrice = (weightMatched + weightSagaMatched) / totalVolume;
                        context.Saga.MatchedVolume = (context.Saga.MatchedVolume ?? 0) + decimal.ToInt32(context.Message.Volume);
                    })
                    .IfElse(context => context.Saga is { MatchedVolume: not null, MatchedPrice: not null } &&
                            context.Saga.MatchedVolume != 0 && context.Saga.MatchedPrice != 0 &&
                            (decimal)(context.Saga.Volume - context.Saga.MatchedVolume) <= 0,
                        binder => binder.Publish(context =>
                        {
                            return new NotifyOrderMatched
                            {
                                Volume = context.Saga.Volume,
                                VolumeMatched = (decimal)context.Saga.MatchedVolume!,
                                AvgPrice = (decimal)context.Saga.MatchedPrice!,
                                CustCode = context.Saga.CustomerCode,
                                Symbol = context.Saga.SecSymbol,
                                OrderAction = context.Saga.OrderAction
                            };
                        }).TransitionTo(OrderMatched),
                        binder => binder.TransitionTo(OrderPlaced)),
                When(OrderRejectedReceived)
                    .Then(context =>
                    {
                        context.Saga.OrderStatus = OrderStatus.Rejected;
                        context.Saga.FailedReason = $"{context.Message.Reason}";
                    })
                    .Publish(context => new NotifyOrderRejected
                    {
                        Volume = context.Saga.Volume,
                        CustCode = context.Saga.CustomerCode,
                        Symbol = context.Saga.SecSymbol,
                        OrderAction = context.Saga.OrderAction
                    })
                    .TransitionTo(OrderRejected),
                When(OrderCancelledReceived)
                    .Then(context =>
                    {
                        context.Saga.OrderStatus = OrderStatus.Cancelled;
                        context.Saga.CancelledVolume = decimal.ToInt32(context.Message.CancelledVolume);
                    })
                    .If(context => context.Saga is { MatchedVolume: not null, MatchedPrice: not null } && context.Saga.MatchedVolume != 0 && context.Saga.MatchedPrice != 0,
                        binder => binder.Publish(context => new NotifyOrderCancelledPartially
                        {
                            CancelledVolume = context.Message.CancelledVolume,
                            OrderVolume = context.Saga.Volume,
                            VolumeMatched = (decimal)context.Saga.MatchedVolume!,
                            AvgPrice = (decimal)context.Saga.MatchedPrice!,
                            CustCode = context.Saga.CustomerCode,
                            Symbol = context.Saga.SecSymbol,
                            OrderAction = context.Saga.OrderAction
                        }))
                    .TransitionTo(OrderCancelled)
            );
        }

        foreach (var state in new[] { OrderMatched, OrderCancelled })
        {
            During(
                state,
                Ignore(OrderMatchedReceived),
                Ignore(OrderChangedReceived),
                Ignore(OrderCancelledReceived),
                Ignore(OrderRejectedReceived)
            );
        }
    }

    public Event<OrderRequestReceived>? OrderRequestReceived { get; private set; }
    public Event<SyncCreateOrderReceived>? SyncCreateOrderReceived { get; private set; }
    public Request<EquityOrderState, SendOrderRequest, SendOrderResponse>? SendOrderRequest { get; private set; }
    public Event<OrderCancelled>? OrderCancelledReceived { get; private set; }
    public Event<OrderMatched>? OrderMatchedReceived { get; private set; }
    public Event<OrderChanged>? OrderChangedReceived { get; private set; }
    public Event<OrderRejected>? OrderRejectedReceived { get; private set; }

    public static State? SendingOrderToBroker { get; set; }
    public static State? OrderPlaced { get; set; }
    public static State? PlaceOrderFailed { get; set; }
    public static State? OrderSynced { get; set; }
    public static State? OrderChanged { get; set; }
    public static State? OrderMatched { get; set; }
    public static State? OrderRejected { get; set; }
    public static State? OrderCancelled { get; set; }

    public static string GetName<T>(Expression<Func<T>> memberExpression)
    {
        var expressionBody = (MemberExpression)memberExpression.Body;
        return expressionBody.Member.Name;
    }

    public static void CleanUp()
    {
        SendingOrderToBroker = null;
        OrderPlaced = null;
        PlaceOrderFailed = null;
        OrderCancelled = null;
        OrderChanged = null;
        OrderMatched = null;
        OrderRejected = null;
        OrderSynced = null;
    }

    private static Action<IEventCorrelationConfigurator<EquityOrderState, TM>> ConfigureOnePortEvent<TM>()
        where TM : OnePortOrderEvent
    {
        return configurator =>
        {
            configurator
                .CorrelateBy((state,
                        ctx) => DateOnly.FromDateTime(state.OrderDateTime ?? state.CreatedAt) ==
                                DateOnly.FromDateTime(ctx.Message.TransactionDateTime) &&
                                state.BrokerOrderId == ctx.Message.FisOrderId)
                .SelectId(x => x.CorrelationId ?? NewId.NextGuid());

            configurator.InsertOnInitial = false;
        };
    }
}

public static class EquityOrderStateMachineExtension
{
    public static async Task SendResponseAsync<T, TMessage>(this BehaviorContext<EquityOrderState, T> context,
        TMessage message)
        where T : class
        where TMessage : class
    {
        if (context.Saga is { ResponseAddress: not null, RequestId: not null })
        {
            var endpoint = await context.GetSendEndpoint(new Uri(context.Saga.ResponseAddress!));
            await endpoint.Send(
                message,
                ctx => ctx.RequestId = context.Saga.RequestId,
                context.CancellationToken
            );
        }
    }

    public static EventActivityBinder<EquityOrderState, Fault<TData>> SendPlaceOrderFailed<TData>(
        this EventActivityBinder<EquityOrderState, Fault<TData>> binder, ILogger<EquityOrderStateMachine> logger)
        where TData : class
    {
        return binder
            .ThenAsync(async context =>
            {
                context.Saga.FailedReason = context.Message.Exceptions.FirstOrDefault()?.Message;
                logger.LogError("Place {SagaOrderType} order failed with error: {Message}", context.Saga.OrderType,
                    context.Saga.FailedReason);

                if (context.Saga.ResponseAddress == null || context.Saga.RequestId == null) return;

                var exception = context.Message.Exceptions.FirstOrDefault();
                var (errorCode, msg) = GetSetExceptionDetail(exception);
                if (exception != null)
                    logger.LogError(
                        "Place {SagaOrderType} order failed with error: {FailedReason} and exception info {ExceptionExceptionType} {ExceptionMessage} {ExceptionStackTrace}",
                        context.Saga.OrderType,
                        context.Saga.FailedReason,
                        exception.ExceptionType,
                        exception.Message,
                        exception.StackTrace);

                var endpoint = await context.GetSendEndpoint(new Uri(context.Saga.ResponseAddress!));
                await endpoint.Send(
                    new PlaceOrderFailed(context.Saga.CorrelationId, errorCode, msg, exception),
                    ctx => ctx.RequestId = context.Saga.RequestId
                );
            });
    }

    private static (SetErrorCode, string?) GetSetExceptionDetail(ExceptionInfo? exception)
    {
        var errorCode = SetErrorCode.SE201;
        string? msg = null;

        if (exception?.ExceptionType.Equals(typeof(SetException).ToString()) == true && exception.Data?["Code"] != null)
        {
            errorCode = Enum.TryParse((string)exception.Data?["Code"]!, out SetErrorCode parse) ? parse : errorCode;
            msg = exception.Message;
        }

        return (errorCode, msg);
    }
}
