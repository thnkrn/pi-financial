using MassTransit;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Commands.SendEmail;
using Pi.WalletService.Application.Commands.SendNotification;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ATS;
using Pi.WalletService.Domain.Events.DepositEntrypoint;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.GlobalTransfer;
using Pi.WalletService.Domain.Events.OddDeposit;
using Pi.WalletService.Domain.Events.QrDeposit;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Events.UpBack;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.AtsEvents;
using EntrypointState = Pi.WalletService.IntegrationEvents.Models.DepositEntrypointState;

namespace Pi.WalletService.Application.StateMachines.DepositEntrypoint;

public class StateMachine : MassTransitStateMachine<DepositEntrypointState>
{
    // Events
    public Event<DepositEntrypointRequest>? DepositEventReceived { get; set; }
    public Event<DepositQrSuccessEvent>? DepositQrSucceed { get; set; }
    public Event<DepositQrFailedEvent>? DepositQrFailed { get; set; }
    public Event<DepositOddSuccessEvent>? DepositOddSucceed { get; set; }
    public Event<DepositOddFailedEvent>? DepositOddFailed { get; set; }
    public Event<DepositAtsSuccessEvent>? DepositAtsSucceed { get; set; }
    public Event<DepositAtsFailedEvent>? DepositAtsFailed { get; set; }
    public Event<UpBackSuccess>? UpBackSucceed { get; set; }
    public Event<UpBackFailed>? UpBackFailed { get; set; }
    public Event<GlobalTransferSuccess>? GlobalTransferSucceed { get; set; }
    public Event<GlobalTransferFailed>? GlobalTransferFailed { get; set; }
    public Event<DepositRefundSucceed>? RefundSucceed { get; set; }
    public Event<GlobalManualAllocationSuccessEvent>? GlobalManualAllocationSucceed { get; set; }
    public Event<DepositOtpValidationFailed>? DepositOtpValidationFailed { get; set; }
    public Event<PaymentNotReceived>? DepositPaymentNotReceived { get; set; }
    public Event<OtpValidationNotReceived>? OtpValidationNotReceived { get; set; }
    public Event<UpdateTransactionStatusSuccessEvent>? UpdateTransactionStatusSuccessEvent { get; set; }
    public Event<UpdateTransactionStatusFailedEvent>? UpdateTransactionStatusFailedEvent { get; set; }

    // Requests
    public Request<DepositEntrypointState, InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>?
        InitiateDepositStateMachineRequest
    { get; set; }

    public Request<DepositEntrypointState, LogActivity, LogActivitySuccess>? LogSnapshot { get; set; }

