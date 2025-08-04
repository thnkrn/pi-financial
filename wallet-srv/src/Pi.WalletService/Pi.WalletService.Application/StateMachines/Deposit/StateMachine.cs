using MassTransit;
using Pi.BackofficeService.IntegrationEvents;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;

namespace Pi.WalletService.Application.StateMachines.Deposit;

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.DepositAggregate.DepositState>
{
    public Event<DepositRequestReceived>? DepositRequestEventReceived { get; set; }
    public Event<DepositPaymentCallbackReceived>? DepositPaymentCallbackEventReceived { get; set; }
    public Event<DepositPaymentFailed>? DepositPaymentFailedEventReceived { get; set; }
    public Event<DepositTransactionApproved>? DepositTransactionApproved { get; set; }
    public Event<ApproveNameMismatch>? ApproveNameMismatch { get; set; }
    public Event<RefundingDeposit>? RefundingDeposit { get; set; }
    public Event<RefundSucceedEvent>? RefundSucceed { get; set; }
    public Event<RefundFailedEvent>? RefundFailed { get; set; }

    public Request<Domain.AggregatesModel.DepositAggregate.DepositState, LogDepositTransaction, LogDepositTransactionSuccess>? LogSnapshot { get; set; }
    public Request<Domain.AggregatesModel.DepositAggregate.DepositState, GenerateTransactionNo, TransactionNoGenerated>? GenerateTransactionNoRequest { get; set; }
    public Request<Domain.AggregatesModel.DepositAggregate.DepositState, GenerateDepositQrRequest, QrCodeGenerated>? DepositGenerateQrRequest { get; set; }
    public Request<Domain.AggregatesModel.DepositAggregate.DepositState, ValidatePaymentName, DepositValidatePaymentNameSucceed>? DepositValidatePaymentNameRequest { get; set; }
    public Request<Domain.AggregatesModel.DepositAggregate.DepositState, ValidatePaymentSource, DepositValidatePaymentSourceSucceed>? DepositValidatePaymentSourceRequest { get; set; }
    public Request<Domain.AggregatesModel.DepositAggregate.DepositState, ValidatePaymentAmount, DepositValidatePaymentAmountSucceed>? DepositValidatePaymentAmountRequest { get; set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => DepositRequestEventReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => DepositPaymentCallbackEventReceived, x => x.CorrelateBy((state, ctx) => state.QrTransactionNo == ctx.Message.TransactionNo));
        Event(() => DepositPaymentFailedEventReceived, x => x.CorrelateBy((state, ctx) => state.QrTransactionNo == ctx.Message.TransactionNo));
        Event(() => DepositTransactionApproved, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => ApproveNameMismatch, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => RefundingDeposit, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.DepositTransactionNo));
        Event(() => RefundSucceed, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.DepositTransactionNo));
        Event(() => RefundFailed, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.DepositTransactionNo));

        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GenerateTransactionNoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => DepositGenerateQrRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => DepositValidatePaymentNameRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => DepositValidatePaymentSourceRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => DepositValidatePaymentAmountRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });

        State(() => DepositState.Received);
        State(() => DepositState.TransactionNoGenerating);
        State(() => DepositState.DepositQrCodeGenerating);
        State(() => DepositState.DepositWaitingForPayment);
        State(() => DepositState.DepositPaymentReceived);
        State(() => DepositState.DepositPaymentNotReceived);
        State(() => DepositState.DepositPaymentSourceValidating);
        State(() => DepositState.DepositPaymentNameValidating);
        State(() => DepositState.DepositPaymentAmountValidating);
        State(() => DepositState.DepositCompleted);
        State(() => DepositState.DepositFailed);
        State(() => DepositState.DepositFailedAmountMismatch);
        State(() => DepositState.DepositFailedNameMismatch);
        State(() => DepositState.DepositFailedInvalidSource);
        State(() => DepositState.NameMismatchApproved);
        State(() => DepositState.DepositRefunding);
        State(() => DepositState.DepositRefundSucceed);
        State(() => DepositState.DepositRefundFailed);

        Initially(
            When(DepositRequestEventReceived)
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.AccountCode = ctx.Message.AccountCode;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.RequesterDeviceId = ctx.Message.DeviceId;
                    ctx.Saga.Channel = ctx.Message.Channel;
                    ctx.Saga.TransactionNo = ctx.Message.TransactionNo;
                    ctx.Saga.Product = ctx.Message.Product;
                    ctx.Saga.Purpose = Purpose.Collateral;
                    ctx.Saga.RequestedAmount = ctx.Message.RequestedAmount;
                    ctx.Saga.QrCodeExpiredTimeInMinute = ctx.Message.QrCodeExpiredTimeInMinute!.Value;
                    ctx.Saga.CustomerName = $"{ctx.Message.CustomerThaiName} ({ctx.Message.CustomerEnglishName})";
                    ctx.Saga.CreatedAt = DateTime.Now;
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                })
                .Request(LogSnapshot, ctx => new LogDepositTransaction(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.DepositReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(DepositState.Received)
        );

        During(DepositState.Received,
            When(LogSnapshot?.Completed, ctx => string.IsNullOrEmpty(ctx.Saga.TransactionNo))
                .Request(GenerateTransactionNoRequest, ctx => new GenerateTransactionNo(ctx.Saga.CorrelationId, ctx.Saga.Product, ctx.Saga.Channel, TransactionType.Deposit, false))
                .TransitionTo(DepositState.TransactionNoGenerating),
            When(LogSnapshot?.Completed, ctx => !string.IsNullOrEmpty(ctx.Saga.TransactionNo))
                .Request(
                    DepositGenerateQrRequest,
                    ctx => new GenerateDepositQrRequest(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.RequestedAmount,
                        ctx.Saga.AccountCode,
                        ctx.Saga.Product,
                        ctx.Saga.QrCodeExpiredTimeInMinute,
                        ctx.Saga.TransactionNo!))
                .TransitionTo(DepositState.DepositQrCodeGenerating)
        );

        During(DepositState.TransactionNoGenerating,
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
                .Request(
                    DepositGenerateQrRequest,
                    ctx => new GenerateDepositQrRequest(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.RequestedAmount,
                        ctx.Saga.AccountCode,
                        ctx.Saga.Product,
                        ctx.Saga.QrCodeExpiredTimeInMinute,
                        ctx.Saga.TransactionNo!))
                .TransitionTo(DepositState.DepositQrCodeGenerating)
        );

        During(
            DepositState.DepositQrCodeGenerating,
            When(DepositGenerateQrRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.QrValue = ctx.Message.QrValue;
                    ctx.Saga.QrTransactionRef = ctx.Message.QrTransactionRef;
                    ctx.Saga.QrTransactionNo = ctx.Message.QrTransactionNo;
                    ctx.Saga.DepositQrGenerateDateTime = ctx.Message.QrGenerateDateTime;
                })
                .TransitionTo(DepositState.DepositWaitingForPayment)
                .LogSnapshot()
                .Respond(ctx => new QrCodeSagaGeneratedResponse(
                    ctx.Saga.TransactionNo!,
                    ctx.Saga.QrValue!,
                    ctx.Saga.QrTransactionRef!,
                    ctx.Saga.QrTransactionNo!,
                    ctx.Saga.DepositQrGenerateDateTime!.Value)),
            When(DepositGenerateQrRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To GenerateQrCode. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .Publish(ctx =>
                    new DepositFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.Product.ToString(),
                        0))
                .SendMetric(Metrics.DepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.GenerateQr, null))
                .TransitionTo(DepositState.DepositFailed)
        );

        During(DepositState.DepositWaitingForPayment,
            When(DepositPaymentCallbackEventReceived)
                .Then(ctx =>
                {
                    ctx.Saga.CurrentState = DepositState.DepositPaymentReceived?.Name ?? ctx.Saga.CurrentState;
                    ctx.Saga.BankFee = ctx.Message.BankFee;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.PaymentReceivedAmount;
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.PaymentReceivedDateTime;
                    ctx.Saga.BankAccountName = ctx.Message.BankAccountName;
                    ctx.Saga.BankName = ctx.Message.BankShortName;
                    ctx.Saga.BankCode = ctx.Message.BankCode;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.Amount = ctx.Message.PaymentReceivedAmount - ctx.Message.BankFee;
                })
                .LogSnapshot()
                .Request(
                    DepositValidatePaymentSourceRequest,
                    ctx =>
                        new ValidatePaymentSource(
                            ctx.Saga.TransactionNo!,
                            ctx.Saga.BankAccountName!
                        )
                )
                .TransitionTo(DepositState.DepositPaymentSourceValidating),
            When(DepositPaymentFailedEventReceived)
                .Then(ctx =>
                {
                    ctx.Saga.CurrentState = DepositState.DepositFailed?.Name ?? ctx.Saga.CurrentState;
                    ctx.Saga.BankFee = ctx.Message.BankFee;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.PaymentReceivedAmount;
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.PaymentReceivedDateTime;
                    ctx.Saga.CustomerName = ctx.Message.CustomerName;
                    ctx.Saga.BankAccountName = ctx.Message.BankAccountName;
                    ctx.Saga.BankName = ctx.Message.BankName;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .Publish(ctx =>
                    new DepositFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.PaymentReceivedAmount!.Value))
                .SendMetric(Metrics.DepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.DepositPaymentFailed, null))
                .LogSnapshot()
        );

        During(
            DepositState.DepositPaymentSourceValidating,
            When(DepositValidatePaymentSourceRequest?.Completed)
                .LogSnapshot()
                .Request(DepositValidatePaymentNameRequest, ctx => new ValidatePaymentName(ctx.Saga.CustomerCode, ctx.Saga.BankAccountName!))
                .TransitionTo(DepositState.DepositPaymentNameValidating),
            When(DepositValidatePaymentSourceRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = "Verify Payment Source Failed."; })
                .LogSnapshot()
                .SendMetric(Metrics.DepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.InvalidSource, null))
                .TransitionTo(DepositState.DepositFailedInvalidSource)
        );

        During(
            DepositState.DepositPaymentNameValidating,
            When(DepositValidatePaymentNameRequest?.Completed)
                .LogSnapshot()
                .Request(DepositValidatePaymentAmountRequest,
                    ctx =>
                        new ValidatePaymentAmount(
                            ctx.Saga.RequestedAmount,
                            ctx.Saga.PaymentReceivedAmount!.Value
                        )
                )
                .TransitionTo(DepositState.DepositPaymentAmountValidating),
            When(DepositValidatePaymentNameRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = "Verify Payment Name Failed."; })
                .LogSnapshot()
                .SendMetric(Metrics.DepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.NameMismatch, null))
                .TransitionTo(DepositState.DepositFailedNameMismatch)
        );

        During(DepositState.DepositFailedNameMismatch,
            When(DepositTransactionApproved)
                .TransitionTo(DepositState.NameMismatchApproved)
                .LogSnapshot()
                .Request(DepositValidatePaymentAmountRequest,
                    ctx =>
                        new ValidatePaymentAmount(
                            ctx.Saga.RequestedAmount,
                            ctx.Saga.PaymentReceivedAmount!.Value
                        )
                )
                .TransitionTo(DepositState.DepositPaymentAmountValidating)
        );

        During(DepositState.DepositFailedNameMismatch,
            When(ApproveNameMismatch)
                .TransitionTo(DepositState.NameMismatchApproved)
                .LogSnapshot()
                .Request(DepositValidatePaymentAmountRequest,
                    ctx =>
                        new ValidatePaymentAmount(
                            ctx.Saga.RequestedAmount,
                            ctx.Saga.PaymentReceivedAmount!.Value
                        )
                )
                .TransitionTo(DepositState.DepositPaymentAmountValidating)
        );

        During(
            DepositState.DepositPaymentAmountValidating,
            When(DepositValidatePaymentAmountRequest?.Completed)
                .LogSnapshot()
                .If(ctx => ctx.Saga.Product != Product.GlobalEquities,
                    binder =>
                        binder.Publish(c =>
                            new CashDepositRequestReceived(
                                c.Saga.CorrelationId,
                                c.Saga.Purpose,
                                c.Saga.UserId,
                                c.Saga.TransactionNo!,
                                c.Saga.PaymentReceivedDateTime!.Value,
                                c.Saga.Product.ToString(),
                                c.Saga.Amount!.Value,
                                c.Saga.CustomerCode,
                                c.Saga.AccountCode,
                                c.Saga.BankName,
                                c.Saga.Channel))
                )
                .Publish(ctx =>
                    new DepositSuccessEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.PaymentReceivedDateTime!.Value,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.Amount!.Value,
                        ctx.Saga.CustomerCode,
                        ctx.Saga.AccountCode,
                        ctx.Saga.BankName,
                        ctx.Saga.DepositQrGenerateDateTime!.Value,
                        ctx.Saga.Channel)
                )
                .SendMetric(Metrics.DepositAmount, ctx => (double)ctx.Saga.PaymentReceivedAmount!.Value, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .SendMetric(Metrics.DepositSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(DepositState.DepositCompleted),
            When(DepositValidatePaymentAmountRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Verify Payment Amount Failed. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .SendMetric(Metrics.DepositFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.AmountMismatch, null))
                .TransitionTo(DepositState.DepositFailedAmountMismatch)
        );

        #region Refund
        foreach (var state in DepositState.GetRefundAllowedStates())
        {
            During(state, When(RefundingDeposit).TransitionTo(DepositState.DepositRefunding));
        }

        During(DepositState.DepositRefunding,
            When(RefundSucceed)
                .Publish(ctx => new DepositRefundSucceedEvent(ctx.Saga.TransactionNo!, ctx.Saga.PaymentReceivedAmount!.Value, ctx.Message.RefundAmount))
                .TransitionTo(DepositState.DepositRefundSucceed)
                .SendMetric(Metrics.DepositRefundSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .LogSnapshot(),
            When(RefundFailed)
                .SendMetric(Metrics.DepositRefundFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(DepositState.DepositRefundFailed)
                .LogSnapshot()
        );

        #endregion

    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, TData> LogSnapshot<TInstance, TData>(
        this EventActivityBinder<TInstance, TData> source
    )
        where TInstance : Domain.AggregatesModel.DepositAggregate.DepositState
        where TData : class
    {
        return source.Publish(
            ctx => new LogDepositTransaction(ctx.Saga.ToSnapshot())
        );
    }

    public static DepositTransactionSnapshot ToSnapshot(this Domain.AggregatesModel.DepositAggregate.DepositState state)
    {
        return new DepositTransactionSnapshot(
            state.CorrelationId,
            state.UserId,
            state.CurrentState ?? string.Empty,
            state.CustomerCode,
            state.TransactionNo ?? string.Empty,
            state.RequestedAmount,
            state.AccountCode,
            state.Channel,
            state.Product,
            state.Purpose,
            state.BankFee,
            state.DepositQrGenerateDateTime,
            state.QrTransactionNo,
            state.QrTransactionRef,
            state.QrValue,
            state.PaymentReceivedDateTime,
            state.PaymentReceivedAmount,
            state.CustomerName,
            state.BankAccountName,
            state.BankName,
            state.BankCode,
            state.BankAccountNo,
            state.FailedReason,
            state.RequesterDeviceId
        );
    }
}
