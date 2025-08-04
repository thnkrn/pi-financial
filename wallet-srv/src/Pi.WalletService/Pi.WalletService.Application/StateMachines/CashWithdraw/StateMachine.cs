using MassTransit;
using Microsoft.Extensions.Configuration;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.TfexAccountEvents;
using Pi.WalletService.IntegrationEvents.TradingAccountEvents;
using CashWithdrawStates = Pi.WalletService.IntegrationEvents.Models.CashWithdrawState;

namespace Pi.WalletService.Application.StateMachines.CashWithdraw;

public class StateMachine : MassTransitStateMachine<CashWithdrawState>
{
    public Event<CashWithdrawRequestReceived>? CashWithdrawReceived { get; set; }
    public Event<WithdrawOtpValidationSuccess>? OtpValidationSuccess { get; set; }
    public Event<CashWithdrawGatewayCallbackSuccessEvent>? CashWithdrawGatewayCallbackSuccessEvent { get; set; }
    public Event<CashWithdrawGatewayCallbackFailedEvent>? CashWithdrawGatewayCallbackFailedEvent { get; set; }
    public Event<WithdrawSuccessEvent>? WithdrawSuccess { get; set; }
    public Event<NonGlobalWithdrawFailedEvent>? WithdrawFailed { get; set; }
    public Event<CashDepositGatewayCallbackSuccessEvent>? CashDepositGatewayCallbackSuccessEvent { get; set; }
    public Event<CashDepositGatewayCallbackFailedEvent>? CashDepositGatewayCallbackFailedEvent { get; set; }

    public Request<CashWithdrawState, LogWithdrawTransaction, LogWithdrawTransactionSuccess>? LogSnapshot { get; set; }
    public Request<CashWithdrawState, GenerateTransactionNo, TransactionNoGenerated>? GenerateTransactionNoRequest
    {
        get;
        set;
    }
    public Request<CashWithdrawState, UpdateTradingAccountBalanceRequest, UpdateTradingAccountBalanceSuccess>?
        UpdateTradingAccountBalanceRequest
    { get; private set; }

    public Request<CashWithdrawState, UpdateTfexAccountBalanceRequest, UpdateTfexAccountBalanceSuccessEvent>?
        UpdateTfexAccountBalanceRequest
    { get; private set; }

    public StateMachine(IConfiguration configuration)
    {
        var kkpBankFee = Convert.ToDecimal(configuration["Bank:KKP:WithdrawFee"]);

        InstanceState(x => x.CurrentState);

        Event(() => CashWithdrawReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => OtpValidationSuccess,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => CashWithdrawGatewayCallbackSuccessEvent,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => CashWithdrawGatewayCallbackFailedEvent,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => WithdrawSuccess,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => WithdrawFailed,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => CashDepositGatewayCallbackSuccessEvent,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => CashDepositGatewayCallbackFailedEvent,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));

        Request(() => LogSnapshot, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => GenerateTransactionNoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => UpdateTradingAccountBalanceRequest, x => { x.Timeout = TimeSpan.Zero; });
        Request(() => UpdateTfexAccountBalanceRequest, x => { x.Timeout = TimeSpan.Zero; });

        State(() => CashWithdrawStates.Received);
        State(() => CashWithdrawStates.TransactionNoGenerating);
        State(() => CashWithdrawStates.CashWithdrawWaitingForOtpValidation);
        State(() => CashWithdrawStates.CashWithdrawWaitingForTradingPlatform);
        State(() => CashWithdrawStates.CashWithdrawTradingPlatformUpdating);
        State(() => CashWithdrawStates.CashWithdrawWaitingForTFexPlatform);
        State(() => CashWithdrawStates.WithdrawalProcessing);
        State(() => CashWithdrawStates.TransferRequestFailed);
        State(() => CashWithdrawStates.RevertTransferFailed);
        State(() => CashWithdrawStates.RevertTfexTransfer);
        State(() => CashWithdrawStates.RevertPlatformTransfer);
        State(() => CashWithdrawStates.RevertWaitingForPlatformCallback);
        State(() => CashWithdrawStates.RevertTransferSuccess);
        State(() => CashWithdrawStates.WithdrawalFailed);
        State(() => CashWithdrawStates.CashWithdrawOtpValidationNotReceived);

