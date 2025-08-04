using MassTransit;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.Events.UpBack;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.UpBackEvents;
using NonGeUpBackState = Pi.WalletService.IntegrationEvents.Models.UpBackState;

namespace Pi.WalletService.Application.StateMachines.UpBack;

public class StateMachine : MassTransitStateMachine<UpBackState>
{
    public Event<UpBackRequest>? UpBackRequestReceived { get; set; }
    public Event<GatewayCallbackSuccessEvent>? GatewayCallbackSuccessEvent { get; set; }
    public Event<GatewayCallbackFailedEvent>? GatewayCallbackFailedEvent { get; set; }
    public Event<ManualAllocateSbaTradingAccountBalanceSuccessV2>? ManualAllocateSbaAccountBalanceSuccess { get; set; }
    public Event<ManualAllocateSetTradeAccountBalanceSuccess>? ManualAllocateSetTradeAccountBalanceSuccess { get; set; }
    public Request<UpBackState, LogActivity, LogActivitySuccess>? LogSnapshot { get; set; }

    public Request<UpBackState, UpdateTradingAccountBalanceV2Request, GatewayUpdateAccountBalanceSuccessEvent>?
        UpdateGatewayAccountBalanceRequest
    { get; private set; }

    public Request<UpBackState, UpdateTfexAccountBalanceV2Request, TradingPlatformCallbackSuccessEvent>?
        UpdateTradingPlatformAccountBalanceRequest
    { get; private set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => UpBackRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GatewayCallbackSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => GatewayCallbackFailedEvent, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => ManualAllocateSbaAccountBalanceSuccess, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => ManualAllocateSetTradeAccountBalanceSuccess,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Request(() => LogSnapshot, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => UpdateTradingPlatformAccountBalanceRequest, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => UpdateGatewayAccountBalanceRequest, x => { x.Timeout = TimeSpan.Zero; });
        State(() => NonGeUpBackState.Initial);
        State(() => NonGeUpBackState.DepositReceived);
        State(() => NonGeUpBackState.WithdrawReceived);
        State(() => NonGeUpBackState.DepositUpdatingAccountBalance);
        State(() => NonGeUpBackState.DepositWaitingForGateway);
        State(() => NonGeUpBackState.DepositUpdatingTradingPlatform);
        State(() => NonGeUpBackState.WithdrawUpdatingAccountBalance);
        State(() => NonGeUpBackState.WithdrawWaitingForGateway);
        State(() => NonGeUpBackState.WithdrawUpdatingTradingPlatform);
        State(() => NonGeUpBackState.UpBackCompleted);
        State(() => NonGeUpBackState.UpBackFailed);
        State(() => NonGeUpBackState.UpBackFailedRequireActionRevert);
        State(() => NonGeUpBackState.UpBackFailedRequireActionSba);
        State(() => NonGeUpBackState.UpBackFailedRequireActionSetTrade);

        // Initial UpBack
        // responsibility: receive request and process based on transaction type (deposit/withdraw)
        Initially(
            When(UpBackRequestReceived)
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.UpBackReceived,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, ctx.Saga.TransactionType))
                .IfElse(ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                    binder => binder
                        .TransitionTo(NonGeUpBackState.DepositReceived),
                    binder => binder
                        .TransitionTo(NonGeUpBackState.WithdrawReceived)
                )
        );

        #region Deposit

        // DepositReceived
        // condition: log snapshot is completed
        // responsibility:
        //   - if ATS, updating trading platform for TFEX or transition to completed for Non-TFEX
        //   - otherwise, updating account balance
        During(NonGeUpBackState.DepositReceived,
            When(LogSnapshot?.Completed)
                .IfElse(ctx => ctx.Saga.Channel != Channel.ATS,
                    binder => binder.TransitionTo(NonGeUpBackState.DepositUpdatingAccountBalance),
                    binder => binder
                        .IfElse(ctx => ctx.Saga.Product == Product.Derivatives,
                            b => b.TransitionTo(NonGeUpBackState.DepositUpdatingTradingPlatform),
                            b => b.TransitionTo(NonGeUpBackState.UpBackCompleted)))
        );

        // DepositUpdatingAccountBalance
        // condition: update gateway account balance request is completed
        // responsibility: if request success move to next state, if request failed then publish failed event
        BeforeEnter(NonGeUpBackState.DepositUpdatingAccountBalance,
            binder => binder
                .Request(UpdateGatewayAccountBalanceRequest,
                    ctx => new UpdateTradingAccountBalanceV2Request(ctx.Saga.CorrelationId, TransactionType.Deposit))
        );
        During(NonGeUpBackState.DepositUpdatingAccountBalance,
            When(UpdateGatewayAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(NonGeUpBackState.DepositWaitingForGateway),
            When(UpdateGatewayAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed with error: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.UpBackFailedRequiredActionSba,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill,
                        TransactionType.Deposit))
                .TransitionTo(NonGeUpBackState.UpBackFailedRequireActionSba)
        );

        // DepositWaitingForGateway
        // condition: gateway callback success or failed
        // responsibility:
        //   - if gateway callback success and product is TFEX then request update trading platform account balance
        //   - if gateway callback success and product is not TFEX then publish success event
        //   - if gateway callback failed then publish failed event
        During(NonGeUpBackState.DepositWaitingForGateway,
            When(GatewayCallbackSuccessEvent)
                .LogSnapshot()
                .IfElse(
                    ctx => ctx.Saga.Product == Product.Derivatives,
                    // TFEX
                    binder => binder.TransitionTo(NonGeUpBackState.DepositUpdatingTradingPlatform),
                    // Non-TFEX
                    binder => binder
                        .SendMetric(Metrics.UpBackSuccess,
                            ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
                        .TransitionTo(NonGeUpBackState.UpBackCompleted)
                ),
            When(GatewayCallbackFailedEvent)
                .Then(ctx =>
                {
                    var errorMessage = FreewillUtils.GetResultMessage(ctx.Message.ResultCode);
                    ctx.Saga.FailedReason = $"Freewill update failed. Result Code {ctx.Message.ResultCode}" +
                                            $"{(string.IsNullOrEmpty(errorMessage) ? "" : $" : {errorMessage}")}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.UpBackFailedRequiredActionSba,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback,
                        TransactionType.Deposit))
                .TransitionTo(NonGeUpBackState.UpBackFailedRequireActionSba)
        );


        // DepositWaitingForTradingPlatform
        // condition: update trading platform account balance request is completed
        // responsibility: if request success then publish success event, if request failed then publish failed event
        BeforeEnter(NonGeUpBackState.DepositUpdatingTradingPlatform,
            binder => binder
                .Request(UpdateTradingPlatformAccountBalanceRequest,
                    ctx => new UpdateTfexAccountBalanceV2Request(ctx.Saga.CorrelationId, TransactionType.Deposit))
        );
        During(NonGeUpBackState.DepositUpdatingTradingPlatform,
            When(UpdateTradingPlatformAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .SendMetric(Metrics.UpBackSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
                .TransitionTo(NonGeUpBackState.UpBackCompleted),
            When(UpdateTradingPlatformAccountBalanceRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"SetTrade Update Failed, Verify MT4 User."; })
                .LogSnapshot()
                .SendMetric(Metrics.UpBackFailedRequiredActionSetTrade,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Tfex,
                        TransactionType.Deposit))
                .TransitionTo(NonGeUpBackState.UpBackFailedRequireActionSetTrade)
        );

        # endregion

        # region Withdraw

        // WithdrawReceived
        // condition: log snapshot is completed
        // responsibility:
        //   - if TFEX then request update trading platform account balance
        //   - if not TFEX (OnlineViaKKP) then request update gateway account balance
        //   - if not TFEX (ATS) then transition to completed
        During(NonGeUpBackState.WithdrawReceived,
            When(LogSnapshot?.Completed)
                .LogSnapshot()
                .IfElse(
                    ctx => ctx.Saga.Product == Product.Derivatives,
                    // TFEX
                    binder => binder
                        .Request(
                            UpdateTradingPlatformAccountBalanceRequest,
                            ctx =>
                                new UpdateTfexAccountBalanceV2Request(
                                    ctx.Saga.CorrelationId,
                                    TransactionType.Withdraw
                                )
                        )
                        .TransitionTo(NonGeUpBackState.WithdrawUpdatingTradingPlatform),
                    // Non-TFEX
                    binder => binder
                        .IfElse(c => c.Saga.Channel != Channel.ATS,
                            b => b.Request(
                                UpdateGatewayAccountBalanceRequest,
                                ctx =>
                                    new UpdateTradingAccountBalanceV2Request(
                                        ctx.Saga.CorrelationId,
                                        TransactionType.Withdraw
                                    )
                            )
                            .TransitionTo(NonGeUpBackState.WithdrawUpdatingAccountBalance),
                            b => b.TransitionTo(NonGeUpBackState.UpBackCompleted)
                        )
                )
        );

        // WithdrawWaitingForTradingPlatform
        // condition: update trading platform account balance request is completed
        // responsibility: if request success
        //      - ATS - transaction to completed
        //      - other - request to update account balance (FreeWill)
        //  if request failed then publish failed event
        During(NonGeUpBackState.WithdrawUpdatingTradingPlatform,
            When(UpdateTradingPlatformAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Channel != Channel.ATS,
                    binder => binder
                        .Request(
                            UpdateGatewayAccountBalanceRequest,
                            ctx =>
                                new UpdateTradingAccountBalanceV2Request(
                                    ctx.Saga.CorrelationId,
                                    TransactionType.Withdraw
                                )
                        )
                        .TransitionTo(NonGeUpBackState.WithdrawUpdatingAccountBalance),
                    binder => binder.TransitionTo(NonGeUpBackState.UpBackCompleted)),
            When(UpdateTradingPlatformAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"SetTrade Update Failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.UpBackFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Tfex,
                        TransactionType.Withdraw))
                .TransitionTo(NonGeUpBackState.UpBackFailed)
        );

        // WithdrawUpdatingAccountBalance
        // condition: update gateway account balance request is completed
        // responsibility: if request success move to next state, if request failed then publish failed event
        During(NonGeUpBackState.WithdrawUpdatingAccountBalance,
            When(UpdateGatewayAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(NonGeUpBackState.WithdrawWaitingForGateway),
            When(UpdateGatewayAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed with error: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.UpBackFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill,
                        TransactionType.Withdraw))
                .IfElse(ctx => ctx.Saga.Product == Product.Derivatives,
                    // Derivatives: Need to revert TFEX.
                    binder => binder.TransitionTo(NonGeUpBackState.UpBackFailedRequireActionRevert),
                    // Others: Can be failed, not making any transaction yet. 
                    binder => binder.TransitionTo(NonGeUpBackState.UpBackFailed))
        );

        // WithdrawWaitingForGateway
        // condition: gateway callback success or failed
        // responsibility: if gateway callback success then publish success event, if gateway callback failed then publish failed event
        During(NonGeUpBackState.WithdrawWaitingForGateway,
            When(GatewayCallbackSuccessEvent)
                .LogSnapshot()
                .TransitionTo(NonGeUpBackState.UpBackCompleted),
            When(GatewayCallbackFailedEvent)
                .Then(ctx =>
                {
                    var errorMessage = FreewillUtils.GetResultMessage(ctx.Message.ResultCode);
                    ctx.Saga.FailedReason = $"Freewill update failed. Result Code {ctx.Message.ResultCode}" +
                                            $"{(string.IsNullOrEmpty(errorMessage) ? "" : $" : {errorMessage}")}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.UpBackFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback,
                        TransactionType.Withdraw))
                .TransitionTo(NonGeUpBackState.UpBackFailedRequireActionRevert)
        );

        #endregion

        #region Completed

        BeforeEnter(NonGeUpBackState.UpBackCompleted,
            binder => binder
                .Publish(ctx =>
                    new UpBackSuccess(
                        ctx.Saga.CorrelationId
                    )
                )
        );

        # endregion

        # region Failed

        BeforeEnter(NonGeUpBackState.UpBackFailed,
            binder => binder
                .Publish(ctx =>
                    new UpBackFailed(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.FailedReason!
                    )
                )
        );

        # endregion

        # region Action

        During(NonGeUpBackState.UpBackFailedRequireActionSba,
            When(ManualAllocateSbaAccountBalanceSuccess)
                .TransitionTo(NonGeUpBackState.DepositWaitingForGateway));

        During(NonGeUpBackState.UpBackFailedRequireActionSetTrade,
            When(ManualAllocateSetTradeAccountBalanceSuccess)
                .SendMetric(Metrics.UpBackSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit)
                )
                .TransitionTo(NonGeUpBackState.UpBackCompleted)
        );

        # endregion
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : UpBackState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this UpBackState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            null,
            state.TransactionType,
            null,
            null,
            null,
            state.Channel,
            state.Product,
            null,
            "UpBack",
            state.CurrentState!,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            state.FailedReason,
            null,
            null
        );
    }
}