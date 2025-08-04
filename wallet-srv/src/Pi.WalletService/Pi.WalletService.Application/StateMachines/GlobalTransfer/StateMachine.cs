using MassTransit;
using Microsoft.Extensions.Configuration;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.GlobalTransfer;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using GlobalTransferStates = Pi.WalletService.IntegrationEvents.Models.GlobalTransferState;

namespace Pi.WalletService.Application.StateMachines.GlobalTransfer;

public class StateMachine : MassTransitStateMachine<GlobalTransferState>
{
    public Event<GlobalTransferRequest>? GlobalTransferRequestReceived { get; private set; }
    public Event<DepositRefundSucceedEventV2>? RefundSuccessEvent { get; private set; }
    public Event<GlobalManualAllocationProcessingEvent>? GlobalManualAllocationProcessing { get; private set; }
    public Event<GlobalManualAllocationSuccessEvent>? GlobalManualAllocationSuccess { get; private set; }
    public Event<GlobalManualAllocationFailedEvent>? GlobalManualAllocationFailed { get; private set; }
    public Event<RefundingDeposit>? RefundingDeposit { get; set; }
    public Event<RefundSucceedEvent>? RefundSucceed { get; set; }
    public Event<RefundFailedEvent>? RefundFailed { get; set; }
    public Event<GlobalRevertEvent>? RevertGlobalTransaction { get; private set; }

    public Request<GlobalTransferState, PrepareGlobalTransferFxExchangeData, PrepareGlobalTransferFxExchangeDataSuccess>? PreparePaymentReceivedDataRequest { get; private set; }
    public Request<GlobalTransferState, LogActivity, LogActivitySuccess>? LogSnapshot { get; private set; }
    public Request<GlobalTransferState, RequestFxTransaction, QueryFxTransactionSucceed>? FxTransactionRequest { get; private set; }
    public Request<GlobalTransferState, ValidateGlobalTransferV2Request, GlobalTransferRequestValidationCompletedV2>? TransferRequestValidation { get; private set; }
    public Request<GlobalTransferState, ValidateGlobalTransferDepositPaymentV2, GlobalTransferDepositPaymentValidationCompleted>? GlobalTransferDepositValidationRequest { get; private set; }
    public Request<GlobalTransferState, InitiateFxV2Request, GetFxInitialQuoteSucceed>? FxInitiateRequest { get; private set; }
    public Request<GlobalTransferState, ValidateFxRequest, ValidateFxRequestSucceed>? FxValidateRequest { get; private set; }
    public Request<GlobalTransferState, ConfirmFxRequest, FxConfirmed>? FxConfirmRequest { get; private set; }
    public Request<GlobalTransferState, TransferUsdMoneyFromMainAccountToSubAccountV2, TransferUsdMoneyToSubSucceeded>? TransferUsdToSubRequest { get; private set; }
    public Request<GlobalTransferState, TransferUsdMoneyFromSubAccountToMainAccountV2, TransferUsdMoneyToMainSucceeded>? TransferUsdToMainRequest { get; private set; }

    public StateMachine(IConfiguration configuration)
    {
        var depositTransferFee = Convert.ToDecimal(configuration["Bank:Exante:DepositTransferFee"]);
        var withdrawTransferFee = Convert.ToDecimal(configuration["Bank:Exante:WithdrawTransferFee"]);

        InstanceState(x => x.CurrentState);
        Event(() => GlobalTransferRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RefundSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GlobalManualAllocationProcessing, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GlobalManualAllocationSuccess, x => x.CorrelateById(ctx => ctx.Message.TransactionId));
        Event(() => GlobalManualAllocationFailed, x => x.CorrelateById(ctx => ctx.Message.TransactionId));
        Event(() => RefundingDeposit, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RefundSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RefundFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RevertGlobalTransaction, x => x.CorrelateBy((state, ctx) => state.CorrelationId == ctx.Message.CorrelationId));

        Request(() => PreparePaymentReceivedDataRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GlobalTransferDepositValidationRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferRequestValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxInitiateRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxValidateRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxTransactionRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => FxConfirmRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferUsdToSubRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => TransferUsdToMainRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });

