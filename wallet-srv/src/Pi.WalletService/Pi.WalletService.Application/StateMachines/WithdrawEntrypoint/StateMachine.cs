using MassTransit;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.SendEmail;
using Pi.WalletService.Application.Commands.SendNotification;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ATS;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.GlobalTransfer;
using Pi.WalletService.Domain.Events.OddWithdraw;
using Pi.WalletService.Domain.Events.Recovery;
using Pi.WalletService.Domain.Events.UpBack;
using Pi.WalletService.Domain.Events.WithdrawEntrypoint;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.AtsEvents;
using EntrypointState = Pi.WalletService.IntegrationEvents.Models.WithdrawEntrypointState;

namespace Pi.WalletService.Application.StateMachines.WithdrawEntrypoint;

public class StateMachine : MassTransitStateMachine<WithdrawEntrypointState>
{
    public Event<WithdrawEntrypointRequest>? WithdrawEventReceived { get; set; }
    public Event<WithdrawOddValidationSuccessEvent>? WithdrawOddValidationSucceed { get; set; }
    public Event<WithdrawOddValidationFailedEvent>? WithdrawOddValidationFailed { get; set; }
    public Event<WithdrawAtsValidationSuccessEvent>? WithdrawAtsValidationSucceed { get; set; }
    public Event<WithdrawAtsValidationFailedEvent>? WithdrawAtsValidationFailed { get; set; }
    public Event<OtpValidationNotReceived>? WithdrawOtpValidationNotReceived { get; set; }
    public Event<UpBackSuccess>? UpBackSucceed { get; set; }
    public Event<UpBackFailed>? UpBackFailed { get; set; }
    public Event<GlobalTransferSuccess>? GlobalTransferSucceed { get; set; }
    public Event<GlobalTransferFailed>? GlobalTransferFailed { get; set; }
    public Event<WithdrawOddSuccessEvent>? WithdrawOddSucceed { get; set; }
    public Event<WithdrawOddFailedEvent>? WithdrawOddFailed { get; set; }
    public Event<WithdrawAtsSuccessEvent>? WithdrawAtsSucceed { get; set; }
    public Event<WithdrawAtsFailedEvent>? WithdrawAtsFailed { get; set; }
    public Event<RecoverySuccess>? RecoverySucceed { get; set; }
    public Event<GlobalManualAllocationSuccessEvent>? GlobalManualAllocateSucceed { get; set; }
    public Event<UpdateTransactionStatusSuccessEvent>? UpdateTransactionStatusSuccessEvent { get; set; }
    public Event<UpdateTransactionStatusFailedEvent>? UpdateTransactionStatusFailedEvent { get; set; }

