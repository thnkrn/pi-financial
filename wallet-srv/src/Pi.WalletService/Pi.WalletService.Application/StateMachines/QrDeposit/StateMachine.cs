using MassTransit;
using Pi.BackofficeService.IntegrationEvents;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.QrDeposit;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using QrState = Pi.WalletService.IntegrationEvents.Models.QrDepositState;

namespace Pi.WalletService.Application.StateMachines.QrDeposit;

public interface QrExpired
{
    Guid QrStateId { get; }
}

public class StateMachine : MassTransitStateMachine<QrDepositState>
{
    // Events
    public Event<QrDepositRequest>? QrDepositRequestReceived { get; set; }
    public Event<DepositPaymentCallbackReceived>? PaymentCallbackReceived { get; set; }
    public Event<DepositPaymentFailed>? PaymentFailedReceived { get; set; }
    public Event<ApproveNameMismatch>? TransactionApproved { get; set; }
    public Event<RefundingDeposit>? Refunding { get; set; }
    public Event<RefundSucceedEvent>? RefundSucceed { get; set; }
    public Event<RefundFailedEvent>? RefundFailed { get; set; }

    // Requests
    public Request<QrDepositState, LogActivity, LogActivitySuccess>? LogSnapshot { get; set; }
    public Request<QrDepositState, GenerateDepositQrV2Request, QrCodeGeneratedV2>? GenerateQrRequest { get; set; }

    public Request<QrDepositState, UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>?
        UpdateDepositEntrypointRequest
    { get; set; }

    public Request<QrDepositState, ValidatePaymentNameV2, DepositValidatePaymentNameSucceed>? ValidatePaymentNameRequest
    {
        get;
        set;
    }

    public Request<QrDepositState, ValidatePaymentAmountV2, DepositValidatePaymentAmountSucceed>?
        ValidatePaymentAmountRequest
    { get; set; }

    public Request<QrDepositState, ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>?
        ValidatePaymentSourceRequest
    { get; set; }

    // Schedule
    public Schedule<QrDepositState, QrExpired>? QrExpire { get; private set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => QrDepositRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => PaymentCallbackReceived,
            x => x.CorrelateBy((state, ctx) => state.QrTransactionNo == ctx.Message.TransactionNo));
        Event(() => PaymentFailedReceived,
            x => x.CorrelateBy((state, ctx) => state.QrTransactionNo == ctx.Message.TransactionNo));
        Event(() => TransactionApproved,
            x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => Refunding, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RefundSucceed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => RefundFailed, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GenerateQrRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => UpdateDepositEntrypointRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => ValidatePaymentNameRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => ValidatePaymentSourceRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => ValidatePaymentAmountRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => QrState.Initial);
        State(() => QrState.Received);
        State(() => QrState.QrCodeGenerating);
        State(() => QrState.WaitingForPayment);
        State(() => QrState.DepositEntrypointUpdating);
        State(() => QrState.PaymentNameValidating);
        State(() => QrState.PaymentSourceValidating);
        State(() => QrState.PaymentAmountValidating);
        State(() => QrState.QrDepositCompleted);
        State(() => QrState.QrDepositFailed);
        State(() => QrState.DepositFailedNameMismatch);
        State(() => QrState.DepositFailedInvalidSource);
        State(() => QrState.DepositFailedAmountMismatch);
        State(() => QrState.NameMismatchApproved);
        State(() => QrState.Refunding);
        State(() => QrState.RefundSucceed);
        State(() => QrState.RefundFailed);
        State(() => QrState.PaymentNotReceived);
        Schedule(
            () => QrExpire,
            instance => instance.QrExpireTokenId,
            s =>
            {
                s.Delay = TimeSpan.FromDays(1);
                s.Received = r => r.CorrelateById(context => context.Message.QrStateId);
            });

        Initially(
            When(QrDepositRequestReceived)
                .Then(ctx => { ctx.Saga.TransactionNo = ctx.Message.TransactionNo; })
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.QrDepositReceived,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
                .TransitionTo(QrState.Received)
        );

        During(QrState.Received,
            When(LogSnapshot?.Completed)
                .Request(GenerateQrRequest, ctx => new GenerateDepositQrV2Request(ctx.Saga.CorrelationId))
                .TransitionTo(QrState.QrCodeGenerating)
        );