        State(() => GlobalTransferStates.Initial);
        State(() => GlobalTransferStates.Received);
        State(() => GlobalTransferStates.PaymentReceivedDataPreparing);
        State(() => GlobalTransferStates.TransferRequestFailed);
        State(() => GlobalTransferStates.GlobalTransferPaymentValidating);
        State(() => GlobalTransferStates.FxInitiating);
        State(() => GlobalTransferStates.FxValidating);
        State(() => GlobalTransferStates.FxConfirming);
        State(() => GlobalTransferStates.FxFailed);
        State(() => GlobalTransferStates.FxRateCompareFailed);
        State(() => GlobalTransferStates.FxQueryTransaction);
        State(() => GlobalTransferStates.TransferRequestValidating);
        State(() => GlobalTransferStates.DepositFxTransferring);
        State(() => GlobalTransferStates.WithdrawFxTransferring);
        State(() => GlobalTransferStates.GlobalTransferCompleted);
        State(() => GlobalTransferStates.GlobalTransferFailed);
        State(() => GlobalTransferStates.FxTransferFailed);
        State(() => GlobalTransferStates.FxTransferInsufficientBalance);
        State(() => GlobalTransferStates.RevertTransferSuccess);
        State(() => GlobalTransferStates.ManualAllocationInprogress);
        State(() => GlobalTransferStates.ManualAllocationFailed);
        State(() => GlobalTransferStates.Refunding);
        State(() => GlobalTransferStates.RefundSucceed);
        State(() => GlobalTransferStates.RefundFailed);

