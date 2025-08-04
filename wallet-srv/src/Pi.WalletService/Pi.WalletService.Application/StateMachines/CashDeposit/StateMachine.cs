using MassTransit;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.TfexAccountEvents;
using Pi.WalletService.IntegrationEvents.TradingAccountEvents;
using CashDepositStates = Pi.WalletService.IntegrationEvents.Models.CashDepositState;

namespace Pi.WalletService.Application.StateMachines.CashDeposit;

public class StateMachine : MassTransitStateMachine<CashDepositState>
{
    public Event<CashDepositRequestReceived>? CashDepositEventReceived { get; set; }
    public Event<CashDepositGatewayCallbackSuccessEvent>? CashDepositGatewayCallbackSuccessEvent { get; set; }
    public Event<CashDepositGatewayCallbackFailedEvent>? CashDepositGatewayCallbackFailedEvent { get; set; }
    public Event<ManualAllocateSbaTradingAccountBalanceSuccess>? ManualAllocateTradingAccountSuccessEvent { get; set; }
    public Event<DepositPaymentNotReceivedSpecific>? CashDepositPaymentNotReceived { get; set; }
    public Event<UpdateTransactionStatusSuccessEvent>? UpdateTransactionStatusSuccessEvent { get; set; }
    public Event<UpdateTransactionStatusFailedEvent>? UpdateTransactionStatusFailedEvent { get; set; }

