using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Factories;
using Pi.Financial.FundService.Application.Models.Metric;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Domain.Events;
using OrderState = Pi.Financial.FundService.IntegrationEvents.Models.FundOrderState;

namespace Pi.Financial.FundService.Application.StateMachines;

public class FundOrderStateMachine : MassTransitStateMachine<FundOrderState>
{
    public Event<SubscriptionFundRequestReceived>? SubscriptionFundRequestReceived { get; private set; }
    public Event<RedemptionFundRequestReceived>? RedemptionFundRequestReceived { get; private set; }
    public Event<SwitchingFundRequestReceived>? SwitchingFundRequestReceived { get; private set; }
    public Event<CreateSwitchInFundOrderReceived>? CreateSwitchInFundOrderReceived { get; private set; }
    public Event<CancelFundOrderReceived>? CancelFundOrderReceived { get; private set; }
    public Request<FundOrderState, GenerateOrderNo, OrderNoGenerated>? OrderNoGenerateRequest { get; private set; }
    public Request<FundOrderState, SubscriptFund, SubscriptionFundOrder>? SubscriptionRequest { get; private set; }
    public Request<FundOrderState, RedeemFund, RedemptionFundOrder>? RedemptionRequest { get; private set; }
    public Request<FundOrderState, SwitchingFund, SwitchingFundOrder>? SwitchingRequest { get; private set; }

    public FundOrderStateMachine(ILogger<FundOrderStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);
        Event(() => SubscriptionFundRequestReceived, e =>
        {
            e.InsertOnInitial = true;
            e.CorrelateById(context => context.Message.CorrelationId);
            e.SetSagaFactory(context => EntityFactory.NewFundOrderState(context.Message));
        });
        Event(() => RedemptionFundRequestReceived, e =>
        {
            e.InsertOnInitial = true;
            e.CorrelateById(context => context.Message.CorrelationId);
            e.SetSagaFactory(context => EntityFactory.NewFundOrderState(context.Message));
        });
        Event(() => SwitchingFundRequestReceived, e =>
        {
            e.InsertOnInitial = true;
            e.CorrelateById(context => context.Message.CorrelationId);
            e.SetSagaFactory(context => EntityFactory.NewFundOrderState(context.Message));
        });
        Event(() => CreateSwitchInFundOrderReceived, e =>
        {
            e.InsertOnInitial = true;
            e.CorrelateById(context => context.Message.FundOrderState.CorrelationId);
            e.SetSagaFactory(context => context.Message.FundOrderState);
        });
        Event(() => CancelFundOrderReceived, e =>
        {
            e.CorrelateById(context => context.Message.CorrelationId);
        });
        Request(() => OrderNoGenerateRequest, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => SubscriptionRequest, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => RedemptionRequest, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => SwitchingRequest, x => { x.Timeout = TimeSpan.Zero; });

        State(() => OrderState.GeneratingOrderNo);
        State(() => OrderState.SendingOrderToBroker);
        State(() => OrderState.OrderPlaced);
        State(() => OrderState.PlaceOrderFailed);
        State(() => OrderState.OrderCancelled);