        // Initialize
        Initially(
            When(GlobalTransferRequestReceived)
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.GlobalTransferReceived, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.Received)
        );

        // Received
        // Condition: LogSnapshot completed
        // Responsibility: Request Update GlobalTransfer data
        During(GlobalTransferStates.Received,
            When(LogSnapshot?.Completed)
                .IfElse(ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                    binder => binder.TransitionTo(GlobalTransferStates.PaymentReceivedDataPreparing),
                    binder => binder.TransitionTo(GlobalTransferStates.FxQueryTransaction)
                    ),
            When(LogSnapshot?.Faulted)
                .SendErrorResponse()
        );

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Deposit

        // Pre - PaymentReceivedDataPreparing
        // Responsibility: Request Update GlobalTransfer data
        BeforeEnter(GlobalTransferStates.PaymentReceivedDataPreparing,
            binder => binder.Request(
                PreparePaymentReceivedDataRequest,
                ctx => new PrepareGlobalTransferFxExchangeData(ctx.Saga.CorrelationId, ctx.Saga.RequestedFxRate, ctx.Saga.FxMarkUpRate)
            )
        );

        // PaymentReceivedDataPreparing
        // Condition: PreparePaymentReceivedDataRequest completed
        // Responsibility: Validate Deposit payment request and transition to next state
        During(GlobalTransferStates.PaymentReceivedDataPreparing,
            When(PreparePaymentReceivedDataRequest?.Completed)
                .LogSnapshot()
                .Request(
                    GlobalTransferDepositValidationRequest,
                    ctx
                        => new ValidateGlobalTransferDepositPaymentV2(ctx.Saga.CorrelationId)
                    )
                .TransitionTo(GlobalTransferStates.GlobalTransferPaymentValidating),
            When(PreparePaymentReceivedDataRequest?.Faulted)
                .SendErrorResponse()
        );

        // GlobalTransferPaymentValidating
        // Condition: GlobalTransferDepositValidationRequest completed
        // Responsibility: if success - transition to next state
        //                 if failed - publish failed and transition to failed state
        During(GlobalTransferStates.GlobalTransferPaymentValidating,
            When(GlobalTransferDepositValidationRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(GlobalTransferStates.FxInitiating),
            When(GlobalTransferDepositValidationRequest?.Faulted)
                .Then(context =>
                {
                    context.Saga.FailedReason = $"Unable To GlobalTransferPaymentProcess Failed. Exception: {string.Join(", ", context.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.ValidateGlobalRequest, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.FxFailed)
        );

        // Pre - FxInitiating
        // Responsibility: Request InitiateFx
        BeforeEnter(GlobalTransferStates.FxInitiating,
            binder => binder
                .Request(
                    FxInitiateRequest,
                    ctx => new InitiateFxV2Request(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.TransactionType,
                        ctx.Saga.ExchangeAmount!.Value,
                        ctx.Saga.ExchangeCurrency!.Value
                    )
                )
        );

        // FxInitiating
        // Condition: FxInitiateRequest completed
        // Responsibility: if success - transition to next state
        //                 if failed - transition to FxFailed state
        During(GlobalTransferStates.FxInitiating,
            When(FxInitiateRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.FxInitiateRequestDateTime = ctx.Message.InitiateDateTime;
                    ctx.Saga.FxTransactionId = ctx.Message.TransactionId;
                    ctx.Saga.FxConfirmedExchangeRate = ctx.Message.ConfirmedFxRate;
                    ctx.Saga.ActualFxRate = ctx.Message.ConfirmedFxRate;
                    ctx.Saga.FxConfirmedAmount = ctx.Message.ConfirmedAmount;
                    ctx.Saga.FxConfirmedCurrency = ctx.Message.ConfirmedCurrency;
                    ctx.Saga.FxConfirmedExchangeAmount = ctx.Message.ConfirmedExchangeAmount;
                    ctx.Saga.FxConfirmedExchangeCurrency = ctx.Message.ConfirmedExchangeCurrency;
                })
                .LogSnapshot()
                .TransitionTo(GlobalTransferStates.FxValidating),
            When(FxInitiateRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To InitiateFx. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxInitiate, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.FxFailed)
        );

        // Pre - FxValidating
        // Responsibility: Request ValidateFx
        BeforeEnter(GlobalTransferStates.FxValidating,
            binder => binder.Request(
                FxValidateRequest,
                ctx =>
                    new ValidateFxRequest(
                        ctx.Saga.FxTransactionId!,
                        ctx.Saga.RequestedFxRate,
                        ctx.Saga.FxConfirmedExchangeRate!.Value,
                        ctx.Saga.FxMarkUpRate
                    )
            )
        );

        // FxValidating
        // Condition: FxValidateRequest completed
        // Responsibility: if success - transition to next state
        //                 if failed - transition to FxFailed state
        During(GlobalTransferStates.FxValidating,
            When(FxValidateRequest?.Completed)
                .LogSnapshot()
                .TransitionTo(GlobalTransferStates.FxConfirming),
            When(FxValidateRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"ValidateFxRequest failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .IfElse(
                    ctx => ctx.Message.Exceptions.Any(e => e.ExceptionType == typeof(FxRateDiffOverThresholdException).FullName),
                    binder => binder
                        .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxRateCompare, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalTransferStates.FxRateCompareFailed),
                    binder => binder
                        .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxFailed, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalTransferStates.FxFailed)
                ));

        // Pre - FxConfirming
        // Responsibility: Request ConfirmFx
        BeforeEnter(GlobalTransferStates.FxConfirming,
            binder => binder
                .Request(
                    FxConfirmRequest,
                    ctx => new ConfirmFxRequest(ctx.Saga.FxTransactionId!)
                )
        );

        // FxConfirming
        // Condition: FxConfirmRequest completed
        // Responsibility: if success - transition to next state
        //                 if failed - transition to FxFailed state
        During(GlobalTransferStates.FxConfirming,
            When(FxConfirmRequest?.Completed)
                .Then(ctx => { ctx.Saga.FxConfirmedDateTime = ctx.Message.ConfirmedTime; })
                .LogSnapshot()
                .TransitionTo(GlobalTransferStates.DepositFxTransferring),
            When(FxConfirmRequest?.Faulted)
                .Then(context => { context.Saga.FailedReason = $"Unable To ConfirmFx. Exception: {string.Join(", ", context.Message.Exceptions.Select(e => e.Message))}"; })
                .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxConfirm, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.FxFailed)
        );

        // Pre - DepositFxTransferring
        // Responsibility: Request TransferUsdMoneyFromMainAccountToSubAccountV2
        BeforeEnter(GlobalTransferStates.DepositFxTransferring,
            binder => binder
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                    ifBinder => ifBinder.Request(
                        TransferUsdToSubRequest,
                        ctx => new TransferUsdMoneyFromMainAccountToSubAccountV2(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.GlobalAccount,
                            ctx.Saga.FxConfirmedCurrency!.Value,
                            ctx.Saga.FxConfirmedAmount!.Value,
                            depositTransferFee
                        ))
                )
        );

        // DepositFxTransferring
        // Condition: TransferUsdToSubRequest completed
        // Responsibility: if success - publish success and finalize
        //                 if failed - publish failed and transition to failed state
        During(GlobalTransferStates.DepositFxTransferring,
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
                        new GlobalTransferSuccess(
                            ctx.Saga.CorrelationId,
                            null
                        )
                )
                .SendMetric(Metrics.GlobalTransferSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .Finalize(),
            When(TransferUsdToSubRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Transfer Money From Main Account to SubAccount. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .IfElse(
                    ctx => Array.Exists(ctx.Message.Exceptions, e => e.ExceptionType == typeof(TransferInsufficientBalanceException).FullName),
                    binder => binder
                        .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxInsufficientBalance, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalTransferStates.FxTransferInsufficientBalance),
                    binder => binder
                        .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxTransfer, ctx.Saga.TransactionType))
                        .TransitionTo(GlobalTransferStates.FxTransferFailed)
                )
        );

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Withdraw

        // Pre - FxQueryTransaction
        // Responsibility: Request FxTransaction
        BeforeEnter(GlobalTransferStates.FxQueryTransaction,
            binder => binder
                .Request(
                    FxTransactionRequest,
                    ctx => new RequestFxTransaction(
                        ctx.Saga.TransactionType,
                        ctx.Saga.FxTransactionId!,
                        ctx.Saga.CreatedAt,
                        ctx.Saga.FxMarkUpRate
                    )
                    {
                        ExpectedContractCurrency = ctx.Saga.RequestedCurrency,
                        ExpectedCounterCurrency = Currency.THB,
                    }
                )
        );

        // FxQueryTransaction
        // Condition: FxTransactionRequest completed
        // Responsibility: if success - transition to next state
        //                 if failed - transition to TransferRequestFailed state
        During(GlobalTransferStates.FxQueryTransaction,
            When(FxTransactionRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.RequestedFxRate = ctx.Message.ExchangeRate;
                    ctx.Saga.ActualFxRate = ctx.Message.PreMarkUpRequestedFxRate;
                    ctx.Saga.FxInitiateRequestDateTime = ctx.Message.TransactionDateTime;
                })
                .LogSnapshot()
                .TransitionTo(GlobalTransferStates.TransferRequestValidating),
            When(FxTransactionRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Fetch Fx Transaction. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .SendErrorResponse()
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxInitiate, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.TransferRequestFailed)
        );

        // Pre - TransferRequestValidating
        // Responsibility: Request ValidateGlobalTransferRequest
        BeforeEnter(GlobalTransferStates.TransferRequestValidating,
            binder => binder
                .Request(
                    TransferRequestValidation,
                    ctx =>
                        new ValidateGlobalTransferV2Request(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.TransactionType,
                            ctx.Saga.RequestedFxRate,
                            ctx.Saga.RequestedCurrency,
                            ctx.Saga.RequestedFxCurrency
                        )
                )
        );

        // TransferRequestValidating
        // Condition: TransferRequestValidation completed
        // Responsibility: if success - transition to next state
        //                 if failed - transition to TransferRequestFailed state
        During(GlobalTransferStates.TransferRequestValidating,
            When(TransferRequestValidation?.Completed)
                .LogSnapshot()
                .TransitionTo(GlobalTransferStates.WithdrawFxTransferring),
            When(TransferRequestValidation?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Validate Transfer Request. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .SendMetric(Metrics.GlobalRequestFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxTransferValidation, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.TransferRequestFailed)
        );

        // Pre - WithdrawFxTransferring
        // Responsibility: Request TransferUsdMoneyFromSubAccountToMainAccountV2
        BeforeEnter(GlobalTransferStates.WithdrawFxTransferring,
            binder => binder
                .If(
                    ctx => ctx.Saga.TransactionType == TransactionType.Withdraw,
                    ifBinder => ifBinder.Request(
                        TransferUsdToMainRequest,
                        ctx =>
                            new TransferUsdMoneyFromSubAccountToMainAccountV2(
                                ctx.Saga.CorrelationId,
                                ctx.Saga.GlobalAccount,
                                ctx.Saga.RequestedCurrency,
                                withdrawTransferFee
                            )
                    )
                )
        );

        // WithdrawFxTransferring
        // Condition: TransferUsdToMainRequest completed
        // Responsibility: if success - publish success and finalize
        //                 if failed - publish failed and transition to failed state
        During(GlobalTransferStates.WithdrawFxTransferring,
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
                .Publish(
                    ctx =>
                        new GlobalTransferSuccess(
                            ctx.Saga.CorrelationId,
                            RoundingUtils.RoundExchangeTransaction(
                                TransactionType.Withdraw,
                                ctx.Saga.TransferCurrency!.Value,
                                ctx.Saga.TransferAmount!.Value - ctx.Saga.TransferFee!.Value,
                                Currency.THB,
                                ctx.Saga.RequestedFxRate,
                                roundExchangeRate: false
                            )
                        )
                )
                .SendMetric(Metrics.GlobalTransferSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.GlobalTransferCompleted),
            When(TransferUsdToMainRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = $"Unable To Transfer Money From Sub Account to Main Account. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.GlobalTransferFailed, ctx => new MetricTags(ctx.Saga.Product, null, TagFailedReason.FxTransfer, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.TransferRequestFailed)
        );

        #region TransferFailed

        BeforeEnter(GlobalTransferStates.TransferRequestFailed,
            binder => binder
                .Publish(ctx =>
                    new GlobalTransferFailed(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.TransactionType,
                        ctx.Saga.FailedReason,
                        ctx.Saga.TransactionType == TransactionType.Withdraw ?
                            RoundingUtils.RoundExchangeTransaction(
                                TransactionType.Withdraw,
                                ctx.Saga.RequestedCurrency,
                                ctx.Saga.TransferAmount!.Value - withdrawTransferFee,
                                Currency.THB,
                                ctx.Saga.RequestedFxRate,
                                roundExchangeRate: false
                            ) : null)
                )
            );

        #endregion

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region Refund

        foreach (var state in GlobalTransferStates.GetRefundAllowedStates())
        {
            During(state,
                When(RefundingDeposit)
                    .If(
                        ctx => ctx.Saga.TransactionType == TransactionType.Deposit,
                        binder => binder
                            .LogSnapshot()
                            .TransitionTo(GlobalTransferStates.Refunding)
                    )
            );
        }

        During(GlobalTransferStates.Refunding,
            When(RefundSucceed)
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRefundSuccess, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.RefundSucceed),
            When(RefundFailed)
                .LogSnapshot()
                .SendMetric(Metrics.GlobalRefundFailed, ctx => new MetricTags(ctx.Saga.Product, null, null, ctx.Saga.TransactionType))
                .TransitionTo(GlobalTransferStates.RefundFailed)
        );

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region ManualAllocation

        foreach (var (state, transactionType) in GlobalTransferStates.GetManualAllocationAllowedStates())
        {
            During(state,
                When(GlobalManualAllocationProcessing)
                    .If(
                        ctx => transactionType == null || ctx.Saga.TransactionType == transactionType,
                        binder => binder
                            .LogSnapshot()
                            .TransitionTo(GlobalTransferStates.ManualAllocationInprogress)
                    )
            );
        }

        During(GlobalTransferStates.ManualAllocationInprogress,
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
                    binder => binder.TransitionTo(GlobalTransferStates.RevertTransferSuccess)
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
                .TransitionTo(GlobalTransferStates.ManualAllocationFailed)
        );

        #endregion

    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : GlobalTransferState
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
        where TInstance : GlobalTransferState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this GlobalTransferState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            null,
            state.TransactionType,
            null,
            null,
            null,
            state.Channel!.Value,
            state.Product!.Value,
            null,
            "GlobalTransfer",
            state.CurrentState,
            null,
            null,
            state.ExchangeAmount,
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
            state.RequestedCurrency,
            state.RequestedFxRate,
            state.RequestedFxCurrency,
            state.ExchangeCurrency,
            state.TransferFee,
            state.FxTransactionId,
            state.FxInitiateRequestDateTime,
            state.FxConfirmedDateTime,
            state.ExchangeAmount,
            state.ExchangeCurrency,
            state.FxConfirmedExchangeRate,
            state.FxConfirmedAmount,
            state.FxConfirmedCurrency,
            state.FxMarkUpRate,
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