    public Request<CashDepositState, LogDepositTransaction, LogDepositTransactionSuccess>? LogSnapshot { get; set; }
    public Request<CashDepositState, UpdateTradingAccountBalanceRequest, UpdateTradingAccountBalanceSuccess>? UpdateTradingAccountBalanceRequest { get; private set; }
    public Request<CashDepositState, UpdateTfexAccountBalanceRequest, UpdateTfexAccountBalanceSuccessEvent>? UpdateTfexAccountBalanceRequest { get; private set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => CashDepositEventReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => CashDepositGatewayCallbackSuccessEvent, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => CashDepositGatewayCallbackFailedEvent, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => ManualAllocateTradingAccountSuccessEvent, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => CashDepositPaymentNotReceived, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => UpdateTransactionStatusSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpdateTransactionStatusFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Request(() => LogSnapshot, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => UpdateTfexAccountBalanceRequest, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => UpdateTradingAccountBalanceRequest, x => { x.Timeout = TimeSpan.Zero; });

        State(() => CashDepositStates.Received);
        State(() => CashDepositStates.CashDepositWaitingForGateway);
        State(() => CashDepositStates.CashDepositTradingPlatformUpdating);
        State(() => CashDepositStates.CashDepositWaitingForTradingPlatform);
        State(() => CashDepositStates.TradingPlatformCashDepositFailed);
        State(() => CashDepositStates.CashDepositPaymentNotReceived);
        State(() => CashDepositStates.CashDepositCompleted);
        State(() => CashDepositStates.CashDepositFailed);
        State(() => CashDepositStates.TfexCashDepositFailed);

        // Start of Cash Deposit after Deposit Success Event received
        Initially(
            When(CashDepositEventReceived, ctx => ctx.Message.Product != Product.GlobalEquities.ToString())
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.TransactionNo = ctx.Message.TransactionNo;
                    ctx.Saga.Purpose = ctx.Message.Purpose;
                    ctx.Saga.CreatedAt = DateTime.Now;
                    ctx.Saga.Product = Enum.Parse<Product>(ctx.Message.Product);
                    ctx.Saga.RequestedAmount = ctx.Message.Amount;
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.PaymentReceivedDateTime;
                    ctx.Saga.Channel = ctx.Message.Channel;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.AccountCode = ctx.Message.AccountCode!;
                    ctx.Saga.BankName = ctx.Message.BankName;
                })
                .Request(LogSnapshot, ctx => new LogDepositTransaction(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.CashDepositReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(CashDepositStates.Received),
            When(CashDepositPaymentNotReceived)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .SendMetric(Metrics.DepositCancelled, ctx => new MetricTags(null, null, TagFailedReason.PaymentNotReceived, null))
                .TransitionTo(CashDepositStates.CashDepositPaymentNotReceived)
        );

        // Check if transaction exists and proceed to calling Freewill gateway
        During(CashDepositStates.Received,
            When(LogSnapshot?.Completed, ctx => !string.IsNullOrEmpty(ctx.Saga.TransactionNo))
                .Request(UpdateTradingAccountBalanceRequest,
                    ctx => new UpdateTradingAccountBalanceRequest(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.RequestedAmount,
                        ctx.Saga.CustomerCode,
                        ctx.Saga.AccountCode,
                        ctx.Saga.BankName!,
                        ctx.Saga.Channel,
                        TransactionType.Deposit))
                .TransitionTo(CashDepositStates.CashDepositTradingPlatformUpdating)
        );

        During(CashDepositStates.CashDepositTradingPlatformUpdating,
            When(UpdateTradingAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(CashDepositStates.CashDepositWaitingForGateway),
            When(UpdateTradingAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed. Reasons: {string.Join(",", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashDepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill, null))
                .TransitionTo(CashDepositStates.CashDepositFailed)
        );

        // Wait for callback from Freewill gateway and call SetTrade for TFex accounts
        During(CashDepositStates.CashDepositWaitingForGateway,
            When(CashDepositGatewayCallbackSuccessEvent)
                .LogSnapshot()
                .IfElse(
                    ctx => ctx.Saga.Product == Product.Derivatives,
                    binder => binder
                        .Request(
                            UpdateTfexAccountBalanceRequest,
                            ctx => new UpdateTfexAccountBalanceRequest(
                                ctx.Saga.UserId,
                                ctx.Saga.TransactionNo!,
                                ctx.Saga.AccountCode,
                                ctx.Saga.RequestedAmount,
                                TransactionType.Deposit))
                        .TransitionTo(CashDepositStates.CashDepositWaitingForTradingPlatform),
                    binder => binder
                        .Publish(ctx =>
                            new CashDepositSuccessEvent(
                                ctx.Saga.CorrelationId,
                                ctx.Saga.UserId,
                                ctx.Saga.TransactionNo!,
                                ctx.Saga.PaymentReceivedDateTime! ?? DateTime.Now,
                                ctx.Saga.Product.ToString(),
                                ctx.Saga.RequestedAmount
                            ))
                        .SendMetric(Metrics.CashDepositSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                        .SendMetric(Metrics.CashDepositAmount, ctx => (double)ctx.Saga.RequestedAmount, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                        .TransitionTo(CashDepositStates.CashDepositCompleted)
                ),
            When(CashDepositGatewayCallbackFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed. ResultCode: " +
                        $"{string.Join(", ", ctx.Message.ResultCode)}, Reason: {ctx.Message.Reason}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashDepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback, null))
                .TransitionTo(CashDepositStates.CashDepositFailed)
        );

        // Wait for response from SetTrade and complete cash deposit
        During(CashDepositStates.CashDepositWaitingForTradingPlatform,
            When(UpdateTfexAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .Publish(ctx =>
                    new CashDepositSuccessEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.PaymentReceivedDateTime! ?? DateTime.Now,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.RequestedAmount
                    ))
                .SendMetric(Metrics.CashDepositSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .SendMetric(Metrics.CashDepositAmount, ctx => (double)ctx.Saga.RequestedAmount, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(CashDepositStates.CashDepositCompleted),
            When(UpdateTfexAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"SetTrade update failed. Reasons: {string.Join(",", ctx.Message.Exceptions.Select(e => e.Message))}, Verify MT4 User";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashDepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Tfex, null))
                .TransitionTo(CashDepositStates.TfexCashDepositFailed)
        );

        During(CashDepositStates.CashDepositFailed,
            When(ManualAllocateTradingAccountSuccessEvent)
                .TransitionTo(CashDepositStates.CashDepositWaitingForGateway));

        DuringAny(
            When(UpdateTransactionStatusSuccessEvent)
                .LogSnapshot()
                .SendMetric(Metrics.CashDepositSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .Then(ctx => ctx.Saga.FailedReason = "Success by operation")
                .TransitionTo(CashDepositStates.CashDepositCompleted),
            When(UpdateTransactionStatusFailedEvent)
                .LogSnapshot()
                .SendMetric(Metrics.CashDepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.WithdrawFailed, null))
                .Then(ctx => ctx.Saga.FailedReason = "Failed by operation")
                .TransitionTo(CashDepositStates.CashDepositFailed));
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : CashDepositState
        where TData : class
    {
        return source.Publish(
            ctx => new LogDepositTransaction(ctx.Saga.ToSnapshot())
        );
    }

    public static DepositTransactionSnapshot ToSnapshot(this CashDepositState state)
    {
        return new DepositTransactionSnapshot(
            state.CorrelationId,
            state.UserId,
            state.CurrentState ?? string.Empty,
            state.CustomerCode,
            state.TransactionNo ?? string.Empty,
            state.RequestedAmount,
            state.AccountCode,
            state.Channel,
            state.Product,
            state.Purpose,
            null,
            null,
            null,
            null,
            null,
            state.PaymentReceivedDateTime,
            null,
            null,
            null,
            state.BankName,
            null,
            null,
            state.FailedReason,
            null
        );
    }
}