    public Request<DepositEntrypointState, GenerateTransactionNo, TransactionNoGenerated>? GenerateTransactionNoRequest
    {
        get;
        set;
    }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => DepositEventReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositQrSucceed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositQrFailed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositOddSucceed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositOddFailed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpBackSucceed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpBackFailed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RefundSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositAtsSucceed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositAtsFailed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => DepositOtpValidationFailed,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GlobalManualAllocationSucceed,
            x => x.CorrelateById(ctx => ctx.Message.TransactionId));
        Event(() => DepositPaymentNotReceived,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => OtpValidationNotReceived,
            x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpdateTransactionStatusSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => UpdateTransactionStatusFailedEvent, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Request(() => InitiateDepositStateMachineRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GenerateTransactionNoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => EntrypointState.Initiate);
        State(() => EntrypointState.Received);
        State(() => EntrypointState.TransactionNoGenerating);
        State(() => EntrypointState.DepositProcessing);
        State(() => EntrypointState.UpBackProcessing);
        State(() => EntrypointState.GlobalTransferProcessing);
        State(() => EntrypointState.DepositFailed);
        State(() => EntrypointState.DepositPaymentNotReceived);

        // Initial State
        // responsibility: receive request and initiate deposit state machine
        Initially(
            When(DepositEventReceived)
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
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.RequesterDeviceId = ctx.Message.DeviceId;
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress!;
                    ctx.Saga.BankCode = ctx.Message.BankCode;
                    ctx.Saga.BankName = ctx.Message.BankName;
                    ctx.Saga.BankBranchCode = ctx.Message.BankBranchCode;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.BankAccountName = ctx.Message.BankAccountName;
                    ctx.Saga.BankAccountTaxId = ctx.Message.BankAccountTaxId;
                    ctx.Saga.EffectiveDate = ctx.Message.EffectiveDate;
                })
                .Request(InitiateDepositStateMachineRequest, ctx => new InitiateDepositStateMachine(ctx.Message))
                .TransitionTo(EntrypointState.Initiate)
        );

        // Initiate
        // condition: initiate request is completed
        // responsibility: log snapshot and transition to next state
        During(EntrypointState.Initiate,
            When(InitiateDepositStateMachineRequest?.Completed)
                .SendMetric(Metrics.DepositReceived,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .TransitionTo(EntrypointState.Received),
            When(InitiateDepositStateMachineRequest?.Faulted)
                .SendFailedEvent(TagFailedReason.Request)
                .TransitionTo(EntrypointState.DepositFailed)
        );

        // Received
        // condition: log snapshot is completed
        // responsibility: request generate transaction no
        During(EntrypointState.Received,
            When(LogSnapshot?.Completed)
                .Request(GenerateTransactionNoRequest,
                    ctx => new GenerateTransactionNo(ctx.Saga.CorrelationId, ctx.Saga.Product, ctx.Saga.Channel,
                        TransactionType.Deposit, true))
                .TransitionTo(EntrypointState.TransactionNoGenerating)
        );

        // TransactionNoGenerating
        // condition: transaction no is generated
        // responsibility: transition to next state based on transaction type (deposit/withdraw)
        During(EntrypointState.TransactionNoGenerating,
            When(GenerateTransactionNoRequest?.Completed)
                .Then(ctx => { ctx.Saga.TransactionNo = ctx.Message.TransactionNo; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.DepositProcessing),
            When(GenerateTransactionNoRequest?.Faulted)
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.GenerateTransactionNo)
                .TransitionTo(EntrypointState.DepositFailed)
        );

        // Pre - DepositProcessing
        // responsibility: publish event to each deposit type (QR/ODD)
        //   - QR: response to client with transaction no then publish event to QrDeposit
        //   - ODD: publish event to OddDeposit
        BeforeEnter(EntrypointState.DepositProcessing,
            binder => binder
                .If(
                    ctx => ctx.Saga.Channel == Channel.QR,
                    c => c
                        .ThenAsync(
                            async ctx =>
                            {
                                var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));
                                await responseEndpoint.Send(
                                    new TransactionNoGenerated(ctx.Saga.TransactionNo!),
                                    callback: context => context.RequestId = ctx.Saga.RequestId
                                );
                            })
                        .Publish(
                            ctx =>
                                new QrDepositRequest(ctx.Saga.CorrelationId, ctx.Saga.TransactionNo!)
                        )
                )
                .If(
                    ctx => ctx.Saga.Channel == Channel.ODD,
                    c => c
                        .Publish(ctx => new OddDepositRequest(ctx.Saga.CorrelationId))
                )
                .If(
                    ctx => ctx.Saga.Channel == Channel.ATS,
                    c => c
                        .Publish(ctx => new AtsDepositRequest(ctx.Saga.CorrelationId))
                )
        );

        // DepositProcessing
        // condition: (QR/ODD/ATS) deposit event is received and success
        // responsibility: persist saga, log snapshot and transition to next state
        During(EntrypointState.DepositProcessing,
            When(DepositQrSucceed)
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    c => c.TransitionTo(EntrypointState.UpBackProcessing),
                    c => c.TransitionTo(EntrypointState.GlobalTransferProcessing)),
            When(DepositOddSucceed)
                .Then(ctx => { ctx.Saga.NetAmount = ctx.Message.NetAmount; })
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    c => c.TransitionTo(EntrypointState.UpBackProcessing),
                    c => c.TransitionTo(EntrypointState.GlobalTransferProcessing)),
            When(DepositAtsSucceed)
                .Then(ctx => { ctx.Saga.NetAmount = ctx.Message.Amount; })
                .LogSnapshot()
                .IfElse(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    c => c.TransitionTo(EntrypointState.UpBackProcessing),
                    c => c.TransitionTo(EntrypointState.GlobalTransferProcessing)),
            When(DepositQrFailed)
                .Then(ctx =>
                {
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.BankName = ctx.Message.BankName;
                    ctx.Saga.BankAccountName = ctx.Message.BankAccountName;
                    ctx.Saga.NetAmount = ctx.Message.Amount;
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.DepositPaymentFailed)
                .TransitionTo(EntrypointState.DepositFailed),
            When(DepositOddFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.Reason; })
                .If(ctx => ctx.Message.ShouldNotify,
                    c => c.Publish(ctx => new DepositWithdrawToIcEmail(ctx.Saga.CorrelationId, false, ctx.Message.FailedCode)))
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.DepositPaymentFailed)
                .TransitionTo(EntrypointState.DepositFailed),
            When(DepositAtsFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.DepositPaymentFailed)
                .TransitionTo(EntrypointState.DepositFailed),
            When(DepositOtpValidationFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .SendMetric(Metrics.DepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.OtpValidation, TransactionType.Deposit))
                .TransitionTo(EntrypointState.DepositFailed),
            When(DepositPaymentNotReceived)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.DepositPaymentNotReceived),
            When(OtpValidationNotReceived)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .TransitionTo(EntrypointState.DepositPaymentNotReceived)
        );

        // Pre - UpBackProcessing
        // responsibility: publish event to Upback (NonGE)
        //   - GE: publish event to GlobalTransfer
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
                        new GlobalTransferRequest(ctx.Saga.CorrelationId)
                )
        );

        // UpBackProcessing
        // condition: (NonGE) UpBack event is received (succeed/failed)
        // responsibility: persist saga, log snapshot and transition to next state (finalize)
        During(EntrypointState.UpBackProcessing,
            When(UpBackSucceed)
                .LogSnapshot()
                .SendSuccessEvent()
                .Finalize(),
            When(UpBackFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason; })
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.UpBack)
                .TransitionTo(EntrypointState.DepositFailed)
        );

        // GlobalTransferProcessing
        // condition: (GE) GlobalTransfer event is received (succeed/failed)
        // responsibility: persist saga, log snapshot and transition to next state (finalize)
        During(EntrypointState.GlobalTransferProcessing,
            When(GlobalTransferSucceed)
                .LogSnapshot()
                .SendSuccessEvent()
                .Finalize(),
            When(GlobalTransferFailed)
                .Then(ctx => { ctx.Saga.FailedReason = ctx.Message.FailedReason!; })
                .LogSnapshot()
                .SendFailedEvent(TagFailedReason.UpBack)
                .TransitionTo(EntrypointState.DepositFailed)
        );

        // Post DepositPaymentNotReceived
        BeforeEnter(EntrypointState.DepositPaymentNotReceived, binder => binder
            .SendMetric(Metrics.DepositCancelled,
                ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.PaymentNotReceived,
                    TransactionType.Deposit))
        );

        During(EntrypointState.DepositPaymentNotReceived,
            When(DepositQrSucceed)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = "Manual confirm QR Callback";
                })
                .LogSnapshot()
                .If(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    c => c.TransitionTo(EntrypointState.UpBackProcessing))
        );

        DuringAny(
            When(UpdateTransactionStatusSuccessEvent)
                .LogSnapshot()
                .Then(ctx => ctx.Saga.FailedReason = "Success by operation")
                .SendSuccessEvent()
                .Finalize(),
            When(UpdateTransactionStatusFailedEvent)
                .LogSnapshot()
                .Then(ctx => ctx.Saga.FailedReason = "Failed by operation")
                .SendFailedEvent(TagFailedReason.OperationSupport)
                .TransitionTo(EntrypointState.DepositFailed));

        #region Refund

        foreach (var state in EntrypointState.GetRefundAllowedStates())
        {
            During(state,
                When(RefundSucceed)
                    .Then(ctx => { ctx.Saga.RefundId = ctx.Message.RefundId; })
                    .LogSnapshot()
                    .SendFailedEvent(TagFailedReason.RefundSuccess)
                    .TransitionTo(EntrypointState.DepositFailed)
            );
        }

        #endregion

        #region GlobalManualAllocate

        During(EntrypointState.GlobalTransferProcessing,
            When(GlobalManualAllocationSucceed)
                .Then(ctx => ctx.Saga.GlobalManualAllocateId = ctx.Message.CorrelationId)
                .SendSuccessEvent()
                .Finalize());

        #endregion
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : DepositEntrypointState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static EventActivityBinder<TInstance, TData> SendSuccessEvent<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : DepositEntrypointState
        where TData : class
    {
        return source
            .SendMetric(Metrics.DepositAmount, ctx => (double)ctx.Saga.RequestedAmount,
                ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
            .SendMetric(Metrics.DepositSuccess,
                ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
            .Publish(ctx => new DepositWithdrawSuccessNotification(ctx.Saga.CorrelationId))
            .Publish(ctx => new DepositWithdrawToIcEmail(ctx.Saga.CorrelationId, true, null));
    }

    public static EventActivityBinder<TInstance, TData> SendFailedEvent<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source,
        TagFailedReason tagFailedReason
    )
        where TInstance : DepositEntrypointState
        where TData : class
    {
        return source
            .SendMetric(Metrics.DepositFailed,
                ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, tagFailedReason, TransactionType.Deposit))
            .Publish(ctx => new DepositWithdrawFailedNotification(ctx.Saga.CorrelationId));
    }

    public static ActivityLogSnapshot ToSnapshot(this DepositEntrypointState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            state.TransactionNo!,
            TransactionType.Deposit,
            state.UserId,
            state.AccountCode,
            state.CustomerCode,
            state.Channel,
            state.Product,
            state.Purpose,
            "DepositEntrypoint",
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