    // Requests
    public Request<WithdrawEntrypointState, InitiateWithdrawStateMachine, InitiateWithdrawStateMachineSuccess>?
        InitiateWithdrawStateMachineRequest
    { get; set; }
    public Request<WithdrawEntrypointState, LogActivity, LogActivitySuccess>? LogSnapshot { get; set; }
    public Request<WithdrawEntrypointState, GenerateTransactionNo, TransactionNoGenerated>? GenerateTransactionNoRequest { get; set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => WithdrawEventReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawOddValidationSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawOddValidationFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawAtsValidationSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawAtsValidationFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawOddSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawOddFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawAtsSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawAtsFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpBackSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpBackFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RecoverySucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => WithdrawOtpValidationNotReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GlobalManualAllocateSucceed, x => x.CorrelateById(ctx => ctx.Message.TransactionId));
        Event(() => UpdateTransactionStatusSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpdateTransactionStatusFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Request(() => InitiateWithdrawStateMachineRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GenerateTransactionNoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => EntrypointState.Initiate);
        State(() => EntrypointState.Received);
        State(() => EntrypointState.TransactionNoGenerating);
        State(() => EntrypointState.WithdrawValidating);
        State(() => EntrypointState.WithdrawProcessing);
        State(() => EntrypointState.UpBackProcessing);
        State(() => EntrypointState.GlobalTransferProcessing);
        State(() => EntrypointState.WithdrawSucceed);
        State(() => EntrypointState.WithdrawFailedRequireActionRecovery);
        State(() => EntrypointState.WithdrawFailed);
        State(() => EntrypointState.OtpValidationNotReceived);

        // Initial State
        // responsibility: receive request and initiate withdraw state machine
        Initially(
            When(WithdrawEventReceived)
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.AccountCode = ctx.Message.AccountCode;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.Product = ctx.Message.Product;
                    ctx.Saga.Purpose = ctx.Message.Purpose;
                    ctx.Saga.Channel = ctx.Message.Channel;
                    ctx.Saga.CustomerName = ctx.Message.CustomerName;
                    ctx.Saga.RequestedAmount = ctx.Message.RequestedAmount;
                    ctx.Saga.BankCode = ctx.Message.BankCode;
                    ctx.Saga.BankName = ctx.Message.BankName;
                    ctx.Saga.BankBranchCode = ctx.Message.BankBranchCode;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.BankAccountName = ctx.Message.BankAccountName;
                    ctx.Saga.BankAccountTaxId = ctx.Message.BankAccountTaxId;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.RequesterDeviceId = ctx.Message.DeviceId;
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress!;
                    ctx.Saga.EffectiveDate = ctx.Message.EffectiveDate;
                })
                .Request(InitiateWithdrawStateMachineRequest, ctx => new InitiateWithdrawStateMachine(ctx.Message))
                .TransitionTo(EntrypointState.Initiate)
        );

        // Initiate
        // condition: initiate request is completed
        // responsibility: log snapshot and transition to next state
        During(EntrypointState.Initiate,
            When(InitiateWithdrawStateMachineRequest?.Completed)
                .SendMetric(Metrics.WithdrawReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Withdraw))
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .TransitionTo(EntrypointState.Received),
            When(InitiateWithdrawStateMachineRequest?.Faulted)
                .SendFailedEvent(TagFailedReason.Request)
                .TransitionTo(EntrypointState.WithdrawFailed)
        );

        // Received
        // condition: log snapshot is completed
        // responsibility: request generate transaction no
        During(EntrypointState.Received,
            When(LogSnapshot?.Completed)
                .Request(GenerateTransactionNoRequest, ctx => new GenerateTransactionNo(ctx.Saga.CorrelationId, ctx.Saga.Product, ctx.Saga.Channel, TransactionType.Withdraw, true))
                .TransitionTo(EntrypointState.TransactionNoGenerating)
        );

        // TransactionNoGenerating
        // condition: transaction no is generated
        // responsibility: transition to next state based on transaction type (withdraw)
        During(EntrypointState.TransactionNoGenerating,
            When(GenerateTransactionNoRequest?.Completed)
                .Then(ctx => { ctx.Saga.TransactionNo = ctx.Message.TransactionNo; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.WithdrawValidating),
            When(GenerateTransactionNoRequest?.Faulted)
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.GenerateTransactionNo)
                .TransitionTo(EntrypointState.WithdrawFailed)
        );

        // Pre - WithdrawValidating
        // responsibility: publish event to each Withdraw type (Odd)
        //   - Odd: publish event to OddDeposit
        BeforeEnter(EntrypointState.WithdrawValidating,
            binder => binder
                .If(
                    ctx => ctx.Saga.Channel == Channel.OnlineViaKKP,
                    c => c
                        .Publish(ctx => new OddWithdrawValidationRequest(ctx.Saga.CorrelationId))
                )
                .If(
                    ctx => ctx.Saga.Channel == Channel.ATS,
                    c => c
                        .Publish(ctx => new AtsWithdrawValidationRequest(ctx.Saga.CorrelationId))
                )
        );

        // WithdrawValidating
        // condition: (OnlineViaKKP(ODD)/ATS) withdraw validation event is received and success
        // responsibility: persist saga, log snapshot and transition to next state
        During(EntrypointState.WithdrawValidating,
            When(WithdrawOddValidationSucceed)
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    c => c.TransitionTo(EntrypointState.UpBackProcessing),
                    c => c.TransitionTo(EntrypointState.GlobalTransferProcessing)),
            When(WithdrawAtsValidationSucceed)
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    c => c.TransitionTo(EntrypointState.UpBackProcessing),
                    c => c.TransitionTo(EntrypointState.GlobalTransferProcessing)),
            When(WithdrawOddValidationFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .SendMetric(Metrics.WithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel
                    , TagFailedReason.OtpValidation, TransactionType.Withdraw))
                .TransitionTo(EntrypointState.WithdrawFailed),
            When(WithdrawAtsValidationFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .SendMetric(Metrics.WithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel
                    , TagFailedReason.OtpValidation, TransactionType.Withdraw))
                .TransitionTo(EntrypointState.WithdrawFailed),
            When(WithdrawOtpValidationNotReceived)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .SendMetric(Metrics.WithdrawCancelled,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.OtpValidationNotReceived,
                        TransactionType.Withdraw))
                .LogSnapshot()
                .TransitionTo(EntrypointState.OtpValidationNotReceived)
        );

        // Pre - UpBackProcessing
        // responsibility: publish event to Upback (NonGE)
        BeforeEnter(EntrypointState.UpBackProcessing,
            binder => binder
                .Publish(
                    ctx =>
                        new UpBackRequest(
                            ctx.Saga.CorrelationId
                        )
                )
        );

        // Pre - GlobalTransferProcessing
        // responsibility: publish event to GlobalTransfer (GE)
        BeforeEnter(EntrypointState.GlobalTransferProcessing,
            binder => binder
                .Publish(
                    ctx =>
                        new GlobalTransferRequest(
                            ctx.Saga.CorrelationId
                        )
                )
        );

        // UpBackProcessing
        // condition: (NonGE) UpBack event is received (succeed/failed)
        // responsibility: persist saga, log snapshot and transition to next state (finalize)
        During(EntrypointState.UpBackProcessing,
            When(UpBackSucceed)
                .Then(ctx => { ctx.Saga.ConfirmedAmount = ctx.Saga.RequestedAmount; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.WithdrawProcessing),
            When(UpBackFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                    ctx.Saga.ConfirmedAmount = ctx.Saga.RequestedAmount;
                })
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.UpBack)
                .TransitionTo(EntrypointState.WithdrawFailed)
        );

        // GlobalTransferProcessing
        // condition: (GE) GlobalTransfer event is received (succeed/failed)
        // responsibility: persist saga, log snapshot and transition to next state (finalize)
        During(EntrypointState.GlobalTransferProcessing,
            When(GlobalTransferSucceed)
                .Then(ctx => { ctx.Saga.ConfirmedAmount = ctx.Message.ConfirmedAmount!.Value; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.WithdrawProcessing),
            When(GlobalTransferFailed)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason!;
                    ctx.Saga.ConfirmedAmount = ctx.Message.ConfirmedAmount!.Value;
                })
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.Global)
                .TransitionTo(EntrypointState.WithdrawFailed)
        );

        // Upback or Global process is Completed
        // Withdraw on payment-srv
        BeforeEnter(EntrypointState.WithdrawProcessing, binder => binder
            .If(ctx => ctx.Saga.Channel == Channel.OnlineViaKKP,
                c => c.Publish(ctx => new OddWithdrawRequest(ctx.Saga.CorrelationId, ctx.Saga.ConfirmedAmount))
            )
            .If(ctx => ctx.Saga.Channel == Channel.ATS,
                c => c.Publish(ctx => new AtsWithdrawRequest(ctx.Saga.CorrelationId))
            )
        );

        During(EntrypointState.WithdrawProcessing,
            When(WithdrawOddSucceed)
                .Then(ctx => { ctx.Saga.NetAmount = ctx.Message.Amount; })
                .LogSnapshot()
                .SendSuccessEvent()
                .Finalize(),
            When(WithdrawAtsSucceed)
                .Then(ctx => { ctx.Saga.NetAmount = ctx.Message.Amount; })
                .LogSnapshot()
                .SendSuccessEvent()
                .Finalize(),
            When(WithdrawOddFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.WithdrawFailedRequireActionRecovery)
                .If(ctx => ctx.Saga.Product == Product.GlobalEquities,
                    // GE: Publish Recovery Event (Revert global transaction)
                    binder => binder.Publish(ctx => new InitRecoveryEvent(ctx.Saga.CorrelationId, TransactionType.Withdraw, ctx.Saga.Product))),
            When(WithdrawAtsFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product == Product.Derivatives,
                    c => c.TransitionTo(EntrypointState.WithdrawFailedRequireActionRecovery),
                    c => c.SendFailedEvent(TagFailedReason.WithdrawFailed).TransitionTo(EntrypointState.WithdrawFailed)
                )
        );

        During(EntrypointState.WithdrawFailedRequireActionRecovery,
            When(RecoverySucceed)
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.Recovery)
                .TransitionTo(EntrypointState.WithdrawFailed),
            When(GlobalManualAllocateSucceed)
                .LogSnapshot()
                .Then(ctx =>
                {
                    ctx.Saga.GlobalManualAllocateId = ctx.Message.CorrelationId;
                })
                .SendFailedEvent(TagFailedReason.ManualAllocation)
                .TransitionTo(EntrypointState.WithdrawFailed)
        );

        DuringAny(
            When(UpdateTransactionStatusSuccessEvent)
                .LogSnapshot()
                .Then(ctx => ctx.Saga.FailedReason = "Success by operation")
                .SendSuccessEvent()
                .TransitionTo(EntrypointState.WithdrawSucceed),
            When(UpdateTransactionStatusFailedEvent)
                .LogSnapshot()
                .Then(ctx => ctx.Saga.FailedReason = "Failed by operation")
                .SendFailedEvent(TagFailedReason.OperationSupport)
                .TransitionTo(EntrypointState.WithdrawFailed));
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : WithdrawEntrypointState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static EventActivityBinder<TInstance, TData> SendSuccessEvent<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : WithdrawEntrypointState
        where TData : class
    {
        return source
            .SendMetric(Metrics.WithdrawAmount, ctx => (double)ctx.Saga.ConfirmedAmount,
                ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Withdraw))
            .SendMetric(Metrics.WithdrawSuccess,
                ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Withdraw))
            .Publish(ctx => new DepositWithdrawSuccessNotification(ctx.Saga.CorrelationId))
            .Publish(ctx => new DepositWithdrawToIcEmail(ctx.Saga.CorrelationId, true, null));
    }

    public static EventActivityBinder<TInstance, TData> SendFailedEvent<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source,
        TagFailedReason tagFailedReason
    )
        where TInstance : WithdrawEntrypointState
        where TData : class
    {
        return source
            .SendMetric(Metrics.WithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, tagFailedReason, TransactionType.Withdraw))
            .Publish(ctx => new DepositWithdrawFailedNotification(ctx.Saga.CorrelationId));
    }

    public static ActivityLogSnapshot ToSnapshot(this WithdrawEntrypointState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            state.TransactionNo,
            TransactionType.Withdraw,
            state.UserId,
            state.AccountCode,
            state.CustomerCode,
            state.Channel,
            state.Product,
            state.Purpose,
            "WithdrawEntrypoint",
            state.CurrentState!,
            state.RequestedAmount,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            state.CustomerName,
            state.BankAccountNo,
            state.BankAccountName,
            state.BankAccountTaxId,
            state.BankName,
            state.BankCode,
            state.BankBranchCode,
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
            state.RequestId.ToString(),
            state.RequesterDeviceId.ToString()
        );
    }
}