        Initially(
            When(SubscriptionFundRequestReceived)
                .Then(context =>
                {
                    context.Saga.Channel = context.Message.Channel;
                    context.Saga.OrderType = FundOrderType.Subscription;
                    context.Saga.OrderSide = OrderSide.Buy;
                    context.Saga.UnitHolderId = context.Message.UnitHolderId;
                    context.Saga.BankAccount = context.Message.BankAccount;
                    context.Saga.BankCode = context.Message.BankCode;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.PaymentType = context.Message.PaymentType;
                    context.Saga.ResponseAddress = context.ResponseAddress?.ToString()!;
                    context.Saga.RequestId = context.RequestId;
                })
                .TransitionTo(OrderState.GeneratingOrderNo)
                .SendMetric(Metrics.FundOrderReceived, ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
                .Request(OrderNoGenerateRequest, ctx => new GenerateOrderNo(ctx.Saga.CorrelationId, ctx.Saga.OrderSide, ctx.Message.Recurring)),
            When(RedemptionFundRequestReceived)
                .Then(context =>
                {
                    context.Saga.Channel = context.Message.Channel;
                    context.Saga.OrderType = FundOrderType.Redemption;
                    context.Saga.OrderSide = OrderSide.Sell;
                    context.Saga.BankAccount = context.Message.BankAccount;
                    context.Saga.BankCode = context.Message.BankCode;
                    context.Saga.UnitHolderId = context.Message.UnitHolderId;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Unit = context.Message.Unit;
                    context.Saga.RedemptionType = context.Message.RedemptionType;
                    context.Saga.SellAllUnit = context.Message.SellAllFlag;
                    context.Saga.ResponseAddress = context.ResponseAddress?.ToString()!;
                    context.Saga.RequestId = context.RequestId;
                })
                .TransitionTo(OrderState.GeneratingOrderNo)
                .SendMetric(Metrics.FundOrderReceived, ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
                .Request(OrderNoGenerateRequest, ctx => new GenerateOrderNo(ctx.Saga.CorrelationId, ctx.Saga.OrderSide)),
            When(SwitchingFundRequestReceived)
                .Then(context =>
                {
                    context.Saga.OrderType = FundOrderType.SwitchOut;
                    context.Saga.OrderSide = OrderSide.Switch;
                    context.Saga.Amount = context.Message.Amount;
                    context.Saga.Unit = context.Message.Unit;
                    context.Saga.UnitHolderId = context.Message.UnitHolderId;
                    context.Saga.CounterFundCode = context.Message.CounterFundCode;
                    context.Saga.RedemptionType = context.Message.RedemptionType;
                    context.Saga.SellAllUnit = context.Message.SellAllFlag;
                    context.Saga.ResponseAddress = context.ResponseAddress?.ToString()!;
                    context.Saga.RequestId = context.RequestId;
                })
                .TransitionTo(OrderState.GeneratingOrderNo)
                .SendMetric(Metrics.FundOrderReceived, ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
                .Request(OrderNoGenerateRequest, ctx => new GenerateOrderNo(ctx.Saga.CorrelationId, ctx.Saga.OrderSide)),
            When(CreateSwitchInFundOrderReceived)
                .SendPlacedFundOrderMetric()
                .TransitionTo(OrderState.OrderPlaced)
        );

        During(OrderState.GeneratingOrderNo,
            When(OrderNoGenerateRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.OrderNo = context.Message.OrderNo;
                })
                .SendMetric(Metrics.FundOrderCreated, ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
                .If(ctx => ctx.Saga.OrderSide == OrderSide.Buy, binder => binder.Request(SubscriptionRequest, ctx => new SubscriptFund
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    FundCode = ctx.Saga.FundCode,
                    UnitHolderId = ctx.Saga.UnitHolderId,
                    EffectiveDate = (DateOnly)ctx.Saga.EffectiveDate!,
                    BankAccount = ctx.Saga.BankAccount!,
                    BankCode = ctx.Saga.BankCode!,
                    TradingAccountNo = ctx.Saga.TradingAccountNo,
                    Amount = (decimal)ctx.Saga.Amount!,
                    Channel = ctx.Saga.Channel,
                    PaymentType = (PaymentType)ctx.Saga.PaymentType!,
                    OrderNo = ctx.Saga.OrderNo!
                }))
                .If(ctx => ctx.Saga.OrderSide == OrderSide.Sell, binder => binder.Request(RedemptionRequest, ctx => new RedeemFund
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    FundCode = ctx.Saga.FundCode,
                    EffectiveDate = (DateOnly)ctx.Saga.EffectiveDate!,
                    BankAccount = ctx.Saga.BankAccount!,
                    UnitHolderId = ctx.Saga.UnitHolderId!,
                    BankCode = ctx.Saga.BankCode!,
                    Channel = ctx.Saga.Channel,
                    TradingAccountNo = ctx.Saga.TradingAccountNo,
                    Amount = ctx.Saga.Amount,
                    Unit = ctx.Saga.Unit,
                    OrderNo = ctx.Saga.OrderNo!,
                    RedemptionType = (RedemptionType)ctx.Saga.RedemptionType!,
                    SellAllFlag = ctx.Saga.SellAllUnit,
                }))
                .If(ctx => ctx.Saga.OrderSide == OrderSide.Switch, binder => binder.Request(SwitchingRequest, ctx => new SwitchingFund
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    FundCode = ctx.Saga.FundCode,
                    EffectiveDate = (DateOnly)ctx.Saga.EffectiveDate!,
                    BankAccount = ctx.Saga.BankAccount!,
                    UnitHolderId = ctx.Saga.UnitHolderId!,
                    BankCode = ctx.Saga.BankCode!,
                    TradingAccountNo = ctx.Saga.TradingAccountNo,
                    Amount = ctx.Saga.Amount,
                    Unit = ctx.Saga.Unit,
                    Channel = ctx.Saga.Channel,
                    OrderNo = ctx.Saga.OrderNo!,
                    RedemptionType = (RedemptionType)ctx.Saga.RedemptionType!,
                    SellAllFlag = ctx.Saga.SellAllUnit,
                    CounterFundCode = ctx.Saga.CounterFundCode!,
                }))
                .TransitionTo(OrderState.SendingOrderToBroker)
        );

