using MassTransit;
using Microsoft.Extensions.Configuration;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;

namespace Pi.WalletService.Application.StateMachines.GlobalWalletTransfer;

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState>
{
    public Event<GlobalTransferDepositRequestReceived>? GlobalDepositRequestReceived { get; private set; }
    public Event<GlobalTransferWithdrawRequestReceived>? GlobalWithdrawRequestReceived { get; private set; }
    public Event<DepositSuccessEvent>? DepositPaymentReceived { get; private set; }
    public Event<DepositFailedEvent>? DepositPaymentFailed { get; private set; }
    public Event<WithdrawOtpValidationSuccess>? WithdrawOtpValidationSuccess { get; private set; }
    public Event<WithdrawSuccessEvent>? WithdrawSuccess { get; private set; }
    public Event<WithdrawFailedEvent>? WithdrawFailed { get; private set; }
    public Event<DepositRefundSucceedEvent>? RefundSuccessEvent { get; private set; }
    public Event<GlobalManualAllocationProcessingEvent>? GlobalManualAllocationProcessing { get; private set; }
    public Event<GlobalManualAllocationSuccessEvent>? GlobalManualAllocationSuccess { get; private set; }
    public Event<GlobalManualAllocationFailedEvent>? GlobalManualAllocationFailed { get; private set; }
    public Event<RefundingDeposit>? RefundingDeposit { get; set; }
    public Event<RefundSucceedEvent>? RefundSucceed { get; set; }
    public Event<RefundFailedEvent>? RefundFailed { get; set; }
    public Event<DepositPaymentNotReceivedSpecific>? GlobalDepositPaymentNotReceived { get; private set; }

    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, LogGlobalWalletTransferTransaction, LogGlobalWalletTransferTransactionSuccess>? LogSnapshot { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, RequestFxTransaction, QueryFxTransactionSucceed>? FxTransactionRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, ValidateGlobalTransferRequest, GlobalTransferRequestValidationCompleted>? TransferRequestValidation { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, GenerateTransactionNo, TransactionNoGenerated>? GenerateTransactionNoRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, ValidateGlobalTransferDepositPayment, GlobalTransferDepositPaymentValidationCompleted>? GlobalTransferDepositValidationRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, WithdrawInitiateRequest, WithdrawInitiateResponse>? GlobalWithdrawInitiateRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, InitiateFxRequest, GetFxInitialQuoteSucceed>? FxInitiateRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, ValidateFxRequest, ValidateFxRequestSucceed>? FxValidateRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, ConfirmFxRequest, FxConfirmed>? FxConfirmRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, KkpWithdrawRequest, WithdrawOddSucceed>? KkpWithdrawRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, TransferUsdMoneyFromMainAccountToSubAccount, TransferUsdMoneyToSubSucceeded>? TransferUsdToSubRequest { get; private set; }
    public Request<Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState, TransferUsdMoneyFromSubAccountToMainAccount, TransferUsdMoneyToMainSucceeded>? TransferUsdToMainRequest { get; private set; }

    public StateMachine(IConfiguration configuration)
    {
        var qrCodeExpireTimeInMinute = int.Parse(configuration["QrCode:TimeOutHour"] ?? "1") * 60;
        var kkpBankFee = Convert.ToDecimal(configuration["Bank:KKP:GlobalWithdrawFee"]);
        var depositTransferFee = Convert.ToDecimal(configuration["Bank:Exante:DepositTransferFee"]);
        var withdrawTransferFee = Convert.ToDecimal(configuration["Bank:Exante:WithdrawTransferFee"]);

        InstanceState(x => x.CurrentState);

        Event(() => GlobalDepositRequestReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => GlobalWithdrawRequestReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => DepositPaymentReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => DepositPaymentFailed, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => WithdrawOtpValidationSuccess, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => WithdrawSuccess, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => WithdrawFailed, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => RefundSuccessEvent, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => GlobalManualAllocationProcessing, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => GlobalManualAllocationSuccess, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => GlobalManualAllocationFailed, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => RefundingDeposit, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.DepositTransactionNo));
        Event(() => RefundSucceed, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.DepositTransactionNo));
        Event(() => RefundFailed, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.DepositTransactionNo));
        Event(() => GlobalDepositPaymentNotReceived, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));

        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GenerateTransactionNoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GlobalTransferDepositValidationRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GlobalWithdrawInitiateRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferRequestValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxInitiateRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxValidateRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxTransactionRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxConfirmRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferUsdToSubRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferUsdToMainRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => KkpWithdrawRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });

        State(() => GlobalWalletTransferState.Received);
        State(() => GlobalWalletTransferState.TransactionNoGenerating);
        State(() => GlobalWalletTransferState.DepositProcessing);
        State(() => GlobalWalletTransferState.DepositFailed);
        State(() => GlobalWalletTransferState.TransferRequestValidating);
        State(() => GlobalWalletTransferState.TransferRequestFailed);
        State(() => GlobalWalletTransferState.GlobalTransferPaymentValidating);
        State(() => GlobalWalletTransferState.AwaitingOtpValidation);
        State(() => GlobalWalletTransferState.GlobalWithdrawInitiating);
        State(() => GlobalWalletTransferState.FxInitiating);
        State(() => GlobalWalletTransferState.FxValidating);
        State(() => GlobalWalletTransferState.FxConfirming);
        State(() => GlobalWalletTransferState.FxFailed);
        State(() => GlobalWalletTransferState.FxRateCompareFailed);
        State(() => GlobalWalletTransferState.FxQueryTransaction);
        State(() => GlobalWalletTransferState.FxTransferring);
        State(() => GlobalWalletTransferState.FxTransferFailed);
        State(() => GlobalWalletTransferState.FxTransferInsufficientBalance);
        State(() => GlobalWalletTransferState.WithdrawalProcessing);
        State(() => GlobalWalletTransferState.RevertingTransfer);
        State(() => GlobalWalletTransferState.RevertTransferSuccess);
        State(() => GlobalWalletTransferState.RevertTransferFailed);
        State(() => GlobalWalletTransferState.ManualAllocationInprogress);
        State(() => GlobalWalletTransferState.ManualAllocationFailed);
        State(() => GlobalWalletTransferState.Refunding);
        State(() => GlobalWalletTransferState.RefundSucceed);
        State(() => GlobalWalletTransferState.RefundFailed);
        State(() => GlobalWalletTransferState.GlobalDepositPaymentNotReceived);
        State(() => GlobalWalletTransferState.GlobalWithdrawOtpValidationNotReceived);
        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Initialization

        // Initialize step 1
        Initially(
            When(GlobalDepositRequestReceived)
                .Then(ctx =>
                {
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.RequesterDeviceId = ctx.Message.DeviceId;
                    ctx.Saga.CustomerId = ctx.Message.CustomerId;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.GlobalAccount = ctx.Message.GlobalAccount;
                    ctx.Saga.Product = Product.GlobalEquities;

                    ctx.Saga.TransactionType = TransactionType.Deposit;
                    ctx.Saga.RequestedAmount = ctx.Message.RequestedAmount;
                    ctx.Saga.RequestedFxAmount = ctx.Message.RequestedFxAmount;
                    ctx.Saga.RequestedCurrency = ctx.Message.RequestedCurrency;
                    ctx.Saga.RequestedFxCurrency = ctx.Message.RequestedFxCurrency;
                })
                .SendMetric(Metrics.GlobalRequestReceived, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.Received),
            When(GlobalWithdrawRequestReceived)
                .Then(ctx =>
                {
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.RequesterDeviceId = ctx.Message.DeviceId;
                    ctx.Saga.CustomerId = ctx.Message.CustomerId;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.GlobalAccount = ctx.Message.GlobalAccount;
                    ctx.Saga.Product = Product.GlobalEquities;

                    ctx.Saga.TransactionType = TransactionType.Withdraw;
                    ctx.Saga.FxTransactionId = ctx.Message.FxTransactionId;
                    ctx.Saga.RequestedAmount = ctx.Message.RequestedForeignAmount;
                    ctx.Saga.RequestedCurrency = ctx.Message.RequestedForeignCurrency;
                    ctx.Saga.RequestedFxCurrency = Currency.THB;
                })
                .SendMetric(Metrics.GlobalRequestReceived, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.Received)
        );

        // Initialize step 2
        BeforeEnter(GlobalWalletTransferState.Received,
            binder => binder
                .Request(LogSnapshot, ctx => new LogGlobalWalletTransferTransaction(ctx.Saga.ToSnapshot()))
        );
        During(GlobalWalletTransferState.Received,
            When(LogSnapshot?.Completed)
                .TransitionTo(GlobalWalletTransferState.TransactionNoGenerating),
            When(LogSnapshot?.Faulted)
                .SendErrorResponse()
        );

        // Initialize step 3
        BeforeEnter(GlobalWalletTransferState.TransactionNoGenerating,
            binder => binder
                .Request(GenerateTransactionNoRequest, ctx =>
                    new GenerateTransactionNo(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.Product,
                        Channel.QR,
                        ctx.Saga.TransactionType,
                        false
                    )
                )
        );
        During(GlobalWalletTransferState.TransactionNoGenerating,
            When(GenerateTransactionNoRequest?.Completed)
                .Then(ctx => { ctx.Saga.TransactionNo = ctx.Message.TransactionNo; })
                .LogSnapshot()
                .ThenAsync(async ctx =>
                {
                    var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

                    await responseEndpoint.Send(
                        new TransactionNoGenerated(ctx.Message.TransactionNo),
                        callback: context => context.RequestId = ctx.Saga.RequestId
                    );
                })
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                    binder => binder.TransitionTo(GlobalWalletTransferState.DepositProcessing)
                )
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Withdraw,
                    binder => binder.TransitionTo(GlobalWalletTransferState.GlobalWithdrawInitiating)
                ),
            When(GenerateTransactionNoRequest?.Faulted)
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.GenerateTransactionNo, ctx.Saga.TransactionType))
                .SendErrorResponse()
        );

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Deposit

        // Deposit Step 1
        BeforeEnter(GlobalWalletTransferState.DepositProcessing,
            binder => binder.Publish(ctx
                => new RequestDeposit(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.UserId,
                    ctx.Saga.CustomerCode,
                    ctx.Saga.Product,
                    Channel.QR,
                    RoundingUtils.RoundExchangeTransaction(
                        TransactionType.Deposit,
                        ctx.Saga.RequestedCurrency,
                        ctx.Saga.RequestedAmount,
                        Currency.THB,
                        ctx.Saga.RequestedFxAmount
                    ),
                    ctx.Saga.RequesterDeviceId!.Value,
                    ctx.Saga.TransactionNo,
                    false
                )
            )
        );
        During(GlobalWalletTransferState.DepositProcessing,
            When(DepositPaymentReceived)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.Amount;
                    ctx.Saga.PaymentReceivedCurrency = Currency.THB;
                })
                .LogSnapshot()
                .Request(
                    GlobalTransferDepositValidationRequest,
                    ctx
                        => new ValidateGlobalTransferDepositPayment(
                            ctx.Saga.TransactionNo!,
                            ctx.Message.QrCodeGeneratedTime!.Value,
                            ctx.Message.PaymentReceivedDateTime,
                            qrCodeExpireTimeInMinute
                        )
                )
                .TransitionTo(GlobalWalletTransferState.GlobalTransferPaymentValidating),
            When(GlobalDepositPaymentNotReceived)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .SendMetric(Metrics.DepositCancelled, ctx => new MetricTags(null, null, TagFailedReason.PaymentNotReceived, null))
                .TransitionTo(GlobalWalletTransferState.GlobalDepositPaymentNotReceived)
        );

        During(GlobalWalletTransferState.DepositProcessing,
            When(DepositPaymentFailed)
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.DepositPaymentFailed, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.DepositFailed)
                .Publish(ctx =>
                    new GlobalDepositFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!
                    )
                )
            );

        During(GlobalWalletTransferState.DepositProcessing,
            When(RefundSuccessEvent)
                .Then(ctx =>
                {
                    ctx.Saga.RefundAmount = ctx.Message.RefundAmount;
                    ctx.Saga.NetAmount = ctx.Message.PaymentReceivedAmount - ctx.Message.RefundAmount;
                })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.RefundSuccess, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.DepositFailed));

        // Deposit Step 2
        During(GlobalWalletTransferState.GlobalTransferPaymentValidating,
            When(GlobalTransferDepositValidationRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.FxInitiating),
            When(GlobalTransferDepositValidationRequest?.Faulted)
                .Then(context =>
                {
                    context.Saga.FailedReason = $"Unable To GlobalTransferPaymentProcess Failed. Exception: {string.Join(", ", context.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.ValidateGlobalRequest, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.TransferRequestFailed)
                .Publish(ctx =>
                    new GlobalDepositFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!
                        )
                )
        );

        // Deposit Step 3
        BeforeEnter(GlobalWalletTransferState.FxInitiating,
            binder => binder
                .Request(
                    FxInitiateRequest,
                    ctx => new InitiateFxRequest(
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionType,
                        ctx.Saga.PaymentReceivedAmount!.Value,
                        ctx.Saga.PaymentReceivedCurrency!.Value
                    )
                )
        );
        During(GlobalWalletTransferState.FxInitiating,
            When(FxInitiateRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.FxInitiateRequestDateTime = ctx.Message.InitiateDateTime;
                    ctx.Saga.FxTransactionId = ctx.Message.TransactionId;
                    ctx.Saga.FxConfirmedExchangeRate = ctx.Message.ConfirmedFxRate;
                    ctx.Saga.FxConfirmedAmount = ctx.Message.ConfirmedAmount;
                    ctx.Saga.FxConfirmedCurrency = ctx.Message.ConfirmedCurrency;
                })
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.FxValidating),
            When(FxInitiateRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To InitiateFx. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxInitiate, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.FxFailed)
        );

        BeforeEnter(GlobalWalletTransferState.FxValidating,
            binder => binder.Request(
                FxValidateRequest,
                ctx =>
                    new ValidateFxRequest(
                        ctx.Saga.FxTransactionId!,
                        ctx.Saga.RequestedFxAmount,
                        ctx.Saga.FxConfirmedExchangeRate!.Value,
                        0
                    )
            )
        );
        During(GlobalWalletTransferState.FxValidating,
            When(FxValidateRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.FxConfirming),
            When(FxValidateRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"ValidateFxRequest failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .IfElse(
                    ctx => ctx.Message.Exceptions.Any(e => e.ExceptionType == typeof(FxRateDiffOverThresholdException).FullName),
                    binder => binder
                        .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxRateCompare, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalWalletTransferState.FxRateCompareFailed),
                    binder => binder
                        .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxFailed, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalWalletTransferState.FxFailed)
                ));

        // Deposit Step 5
        BeforeEnter(GlobalWalletTransferState.FxConfirming,
            binder => binder
                .Request(
                    FxConfirmRequest,
                    ctx => new ConfirmFxRequest(ctx.Saga.FxTransactionId!)
                )
        );
        During(GlobalWalletTransferState.FxConfirming,
            When(FxConfirmRequest?.Completed)
                .Then(ctx => { ctx.Saga.FxConfirmedDateTime = ctx.Message.ConfirmedTime; })
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.FxTransferring),
            When(FxConfirmRequest?.Faulted)
                .Then(context => { context.Saga.FailedReason = $"Unable To ConfirmFx. Exception: {string.Join(", ", context.Message.Exceptions.Select(e => e.Message))}"; })
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxConfirm, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.FxFailed)
        );

        // Deposit Step 6 (Last); State shared with Withdraw
        BeforeEnter(GlobalWalletTransferState.FxTransferring,
            binder => binder
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                    ifBinder => ifBinder.Request(
                        TransferUsdToSubRequest,
                        ctx => new TransferUsdMoneyFromMainAccountToSubAccount(
                            ctx.Saga.TransactionNo!,
                            ctx.Saga.GlobalAccount,
                            ctx.Saga.FxConfirmedCurrency!.Value,
                            ctx.Saga.FxConfirmedAmount!.Value,
                            depositTransferFee
                        ))
                )
        );
        During(GlobalWalletTransferState.FxTransferring,
            When(TransferUsdToSubRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.TransferFromAccount = context.Message.FromAccount;
                    context.Saga.TransferToAccount = context.Message.ToAccount;
                    context.Saga.TransferRequestTime = context.Message.RequestedTime;
                    context.Saga.TransferAmount = context.Message.TransferAmount;
                    context.Saga.TransferFee = context.Message.Fee;
                    context.Saga.TransferCurrency = context.Message.TransferCurrency;
                    context.Saga.TransferCompleteTime = context.Message.CompletedTime;
                })
                .LogSnapshot()
                .Publish(
                    ctx =>
                        new GlobalDepositSuccessEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.UserId,
                            ctx.Saga.TransactionNo!
                        )
                )
                .SendMetric(Metrics.GlobalRequestSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .Finalize(),
            When(TransferUsdToSubRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Transfer Money From Main Account to SubAccount. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                // .Publish(
                //     ctx =>
                //         new GlobalDepositFailedEvent(
                //             ctx.Saga.CorrelationId,
                //             ctx.Saga.UserId,
                //             ctx.Saga.TransactionNo!
                //         )
                // )
                .IfElse(
                    ctx => Array.Exists(ctx.Message.Exceptions, e => e.ExceptionType == typeof(TransferInsufficientBalanceException).FullName),
                    binder => binder
                        .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxInsufficientBalance, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalWalletTransferState.FxTransferInsufficientBalance),
                    binder => binder
                        .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxTransfer, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalWalletTransferState.FxTransferFailed)
                )
        );

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Withdraw

        // Withdraw Step 1
        BeforeEnter(GlobalWalletTransferState.GlobalWithdrawInitiating,
            binder =>
                binder.Request(
                    GlobalWithdrawInitiateRequest,
                    ctx => new WithdrawInitiateRequest(ctx.Saga.UserId, ctx.Saga.CustomerCode, ctx.Saga.Product)
                )
        );
        During(GlobalWalletTransferState.GlobalWithdrawInitiating,
            When(GlobalWithdrawInitiateRequest?.Completed)
                .Publish(ctx => new WithdrawRequestReceived(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.UserId,
                    ctx.Saga.CustomerCode + '2',
                    ctx.Saga.CustomerCode,
                    ctx.Saga.RequesterDeviceId!.Value,
                    Product.GlobalEquities,
                    ctx.Saga.TransactionNo,
                    ctx.Message.BankAccountNo,
                    ctx.Message.BankCode,
                    ctx.Message.BankName,
                    ctx.Saga.RequestId,
                    ctx.Saga.ResponseAddress
                ))
                .TransitionTo(GlobalWalletTransferState.AwaitingOtpValidation),
            When(GlobalWithdrawInitiateRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To Initiate Withdraw Transaction. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .SendErrorResponse()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.Request, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.TransferRequestFailed)
        );

        // Withdraw Step 2
        During(GlobalWalletTransferState.AwaitingOtpValidation,
            When(WithdrawOtpValidationSuccess)
                .TransitionTo(GlobalWalletTransferState.FxQueryTransaction),
            When(WithdrawFailed)
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.WithdrawFailed, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.TransferRequestFailed)
        );

        // Withdraw Step 3
        BeforeEnter(GlobalWalletTransferState.FxQueryTransaction,
            binder => binder
                .Request(
                    FxTransactionRequest,
                    ctx => new RequestFxTransaction(
                        ctx.Saga.TransactionType,
                        ctx.Saga.FxTransactionId!,
                        ctx.Saga.CreatedAt,
                        0
                    )
                    {
                        ExpectedContractCurrency = ctx.Saga.RequestedCurrency,
                        ExpectedCounterCurrency = Currency.THB,
                    }
                )
        );
        During(GlobalWalletTransferState.FxQueryTransaction,
            When(FxTransactionRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.RequestedFxAmount = ctx.Message.ExchangeRate;
                    ctx.Saga.FxInitiateRequestDateTime = ctx.Message.TransactionDateTime;
                })
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.TransferRequestValidating),
            When(FxTransactionRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To Fetch Fx Transaction. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .SendErrorResponse()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxInitiate, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.TransferRequestFailed)
        );

        // Withdraw Step 4
        BeforeEnter(GlobalWalletTransferState.TransferRequestValidating,
            binder => binder
                .Request(
                    TransferRequestValidation,
                    ctx =>
                        new ValidateGlobalTransferRequest(
                            ctx.Saga.TransactionNo!,
                            ctx.Saga.TransactionType,
                            ctx.Saga.UserId,
                            ctx.Saga.CustomerCode,
                            ctx.Saga.RequestedFxAmount,
                            ctx.Saga.RequestedAmount,
                            ctx.Saga.RequestedCurrency,
                            ctx.Saga.RequestedFxCurrency
                        )
                )
        );
        During(GlobalWalletTransferState.TransferRequestValidating,
            When(TransferRequestValidation?.Completed)
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.FxTransferring),
            When(TransferRequestValidation?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To Validate Transfer Request. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxTransferValidation, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.TransferRequestFailed)
        );

        // Withdraw Step 5; State shared with Deposit
        BeforeEnter(GlobalWalletTransferState.FxTransferring,
            binder => binder
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Withdraw,
                    ifBinder => ifBinder.Request(
                        TransferUsdToMainRequest,
                        ctx =>
                            new TransferUsdMoneyFromSubAccountToMainAccount(
                                ctx.Saga.TransactionNo!,
                                ctx.Saga.GlobalAccount,
                                ctx.Saga.RequestedCurrency,
                                ctx.Saga.RequestedAmount,
                                withdrawTransferFee
                            )
                    )
                )
        );
        During(GlobalWalletTransferState.FxTransferring,
            When(TransferUsdToMainRequest?.Completed)
                .Then(context =>
                {
                    context.Saga.TransferFromAccount = context.Message.FromAccount;
                    context.Saga.TransferToAccount = context.Message.ToAccount;
                    context.Saga.TransferRequestTime = context.Message.RequestedTime;
                    context.Saga.TransferAmount = context.Message.TransferAmount;
                    context.Saga.TransferFee = context.Message.Fee;
                    context.Saga.TransferCurrency = context.Message.TransferCurrency;
                    context.Saga.TransferCompleteTime = context.Message.CompletedTime;
                })
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.WithdrawalProcessing),
            When(TransferUsdToMainRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Transfer Money From Sub Account to Main Account. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxTransfer, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.TransferRequestFailed)
        );

        // Withdraw Step 6 (Last)
        BeforeEnter(GlobalWalletTransferState.WithdrawalProcessing,
            binder => binder
                .Publish(
                    ctx => new WithdrawConfirmationReceived(
                        ctx.Saga.TransactionNo!,
                        RoundingUtils.RoundExchangeTransaction(
                            TransactionType.Withdraw,
                            ctx.Saga.TransferCurrency!.Value,
                            ctx.Saga.TransferAmount!.Value - ctx.Saga.TransferFee!.Value,
                            Currency.THB,
                            ctx.Saga.RequestedFxAmount,
                            roundExchangeRate: false
                        ),
                        kkpBankFee
                    ))
        );
        During(GlobalWalletTransferState.WithdrawalProcessing,
            When(WithdrawSuccess)
                .LogSnapshot()
                .Then(ctx =>
                {
                    ctx.Saga.NetAmount = ctx.Message.Amount;
                    ctx.Saga.RefundAmount = 0;
                })
                .Respond(
                    ctx => new WithdrawOddSucceedSagaResponse(ctx.Saga.TransactionNo!)
                )
                .SendMetric(Metrics.GlobalRequestSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .Finalize(),
            When(WithdrawFailed)
                .LogSnapshot()
                .TransitionTo(GlobalWalletTransferState.RevertingTransfer)
        );

        #region Recovery

        BeforeEnter(GlobalWalletTransferState.RevertingTransfer,
            binder => binder
                .Request(
                    TransferUsdToSubRequest,
                    ctx =>
                        new TransferUsdMoneyFromMainAccountToSubAccount(
                            ctx.Saga.TransactionNo!,
                            ctx.Saga.GlobalAccount,
                            ctx.Saga.TransferCurrency!.Value,
                            ctx.Saga.TransferAmount!.Value,
                            0
                        )
                )
        );
        During(GlobalWalletTransferState.RevertingTransfer,
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
                .LogSnapshot()
                .SendMetric(Metrics.RevertSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.RevertTransferSuccess),
            When(TransferUsdToSubRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Transfer Money Back From Main Account to Sub Account. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.RevertFailed, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.RevertTransferFailed)
        );

        #endregion

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Refund

        foreach (var state in GlobalWalletTransferState.GetRefundAllowedStates())
        {
            During(state,
                When(RefundingDeposit)
                    .If(
                        ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                        binder => binder
                            .LogSnapshot()
                            .TransitionTo(GlobalWalletTransferState.Refunding)
                    )
            );
        }

        During(GlobalWalletTransferState.Refunding,
            When(RefundSucceed)
                .Then(ctx =>
                {
                    ctx.Saga.RefundAmount = ctx.Message.RefundAmount;
                    ctx.Saga.NetAmount = ctx.Saga.PaymentReceivedAmount - ctx.Message.RefundAmount;
                })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRefundSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.RefundSucceed),
            When(RefundFailed)
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRefundFailed, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalWalletTransferState.RefundFailed)
        );

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region ManualAllocation

        foreach (var (state, transactionType) in GlobalWalletTransferState.GetManualAllocationAllowedStates())
        {
            During(state,
                When(GlobalManualAllocationProcessing)
                    .If(
                        ctx => transactionType == null || ctx.Saga.TransactionType == transactionType,
                        binder => binder
                            .LogSnapshot()
                            .TransitionTo(GlobalWalletTransferState.ManualAllocationInprogress)
                    )
            );
        }

        During(GlobalWalletTransferState.ManualAllocationInprogress,
            When(GlobalManualAllocationSuccess)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = null;
                    ctx.Saga.TransferFromAccount = ctx.Message.FromAccount;
                    ctx.Saga.TransferToAccount = ctx.Message.ToAccount;
                    ctx.Saga.TransferRequestTime = ctx.Message.RequestedTime;
                    ctx.Saga.TransferAmount = ctx.Message.TransferAmount;
                    ctx.Saga.TransferCurrency = ctx.Message.TransferCurrency;
                    ctx.Saga.TransferCompleteTime = ctx.Message.CompletedTime;
                })
                .LogSnapshot()
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                    binder => binder.Finalize()
                )
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Withdraw,
                    binder => binder.TransitionTo(GlobalWalletTransferState.RevertTransferSuccess)
                ),
            When(GlobalManualAllocationFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .LogSnapshot()
                // This will make fail tx be retry on next day
                // not desired behavior right now so use below code for now
                // .IfElse(
                //     ctx => ctx.Message.RequestType == GlobalManualAllocationType.DepositInsufficientBalanceAutoRetry,
                //     binder => binder.TransitionTo(GlobalTransferState.FxTransferInsufficientBalance),
                //     binder => binder.TransitionTo(GlobalTransferState.ManualAllocationFailed)
                // )
                // This will only retry once even if it fails
                .TransitionTo(GlobalWalletTransferState.ManualAllocationFailed)
        );

        #endregion

    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState
        where TData : class
    {
        return source.ThenAsync(async ctx =>
        {
            var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

            await responseEndpoint.Send(
                new BusRequestFailed(ctx.Message.Exceptions.FirstOrDefault()),
                callback: context => context.RequestId = ctx.Saga.RequestId
            );
        });
    }

    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState
        where TData : class
    {
        return source.Publish(
            ctx => new LogGlobalWalletTransferTransaction(ctx.Saga.ToSnapshot())
        );
    }

    public static GlobalWalletTransferTransactionSnapshot ToSnapshot(this Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState state)
    {
        return new GlobalWalletTransferTransactionSnapshot(
            state.UserId,
            state.CurrentState ?? string.Empty,
            state.CustomerId,
            state.CustomerCode,
            state.GlobalAccount,
            state.CorrelationId,
            state.TransactionNo ?? string.Empty,
            state.TransactionType,
            state.RequestedAmount,
            state.RequestedCurrency,
            state.RequestedFxAmount,
            state.RequestedFxCurrency,
            state.PaymentReceivedAmount,
            state.PaymentReceivedCurrency,
            state.FxInitiateRequestDateTime,
            state.FxTransactionId,
            state.FxConfirmedDateTime,
            state.FxConfirmedExchangeRate,
            state.FxConfirmedAmount,
            state.FxConfirmedCurrency,
            state.TransferFromAccount,
            state.TransferAmount,
            state.TransferToAccount,
            state.TransferCurrency,
            state.TransferRequestTime,
            state.TransferCompleteTime,
            state.TransferFee,
            state.RefundAmount,
            state.NetAmount,
            state.RequesterDeviceId
        );
    }
}
