using MassTransit;
using Microsoft.Extensions.Configuration;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.IntegrationEvents.Models;
namespace Pi.WalletService.Application.StateMachines.GlobalWalletManualAllocation;

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.GlobalWalletManualAllocationAggregate.GlobalManualAllocationState>
{
    public Event<GlobalManualAllocationRequestReceivedEvent>? GlobalManualAllocationRequestReceived { get; private set; }

    public Request<Domain.AggregatesModel.GlobalWalletManualAllocationAggregate.GlobalManualAllocationState, TransferUsdMoneyFromMainAccountToSubAccount, TransferUsdMoneyToSubSucceeded>?
        TransferUsdToSubRequest
    { get; private set; }

    public StateMachine(IConfiguration configuration)
    {
        var depositTransferFee = Convert.ToDecimal(configuration["Bank:Exante:DepositTransferFee"]);

        InstanceState(x => x.CurrentState);
        Event(
            () => GlobalManualAllocationRequestReceived,
            x => x.CorrelateById(ctx => ctx.Message.TicketId)
        );
        Request(() => TransferUsdToSubRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => GlobalManualAllocationState.Allocating);
        State(() => GlobalManualAllocationState.Failed);

        Initially(
            When(GlobalManualAllocationRequestReceived)
                .Then(ctx =>
                {
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress;
                    ctx.Saga.RequestType = ctx.Message.RequestType;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.TransactionNo = ctx.Message.TransactionNo;
                    ctx.Saga.GlobalAccount = ctx.Message.GlobalAccount;
                    ctx.Saga.Currency = ctx.Message.Currency;
                    ctx.Saga.Amount = ctx.Message.Amount;
                    ctx.Saga.TransactionId = ctx.Message.TransactionId;
                })
                .TransitionTo(GlobalManualAllocationState.Allocating)
        );

        BeforeEnter(GlobalManualAllocationState.Allocating,
            binder =>
                binder
                    .Publish(ctx => new GlobalManualAllocationProcessingEvent(ctx.Saga.TransactionId, ctx.Saga.TransactionNo))
                    .Request(
                        TransferUsdToSubRequest,
                        ctx =>
                            new TransferUsdMoneyFromMainAccountToSubAccount(
                                ctx.Saga.TransactionNo,
                                ctx.Saga.GlobalAccount,
                                ctx.Saga.Currency,
                                ctx.Saga.Amount,
                                depositTransferFee
                            )
                    )
                    .Then(ctx => { ctx.Saga.InitiateTransferAt = DateTime.UtcNow; })
        );
        During(GlobalManualAllocationState.Allocating,
            When(TransferUsdToSubRequest?.Completed)
                .Then(ctx => { ctx.Saga.CompletedTransferAt = DateTime.UtcNow; })
                .Publish(ctx =>
                    new GlobalManualAllocationSuccessEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.TransactionId,
                        ctx.Saga.TransactionNo,
                        ctx.Message.FromAccount,
                        ctx.Message.ToAccount,
                        ctx.Message.TransferAmount,
                        ctx.Message.TransferCurrency,
                        ctx.Message.RequestedTime,
                        ctx.Message.CompletedTime
                    )
                )
                .If(
                    ctx => !string.IsNullOrEmpty(ctx.Saga.ResponseAddress),
                    binder => binder.ThenAsync(async ctx =>
                    {
                        var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

                        await responseEndpoint.Send(
                            new GlobalManualAllocationRequestCompletedEvent(ctx.Saga.TransactionNo),
                            callback: context => context.RequestId = ctx.Saga.RequestId
                        );
                    })
                )
                .SendMetric(Metrics.GlobalManualAllocationSuccess, ctx => new MetricTags(null, null, null, null))
                .Finalize(),
            When(TransferUsdToSubRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Manual Allocation Failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .Publish(ctx =>
                    new GlobalManualAllocationFailedEvent(ctx.Saga.CorrelationId, ctx.Saga.RequestType, ctx.Saga.TransactionId, ctx.Saga.TransactionNo, ctx.Saga.FailedReason ?? "Manual Allocation Failed.")
                )
                .SendErrorResponse()
                .SendMetric(Metrics.GlobalManualAllocationFailed, ctx => new MetricTags(null, null, null, null))
                .TransitionTo(GlobalManualAllocationState.Failed)
        );
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : Domain.AggregatesModel.GlobalWalletManualAllocationAggregate.GlobalManualAllocationState
        where TData : class
    {
        return source
            .If(
                ctx => !string.IsNullOrEmpty(ctx.Saga.ResponseAddress),
                binder => binder.ThenAsync(async ctx =>
                {
                    var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

                    await responseEndpoint.Send(
                        new BusRequestFailed(ctx.Message.Exceptions.FirstOrDefault()),
                        callback: context => context.RequestId = ctx.Saga.RequestId
                    );
                })
            );
    }
}