        // Start of Cash Withdraw
        Initially(
            When(CashWithdrawReceived, ctx => ctx.Message.Product != Product.GlobalEquities)
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.CreatedAt = DateTime.Now;
                    ctx.Saga.Product = ctx.Message.Product;
                    ctx.Saga.RequestedAmount = ctx.Message.Amount;
                    ctx.Saga.Channel = ctx.Message.Channel;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.AccountCode = ctx.Message.AccountCode;
                    ctx.Saga.DeviceId = ctx.Message.DeviceId;
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress!;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.BankName = ctx.Message.BankName.ToUpper();
                    ctx.Saga.BankCode = ctx.Message.BankCode;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                })
                .Request(LogSnapshot, ctx => new LogWithdrawTransaction(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.CashWithdrawReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(CashWithdrawStates.Received)
        );

        // Log snapshot
        During(CashWithdrawStates.Received,
            When(LogSnapshot?.Completed)
                .LogSnapshot()
                .TransitionTo(CashWithdrawStates.TransactionNoGenerating)
        );

        BeforeEnter(CashWithdrawStates.TransactionNoGenerating,
            binder => binder.Request(
                GenerateTransactionNoRequest,
                    ctx => new GenerateTransactionNo(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.Product,
                        ctx.Saga.Channel,
                        TransactionType.Withdraw,
                        false
                    )
                )
        );

        // Generating transaction number
        During(CashWithdrawStates.TransactionNoGenerating,
            When(GenerateTransactionNoRequest?.Completed)
                .Then(ctx => { ctx.Saga.TransactionNo = ctx.Message.TransactionNo; })
                .LogSnapshot()
                .TransitionTo(CashWithdrawStates.CashWithdrawWaitingForOtpValidation),
            When(GenerateTransactionNoRequest?.Faulted)
                .SendMetric(Metrics.CashWithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.GenerateTransactionNo, null))
                .SendErrorResponse()
        );


        // Transition to withdraw state machine for OTP validation
        BeforeEnter(CashWithdrawStates.CashWithdrawWaitingForOtpValidation,
            binder =>
                binder.Publish(ctx => new WithdrawRequestReceived(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.AccountCode,
                        ctx.Saga.CustomerCode,
                        ctx.Saga.DeviceId,
                        ctx.Saga.Product,
                        ctx.Saga.TransactionNo,
                        ctx.Saga.BankAccountNo,
                        ctx.Saga.BankCode,
                        ctx.Saga.BankName,
                        ctx.Saga.RequestId,
                        ctx.Saga.ResponseAddress
                    )
                )
        );

        // On successful OTP validation call SetTrade or Freewill based on product
        During(CashWithdrawStates.CashWithdrawWaitingForOtpValidation,
            When(OtpValidationSuccess)
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
                                TransactionType.Withdraw))
                        .TransitionTo(CashWithdrawStates.CashWithdrawWaitingForTFexPlatform),
                    binder => binder
                        .TransitionTo(CashWithdrawStates.CashWithdrawTradingPlatformUpdating)
                ),
            When(WithdrawFailed)
                .SendMetric(Metrics.CashWithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.WithdrawFailed, null))
                .TransitionTo(CashWithdrawStates.TransferRequestFailed)
        );

        // Wait for response from SetTrade and proceed to calling Freewill
        During(CashWithdrawStates.CashWithdrawWaitingForTFexPlatform,
            When(UpdateTfexAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(CashWithdrawStates.CashWithdrawTradingPlatformUpdating),
            When(UpdateTfexAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"SetTrade update failed. Reasons: {string.Join(",", ctx.Message.Exceptions.Select(e => e.Message))}, , Verify MT4 User";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Tfex, null))
                .TransitionTo(CashWithdrawStates.TransferRequestFailed)
        );

        BeforeEnter(CashWithdrawStates.CashWithdrawTradingPlatformUpdating,
            binder => binder.Request(
                UpdateTradingAccountBalanceRequest,
                ctx => new UpdateTradingAccountBalanceRequest(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.TransactionNo!,
                    ctx.Saga.RequestedAmount,
                    ctx.Saga.CustomerCode,
                    ctx.Saga.AccountCode,
                    ctx.Saga.BankName!,
                    ctx.Saga.Channel,
                    TransactionType.Withdraw))
        );

        During(CashWithdrawStates.CashWithdrawTradingPlatformUpdating,
            When(UpdateTradingAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(CashWithdrawStates.CashWithdrawWaitingForTradingPlatform),
            When(UpdateTradingAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed. Reasons: {string.Join(",", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill, null))
                .TransitionTo(CashWithdrawStates.TransferRequestFailed)
        // TODO: In case want to bring back Revert
        // .IfElse(ctx => ctx.Saga.Product == Product.Derivatives,
        //     binder => binder.TransitionTo(CashWithdrawStates.RevertTfexTransfer),
        //     binder => binder.TransitionTo(CashWithdrawStates.TransferRequestFailed))
        );

        // Wait for callback from Freewill gateway
        During(CashWithdrawStates.CashWithdrawWaitingForTradingPlatform,
            When(CashWithdrawGatewayCallbackSuccessEvent)
                .LogSnapshot()
                .Publish(ctx =>
                    new WithdrawConfirmationReceived(
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.RequestedAmount,
                        kkpBankFee
                    ))
                .TransitionTo(CashWithdrawStates.WithdrawalProcessing),
            When(CashWithdrawGatewayCallbackFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed. ResultCode: " +
                        $"{string.Join(", ", ctx.Message.ResultCode)} Reason: {ctx.Message.Reason}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback, null))
                .TransitionTo(CashWithdrawStates.TransferRequestFailed)
        // TODO: In case want to bring back Revert
        // .IfElse(ctx => ctx.Saga.Product == Product.Derivatives,
        //     binder => binder.TransitionTo(CashWithdrawStates.RevertTfexTransfer),
        //     binder => binder.TransitionTo(CashWithdrawStates.TransferRequestFailed))
        );

        During(CashWithdrawStates.WithdrawalProcessing,
            When(WithdrawSuccess)
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .SendMetric(Metrics.CashWithdrawAmount, ctx => (double)ctx.Saga.RequestedAmount, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .Publish(ctx =>
                    new CashWithdrawSuccessEvent(
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        DateTime.Now,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.RequestedAmount))
                .Finalize(),
            When(WithdrawFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Withdraw Failed. Reason: {ctx.Message.Reason}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.WithdrawFailed, null))
                .TransitionTo(CashWithdrawStates.TransferRequestFailed)
        //.TransitionTo(CashWithdrawStates.RevertPlatformTransfer)
        );

        // Assigned when manual publish event from internal endpoint
        During(CashWithdrawStates.TransferRequestFailed,
            When(WithdrawFailed)
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.OperationSupport, null))
                .TransitionTo(CashWithdrawStates.WithdrawalFailed));

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Revert Transfer

        BeforeEnter(CashWithdrawStates.RevertTfexTransfer,
        binder => binder.Request(
            UpdateTfexAccountBalanceRequest,
                ctx => new UpdateTfexAccountBalanceRequest(
                    ctx.Saga.UserId,
                    ctx.Saga.TransactionNo!,
                    ctx.Saga.AccountCode,
                    ctx.Saga.RequestedAmount,
                    TransactionType.Deposit))
        );

        During(CashWithdrawStates.RevertTfexTransfer,
            When(UpdateTfexAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawRevertTransferSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(CashWithdrawStates.RevertTransferSuccess),
            When(UpdateTfexAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"SetTrade Revert Transfer Failed. Exception: {ctx.Message.Exceptions}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawRevertTransferFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Tfex, null))
                .TransitionTo(CashWithdrawStates.RevertTransferFailed)
        );

        BeforeEnter(CashWithdrawStates.RevertPlatformTransfer,
            binder => binder.Request(
                UpdateTradingAccountBalanceRequest,
                ctx => new UpdateTradingAccountBalanceRequest(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.TransactionNo!,
                    ctx.Saga.RequestedAmount,
                    ctx.Saga.CustomerCode,
                    ctx.Saga.AccountCode,
                    ctx.Saga.BankName!,
                    ctx.Saga.Channel,
                    TransactionType.Deposit))
        );

        During(CashWithdrawStates.RevertPlatformTransfer,
            When(UpdateTradingAccountBalanceRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(CashWithdrawStates.RevertWaitingForPlatformCallback),
            When(UpdateTradingAccountBalanceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill reverting failed with error: {ctx.Message.Exceptions}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawRevertTransferFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill, null))
                .TransitionTo(CashWithdrawStates.RevertTransferFailed)
        );

        During(CashWithdrawStates.RevertWaitingForPlatformCallback,
            When(CashDepositGatewayCallbackSuccessEvent)
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product == Product.Derivatives,
                    binder => binder.TransitionTo(CashWithdrawStates.RevertTfexTransfer),
                    binder => binder
                        .SendMetric(Metrics.CashWithdrawRevertTransferSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                        .TransitionTo(CashWithdrawStates.RevertTransferSuccess)),
            When(CashDepositGatewayCallbackFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill update failed. ResultCode: " +
                        $"{string.Join(", ", ctx.Message.ResultCode)}, Reason: {ctx.Message.Reason}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.CashWithdrawRevertTransferFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback, null))
                .TransitionTo(CashWithdrawStates.RevertTransferFailed)
        );

        #endregion
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : CashWithdrawState
        where TData : class
    {
        return source.ThenAsync(async ctx =>
        {
            var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

            var exceptionInfo = ctx.Message.Exceptions.FirstOrDefault();
            var errorMessage = exceptionInfo?.ExceptionType switch
            {
                "Pi.WalletService.Domain.Exceptions.UserOtpRequestLimitReachedException"
                    => new BusRequestFailed(exceptionInfo, ErrorCodes.UserOtpRequestLimitReached,
                        "UserOtpRequestLimitReached"),
                "Pi.WalletService.Domain.Exceptions.UserOtpVerificationLimitReachedException"
                    => new BusRequestFailed(exceptionInfo, ErrorCodes.UserOtpVerificationLimitReached,
                        "UserOtpVerificationLimitReached"),
                _ => new BusRequestFailed(exceptionInfo),
            };

            await responseEndpoint.Send(
                errorMessage,
                callback: context => context.RequestId = ctx.Saga.RequestId
            );
        });
    }

    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : CashWithdrawState
        where TData : class
    {
        return source.Publish(
            ctx => new LogWithdrawTransaction(ctx.Saga.ToSnapshot())
        );
    }

    public static WithdrawTransactionSnapshot ToSnapshot(this CashWithdrawState state)
    {
        return new WithdrawTransactionSnapshot(
            state.CorrelationId,
            state.UserId,
            state.CurrentState ?? string.Empty,
            state.CustomerCode,
            state.TransactionNo ?? string.Empty,
            state.AccountCode,
            state.Channel,
            state.Product,
            state.BankFee,
            null,
            state.RequestedAmount,
            state.BankName,
            state.BankCode,
            state.BankAccountNo,
            state.FailedReason,
            state.DeviceId
        );
    }
}