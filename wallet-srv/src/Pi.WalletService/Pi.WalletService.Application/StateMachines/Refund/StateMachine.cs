using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Commands.Refund;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Requests;
using Pi.WalletService.IntegrationEvents.Responses;

namespace Pi.WalletService.Application.StateMachines.Refund;

public class StateMachine : MassTransitStateMachine<RefundState>
{
    public Event<RefundRequest>? RefundRequest { get; private set; }
    public Request<RefundState, RefundInfoRequest, RefundInfoResponse>? RefundInfoRequest { get; private set; }
    public Request<RefundState, KkpWithdrawRequest, WithdrawOddSucceed>? KkpWithdrawRequest { get; private set; }

    public StateMachine(ILogger<StateMachine> logger)
    {

        #region Setup

        State(() => IntegrationEvents.Models.RefundState.Received);
        State(() => IntegrationEvents.Models.RefundState.Refunding);
        State(() => IntegrationEvents.Models.RefundState.RefundSucceed);
        State(() => IntegrationEvents.Models.RefundState.RefundFailed);

        InstanceState(x => x.CurrentState);
        Event(() => RefundRequest,
            x =>
            {
                x.CorrelateBy((state, ctx) => state.DepositTransactionNo == ctx.Message.DepositTransactionNo && state.CurrentState != IntegrationEvents.Models.RefundState.RefundFailed!.Name);
            });
        Request(() => RefundInfoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => KkpWithdrawRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });

        #endregion

        #region Activities

        Initially(When(RefundRequest)
            .Then(ctx =>
            {
                ctx.Saga.ResponseAddress = ctx.ResponseAddress?.ToString()!;
                ctx.Saga.RequestId = ctx.RequestId;
                ctx.Saga.DepositTransactionNo = ctx.Message.DepositTransactionNo;
                ctx.Saga.CreatedAt = DateTime.Now;
            })
            .Request(
                RefundInfoRequest,
                ctx => new RefundInfoRequest(
                    ctx.Saga.CorrelationId,
                    ctx.Message.DepositTransactionNo
                )
            )
            .Publish(ctx => new RefundingDeposit(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.DepositTransactionNo!,
                    ctx.Saga.CustomerCode!,
                    ctx.Saga.Amount
                )
            )
            .SendMetric(Metrics.RefundReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
            .TransitionTo(IntegrationEvents.Models.RefundState.Received)
        );

        During(IntegrationEvents.Models.RefundState.Received,
            When(RefundInfoRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.Amount = ctx.Message.RefundAmount;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.BankName = ctx.Message.BankName;
                    ctx.Saga.BankCode = ctx.Message.BankCode;
                    ctx.Saga.BankFee = ctx.Message.BankFee;
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.AccountCode = ctx.Message.AccountCode;
                    ctx.Saga.Product = ctx.Message.Product;
                    ctx.Saga.Channel = Channel.OnlineViaKKP;
                })
                .Request(KkpWithdrawRequest, ctx => new KkpWithdrawRequest(ctx.Saga.AccountCode!, ctx.Saga.Amount, ctx.Saga.BankAccountNo!, ctx.Saga.BankCode!, ctx.Saga.Product, ctx.Saga.DepositTransactionNo))
                .TransitionTo(IntegrationEvents.Models.RefundState.Refunding)
                .ThenAsync(async ctx =>
                {
                    if (ctx.Saga.ResponseAddress != null)
                    {
                        var endpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress!));
                        await endpoint.Send(
                            new RefundingResponse(ctx.Saga.CorrelationId, ctx.Saga.DepositTransactionNo),
                            q => q.RequestId = ctx.Saga.RequestId
                        );
                    }
                }),
            When(RefundInfoRequest?.Faulted)
                .Publish(ctx => { return new RefundFailedEvent(ctx.Saga.CorrelationId, ctx.Saga.DepositTransactionNo!); })
                .Then(ctx =>
                {
                    var exceptionMsg = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));
                    var errorMsg = $"RefundInfoRequest Faulted With: {exceptionMsg}";
                    ctx.Saga.FailedReason = errorMsg;
                    logger.LogError("{ErrorMsg}", errorMsg);
                })
                .SendMetric(Metrics.RefundFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Request, null))
                .TransitionTo(IntegrationEvents.Models.RefundState.RefundFailed)
                .ThenAsync(async ctx =>
                {
                    if (ctx.Saga.ResponseAddress != null)
                    {
                        var errorMsg = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));

                        var endpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress!));
                        await endpoint.Send(
                            new InvalidRefundResponse(ctx.Saga.CorrelationId, ctx.Saga.DepositTransactionNo!, errorMsg),
                            q => q.RequestId = ctx.Saga.RequestId
                        );
                    }
                })
        );

        During(IntegrationEvents.Models.RefundState.Refunding,
            When(KkpWithdrawRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.TransactionNo = ctx.Message.TransactionNo;
                    ctx.Saga.RefundedAt = DateTime.Now;
                })
                .Publish(ctx => new RefundSucceedEvent(ctx.Saga.CorrelationId, ctx.Saga.DepositTransactionNo!, ctx.Saga.TransactionNo!, ctx.Saga.Amount))
                .SendMetric(Metrics.RefundSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(IntegrationEvents.Models.RefundState.RefundSucceed),
            When(KkpWithdrawRequest?.Faulted)
                .Publish(ctx => new RefundFailedEvent(ctx.Saga.CorrelationId, ctx.Saga.DepositTransactionNo!))
                .Then(ctx =>
                {
                    var exceptionMsg = string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message));
                    var errorMsg = $"KkpWithdrawRequest Faulted With: {exceptionMsg}";
                    ctx.Saga.FailedReason = errorMsg;
                    logger.LogError("{ErrorMsg}", errorMsg);
                })
                .SendMetric(Metrics.RefundFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Kkp, null))
                .TransitionTo(IntegrationEvents.Models.RefundState.RefundFailed)
        );

        #endregion

    }
}