        During(OrderState.SendingOrderToBroker,
            When(SubscriptionRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.BrokerOrderId = context.Message.TransactionId;
                    context.Saga.UnitHolderId = context.Message.UnitHolderId;
                    context.Saga.SaleLicense = context.Message.SaleLicense;
                })
                .If(ctx => ctx.Message.NewUnitHolder, binder =>
                    binder.Publish(ctx => new CreateUnitHolder
                    {
                        CustomerCode = ctx.Saga.CustomerCode!,
                        TradingAccountNo = ctx.Saga.TradingAccountNo,
                        UnitHolderId = ctx.Message.UnitHolderId,
                        UnitHolderType = ctx.Message.UnitHolderType,
                        FundCode = ctx.Saga.FundCode
                    })
                )
                .Publish(ctx => new SubscriptionOrderPlaced
                {
                    CustCode = ctx.Saga.CustomerCode!,
                    TradingAccountNo = ctx.Saga.TradingAccountNo,
                    FundCode = ctx.Saga.FundCode,
                    UnitHolderId = ctx.Message.UnitHolderId,
                    TransactionId = ctx.Message.TransactionId,
                    SaOrderReferenceNo = ctx.Message.SaOrderReferenceNo,
                })
                .SendPlacedFundOrderMetric()
                .ThenAsync(async context =>
                {
                    await context.SendResponseAsync(context.Message);
                })
                .TransitionTo(OrderState.OrderPlaced),
            When(RedemptionRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.BrokerOrderId = context.Message.TransactionId;
                    context.Saga.PaymentType = context.Message.PaymentType;
                    context.Saga.UnitHolderId = context.Message.UnitHolderId;
                    context.Saga.SellAllUnit = context.Message.SellAllFlag;
                    context.Saga.SettlementDate = context.Message.SettlementDate;
                    context.Saga.SaleLicense = context.Message.SaleLicense;
                })
                .SendPlacedFundOrderMetric()
                .ThenAsync(async context =>
                {
                    await context.SendResponseAsync(context.Message);
                })
                .TransitionTo(OrderState.OrderPlaced),
            When(SwitchingRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.BrokerOrderId = context.Message.TransactionId;
                    context.Saga.UnitHolderId = context.Message.UnitHolderId;
                    context.Saga.SellAllUnit = context.Message.SellAllFlag;
                    context.Saga.SaleLicense = context.Message.SaleLicense;
                })
                .Publish(ctx => new SwitchOrderPlaced
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    OrderNo = ctx.Saga.OrderNo!,
                })
                .SendPlacedFundOrderMetric()
                .ThenAsync(async context =>
                {
                    await context.SendResponseAsync(context.Message);
                })
                .TransitionTo(OrderState.OrderPlaced),
            When(RedemptionRequest?.Faulted).SendPlaceOrderFailed(logger).TransitionTo(OrderState.PlaceOrderFailed),
            When(SubscriptionRequest?.Faulted).SendPlaceOrderFailed(logger).TransitionTo(OrderState.PlaceOrderFailed),
            When(SwitchingRequest?.Faulted).SendPlaceOrderFailed(logger).TransitionTo(OrderState.PlaceOrderFailed)
        );

        During(OrderState.OrderPlaced,
            When(CancelFundOrderReceived)
                .Then(context =>
                {
                    context.Saga.OrderStatus = FundOrderStatus.Cancelled;
                }) // No need to send Metric Data as cancel fund order will not be used in production
                .TransitionTo(OrderState.OrderCancelled)
        );
    }
}

