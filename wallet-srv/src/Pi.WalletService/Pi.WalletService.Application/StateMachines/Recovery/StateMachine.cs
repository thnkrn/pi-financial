using MassTransit;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.Recovery;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using RecoveryState = Pi.WalletService.IntegrationEvents.Models.RecoveryState;

namespace Pi.WalletService.Application.StateMachines.Recovery;

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.RecoveryAggregate.RecoveryState>
{
    public Event<InitRecoveryEvent>? InitRecoveryEvent { get; set; }

    public Request<Domain.AggregatesModel.RecoveryAggregate.RecoveryState, LogActivity, LogActivitySuccess>? LogSnapshot { get; set; }
    public Request<Domain.AggregatesModel.RecoveryAggregate.RecoveryState, InitiateRecoveryStateMachine, InitiateRecoveryStateMachineSuccess>? InitiateRecoveryStateMachine { get; set; }
    public Request<Domain.AggregatesModel.RecoveryAggregate.RecoveryState, TransferUsdMoneyFromMainAccountToSubAccountV2, TransferUsdMoneyToSubSucceeded>? TransferUsdToSubRequest { get; private set; }

    public StateMachine()
    {

        #region Setup

        InstanceState(x => x.CurrentState);
        Event(() => InitRecoveryEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));

        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferUsdToSubRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => InitiateRecoveryStateMachine, x => { x.Timeout = Timeout.InfiniteTimeSpan; });

        State(() => RecoveryState.RevertRequestReceived);
        State(() => RecoveryState.RevertTransferInitiate);
        State(() => RecoveryState.RevertingTransfer);
        State(() => RecoveryState.RevertTransferSuccess);
        State(() => RecoveryState.RevertTransferFailed);


        #endregion

        #region Activities

        // InitRecoveryEvent
        // condition: ODD withdraw failed
        // responsibility: setup recovery state machine
        Initially(
            When(InitRecoveryEvent)
                .Then(ctx =>
                    {
                        ctx.Saga.Product = ctx.Message.Product;
                        ctx.Saga.RequestId = ctx.RequestId;
                        ctx.Saga.TransactionType = ctx.Message.TransactionType;
                        ctx.Saga.CreatedAt = DateTime.Now;
                    }
                )
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .TransitionTo(RecoveryState.RevertRequestReceived)
        );

        During(RecoveryState.RevertRequestReceived,
            When(LogSnapshot?.Completed)
                .Request(InitiateRecoveryStateMachine, ctx =>
                    new InitiateRecoveryStateMachine(ctx.Saga.CorrelationId))
                .SendMetric(Metrics.RevertReceived, ctx => new MetricTags(ctx.Saga.Product, null, null, null))
                .TransitionTo(RecoveryState.RevertTransferInitiate)
        );

        // RevertTransferInitiate
        // condition: Recovery event received
        // responsibility: Initiate recovery state machine
        During(RecoveryState.RevertTransferInitiate,
            When(InitiateRecoveryStateMachine?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.GlobalAccount = ctx.Message.GlobalAccount!;
                    ctx.Saga.TransferCurrency = ctx.Message.TransferCurrency;
                    ctx.Saga.TransferAmount = ctx.Message.TransferAmount;
                    ctx.Saga.RequestId = ctx.RequestId;
                    ctx.Saga.CreatedAt = DateTime.Now;
                })
                .Request(
                    TransferUsdToSubRequest,
                    ctx =>
                        new TransferUsdMoneyFromMainAccountToSubAccountV2(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.GlobalAccount,
                            ctx.Saga.TransferCurrency!.Value,
                            ctx.Saga.TransferAmount!.Value,
                            0
                        )
                )
                .LogSnapshot()
                .TransitionTo(RecoveryState.RevertingTransfer),
            When(InitiateRecoveryStateMachine?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable to fetch Global Account data. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .SendMetric(Metrics.RevertFailed, ctx => new MetricTags(ctx.Saga.Product, null, null, null))
                .TransitionTo(RecoveryState.RevertTransferFailed)
            );

        // RevertingTransfer
        // condition: Recovery state machine initiated
        // responsibility: Revert global transfer
        During(RecoveryState.RevertingTransfer,
            When(TransferUsdToSubRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.TransferFromAccount = context.Message.FromAccount;
                    context.Saga.TransferToAccount = context.Message.ToAccount;
                    context.Saga.TransferRequestTime = context.Message.RequestedTime;
                    context.Saga.TransferAmount = context.Message.TransferAmount;
                    context.Saga.TransferCurrency = context.Message.TransferCurrency;
                    context.Saga.TransferCompleteTime = context.Message.CompletedTime;
                })
                .SendMetric(Metrics.RevertSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, null))
                .LogSnapshot()
                .Publish(ctx => new RecoverySuccess(ctx.Saga.CorrelationId))
                .TransitionTo(RecoveryState.RevertTransferSuccess),
            When(TransferUsdToSubRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Transfer Money Back From Main Account to Sub Account. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .SendMetric(Metrics.RevertFailed, ctx => new MetricTags(ctx.Saga.Product, null, null, null))
                .TransitionTo(RecoveryState.RevertTransferFailed)
        );

        #endregion

    }
}

internal static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : Domain.AggregatesModel.RecoveryAggregate.RecoveryState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this Domain.AggregatesModel.RecoveryAggregate.RecoveryState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            null,
            state.TransactionType,
            null,
            null,
            null,
            null,
            state.Product!.Value,
            null,
            "Recovery",
            state.CurrentState,
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
            state.TransferFromAccount,
            state.TransferAmount,
            state.TransferToAccount,
            state.TransferCurrency,
            state.TransferRequestTime,
            state.TransferCompleteTime,
            state.FailedReason,
            null,
            null
        );
    }
}