        During(QrState.QrCodeGenerating,
            When(GenerateQrRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.QrValue = ctx.Message.QrValue;
                    ctx.Saga.QrTransactionRef = ctx.Message.QrTransactionRef;
                    ctx.Saga.QrTransactionNo = ctx.Message.QrTransactionNo;
                    ctx.Saga.DepositQrGenerateDateTime = ctx.Message.QrGenerateDateTime;
                })
                .LogSnapshot()
                .Respond(
                    ctx =>
                        new QrCodeSagaGeneratedResponse(
                            ctx.Message.TransactionNo,
                            ctx.Saga.QrValue!,
                            ctx.Saga.QrTransactionRef!,
                            ctx.Saga.QrTransactionNo!,
                            ctx.Saga.DepositQrGenerateDateTime!.Value
                        )
                )
                .Schedule(
                    QrExpire,
                    context => context.Init<QrExpired>(new
                    {
                        QrStateId = context.Saga.CorrelationId
                    }),
                    context => TimeSpan.FromMinutes(context.Saga.QrCodeExpiredTimeInMinute))
                .TransitionTo(QrState.WaitingForPayment),
            When(GenerateQrRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Unable To GenerateQrCode. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .Publish(
                    ctx =>
                        new DepositQrFailedEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.FailedReason!,
                            null,
                            null,
                            null,
                            0
                        )
                )
                .SendMetric(Metrics.QrDepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.GenerateQr,
                        TransactionType.Deposit))
                .TransitionTo(QrState.QrDepositFailed)
        );

        During(QrState.WaitingForPayment,
            When(PaymentCallbackReceived)
                .Then(ctx =>
                {
                    ctx.Saga.Fee = ctx.Message.BankFee;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.PaymentReceivedAmount;
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.PaymentReceivedDateTime;
                })
                .Unschedule(QrExpire)
                .Request(
                    UpdateDepositEntrypointRequest,
                    ctx =>
                        new UpdateDepositEntrypointRequest(
                            new DepositEntrypointState
                            {
                                CorrelationId = ctx.Saga.CorrelationId,
                                BankAccountNo = ctx.Message.BankAccountNo,
                                BankAccountName = ctx.Message.BankAccountName,
                                BankName = ctx.Message.BankShortName,
                                BankCode = ctx.Message.BankCode,
                                NetAmount = ctx.Message.PaymentReceivedAmount - ctx.Message.BankFee
                            })
                )
                .LogSnapshot()
                .TransitionTo(QrState.DepositEntrypointUpdating),
            When(PaymentFailedReceived)
                .Then(ctx =>
                {
                    ctx.Saga.Fee = ctx.Message.BankFee;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.PaymentReceivedAmount;
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.PaymentReceivedDateTime;
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .Publish(ctx =>
                    new DepositQrFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.FailedReason!,
                        ctx.Message.BankName,
                        ctx.Message.BankAccountNo,
                        ctx.Message.BankAccountName,
                        ctx.Saga.PaymentReceivedAmount!.Value
                    )
                )
                .LogSnapshot()
                .SendMetric(Metrics.QrDepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.DepositPaymentFailed,
                        TransactionType.Deposit))
                .TransitionTo(QrState.QrDepositFailed),
            When(QrExpire?.Received)
                .Then(ctx => ctx.Saga.FailedReason = "Qr Payment Not Received")
                .Publish(ctx =>
                    new PaymentNotReceived(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.FailedReason!
                    )
                )
                .SendMetric(Metrics.QrDepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.DepositPaymentFailed,
                        TransactionType.Deposit))
                .TransitionTo(QrState.PaymentNotReceived)
        );

        During(QrState.DepositEntrypointUpdating,
            When(UpdateDepositEntrypointRequest?.Completed)
                .Request(ValidatePaymentSourceRequest, ctx => new ValidatePaymentSourceV2(ctx.Saga.CorrelationId))
                .TransitionTo(QrState.PaymentSourceValidating)
        );

        During(
            QrState.PaymentSourceValidating,
            When(ValidatePaymentSourceRequest?.Completed)
                .LogSnapshot()
                .Request(ValidatePaymentNameRequest, ctx => new ValidatePaymentNameV2(ctx.Saga.CorrelationId))
                .TransitionTo(QrState.PaymentNameValidating),
            When(ValidatePaymentSourceRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Verify Payment Source Failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.QrDepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.InvalidSource,
                        TransactionType.Deposit))
                .TransitionTo(QrState.DepositFailedInvalidSource)
        );

        During(
            QrState.PaymentNameValidating,
            When(ValidatePaymentNameRequest?.Completed)
                .LogSnapshot()
                .Request(ValidatePaymentAmountRequest,
                    ctx =>
                        new ValidatePaymentAmountV2(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.PaymentReceivedAmount!.Value
                        )
                )
                .TransitionTo(QrState.PaymentAmountValidating),
            When(ValidatePaymentNameRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Verify Payment Name Failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.QrDepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.NameMismatch, null))
                .TransitionTo(QrState.DepositFailedNameMismatch)
        );

        During(QrState.DepositFailedNameMismatch,
            When(TransactionApproved)
                .TransitionTo(QrState.NameMismatchApproved)
                .LogSnapshot()
                .Request(ValidatePaymentAmountRequest,
                    ctx =>
                        new ValidatePaymentAmountV2(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.PaymentReceivedAmount!.Value
                        )
                )
                .TransitionTo(QrState.PaymentAmountValidating)
        );

        During(
            QrState.PaymentAmountValidating,
            When(ValidatePaymentAmountRequest?.Completed)
                .LogSnapshot()
                .Publish(ctx =>
                    new DepositQrSuccessEvent(
                        ctx.Saga.CorrelationId
                    )
                )
                .SendMetric(Metrics.QrDepositAmount, ctx => (double)ctx.Saga.PaymentReceivedAmount!.Value,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
                .SendMetric(Metrics.QrDepositSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(QrState.QrDepositCompleted),
            When(ValidatePaymentAmountRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Verify Payment Amount Failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendMetric(Metrics.QrDepositFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.AmountMismatch,
                        TransactionType.Deposit))
                .TransitionTo(QrState.DepositFailedAmountMismatch)
        );

        During(QrState.PaymentNotReceived,
            When(PaymentCallbackReceived)
                .Then(ctx =>
                {
                    ctx.Saga.Fee = ctx.Message.BankFee;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.PaymentReceivedAmount;
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.PaymentReceivedDateTime;
                    ctx.Saga.FailedReason = "Manual confirm QR Callback";
                })
                .Request(
                    UpdateDepositEntrypointRequest,
                    ctx =>
                        new UpdateDepositEntrypointRequest(
                            new DepositEntrypointState
                            {
                                CorrelationId = ctx.Saga.CorrelationId,
                                BankAccountNo = ctx.Message.BankAccountNo,
                                BankAccountName = ctx.Message.BankAccountName,
                                BankName = ctx.Message.BankShortName,
                                BankCode = ctx.Message.BankCode,
                                NetAmount = ctx.Message.PaymentReceivedAmount - ctx.Message.BankFee
                            })
                )
                .LogSnapshot()
                .TransitionTo(QrState.DepositEntrypointUpdating)
        );

        # region refund

        foreach (var state in QrState.GetRefundAllowedStates())
        {
            During(state, When(Refunding).TransitionTo(QrState.Refunding));
        }

        During(QrState.Refunding,
            When(RefundSucceed)
                .Publish(ctx => new DepositRefundSucceedEventV2(ctx.Saga.CorrelationId,
                    ctx.Saga.PaymentReceivedAmount!.Value, ctx.Message.RefundAmount))
                .TransitionTo(QrState.RefundSucceed)
                .SendMetric(Metrics.DepositRefundSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Refund))
                .LogSnapshot(),
            When(RefundFailed)
                .SendMetric(Metrics.DepositRefundFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Refund))
                .TransitionTo(QrState.RefundFailed)
                .LogSnapshot()
        );

        # endregion
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : QrDepositState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this QrDepositState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            null,
            TransactionType.Deposit,
            null,
            null,
            null,
            state.Channel,
            state.Product,
            null,
            "QrDeposit",
            state.CurrentState!,
            null,
            state.PaymentReceivedDateTime ?? null,
            state.PaymentReceivedAmount,
            null,
            null,
            null,
            null,
            null,
            null,
            state.Fee,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            state.DepositQrGenerateDateTime ?? null,
            state.QrTransactionNo,
            state.QrTransactionRef,
            state.QrValue,
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