public static class FundOrderExtension
{
    public static async Task SendResponseAsync<T, TMessage>(this BehaviorContext<FundOrderState, T> context, TMessage message)
        where T : class
        where TMessage : class
    {
        if (context.Saga is { ResponseAddress: not null, RequestId: not null })
        {
            var endpoint = await context.GetSendEndpoint(new Uri(context.Saga.ResponseAddress!));
            await endpoint.Send(
                message,
                ctx => ctx.RequestId = context.Saga.RequestId,
                cancellationToken: context.CancellationToken
            );
        }
    }

    public static EventActivityBinder<FundOrderState, TData> SendPlacedFundOrderMetric<TData>(
        this EventActivityBinder<FundOrderState, TData> binder)
        where TData : class
    {
        return binder
            .SendMetric(Metrics.FundOrderPlacedSuccess, ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
            .SendMetric(Metrics.FundOrderPlacedAmount, ctx => (double)(ctx.Saga.Amount ?? 0), ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
            .SendMetric(Metrics.FundOrderPlacedUnit, ctx => (double)(ctx.Saga.Unit ?? 0), ctx => MetricFactory.NewFundOrderTag(ctx.Saga));
    }

    public static EventActivityBinder<FundOrderState, Fault<TData>> SendPlaceOrderFailed<TData>(
        this EventActivityBinder<FundOrderState, Fault<TData>> binder, ILogger<FundOrderStateMachine> logger)
        where TData : class
    {
        return binder.
            Publish(ctx => new SendMetric(Metrics.FundOrderError, 1,
                MetricFactory.NewFundOrderTag(new MetricTags
                {
                    OrderSide = ctx.Saga.OrderSide,
                    OrderType = ctx.Saga.OrderType,
                    RedemptionType = ctx.Saga.RedemptionType,
                    ErrorCode = GetErrorCode(ctx.Message.Exceptions.FirstOrDefault())
                })))
            .SendMetric(Metrics.FundOrderPlacedFailed, ctx => MetricFactory.NewFundOrderTag(ctx.Saga))
            .ThenAsync(async context =>
            {
                context.Saga.FailedReason = context.Message.Exceptions.FirstOrDefault()?.Message;
                logger.LogError("Place {SagaOrderType} order failed with error: {Message}", context.Saga.OrderType,
                    context.Saga.FailedReason);

                if (context.Saga.ResponseAddress == null || context.Saga.RequestId == null) return;

                var exception = context.Message.Exceptions.FirstOrDefault();
                var errorCode = GetErrorCode(exception);
                if (exception != null)
                {
                    logger.LogError(
                        "Place {SagaOrderType} order failed with error: {Message} and exception info {ExceptionExceptionType} {ExceptionMessage} {ExceptionStackTrace}",
                        context.Saga.OrderType,
                        context.Saga.FailedReason,
                        exception.ExceptionType,
                        exception.Message,
                        exception.StackTrace);
                }

                var endpoint = await context.GetSendEndpoint(new Uri(context.Saga.ResponseAddress!));
                await endpoint.Send(
                    new PlaceOrderFailed(context.Saga.CorrelationId, errorCode, exception, context.Saga.OrderNo),
                    ctx => ctx.RequestId = context.Saga.RequestId
                );
            });
    }

    private static FundOrderErrorCode GetErrorCode(ExceptionInfo? exception)
    {
        var errorCode = FundOrderErrorCode.FOE201;

        if (exception?.ExceptionType.Equals(typeof(FundOrderException).ToString()) == true && exception.Data?["Code"] != null)
        {
            errorCode = Enum.TryParse((string)exception.Data?["Code"]!, out FundOrderErrorCode parse) ? parse : errorCode;
        }

        return errorCode;
    }
}
