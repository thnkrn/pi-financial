using MassTransit;
using Pi.OtpService.IntegrationEvents;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Withdraw;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.Withdraw;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.StateMachines.Withdraw;

public class StateMachine : MassTransitStateMachine<WithdrawState>
{
    public Event<WithdrawRequestReceived>? WithdrawRequestEventReceived { get; set; }
    public Event<OtpValidationSuccess>? OtpValidationReceived { get; private set; }
    public Event<WithdrawOtpValidationNotReceivedSpecific>? OtpValidationNotReceived { get; private set; }
    public Event<WithdrawConfirmationReceived>? WithdrawConfirmationReceived { get; private set; }
    public Request<WithdrawState, LogWithdrawTransaction, LogWithdrawTransactionSuccess>? LogSnapshot { get; set; }
    public Request<WithdrawState, GenerateTransactionNo, TransactionNoGenerated>? GenerateTransactionNoRequest { get; set; }
    public Request<WithdrawState, RequestOtp, RequestOtpSuccess>? RequestOtpValidation { get; private set; }
    public Request<WithdrawState, KkpWithdrawRequest, WithdrawOddSucceed>? KkpWithdrawRequest { get; private set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => WithdrawRequestEventReceived, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(
            () => OtpValidationReceived,
            x =>
            {
                x.CorrelateBy((state, ctx) => state.OtpRequestId == ctx.Message.RequestId && state.OtpRequestRef == ctx.Message.RefNo);
                x.OnMissingInstance(e => e.Discard());
            }
        );
        Event(() => OtpValidationNotReceived, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Event(() => WithdrawConfirmationReceived, x => x.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo));
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => GenerateTransactionNoRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => RequestOtpValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => KkpWithdrawRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => IntegrationEvents.Models.WithdrawState.Received);
        State(() => IntegrationEvents.Models.WithdrawState.TransactionNoGenerating);
        State(() => IntegrationEvents.Models.WithdrawState.RequestingOtpValidation);
        State(() => IntegrationEvents.Models.WithdrawState.RequestingOtpValidationFailed);
        State(() => IntegrationEvents.Models.WithdrawState.WaitingForOtpValidation);
        State(() => IntegrationEvents.Models.WithdrawState.OtpValidationNotReceived);
        State(() => IntegrationEvents.Models.WithdrawState.WaitingForConfirmation);
        State(() => IntegrationEvents.Models.WithdrawState.WithdrawalProcessing);
        State(() => IntegrationEvents.Models.WithdrawState.WithdrawalFailed);

        Initially(
            When(WithdrawRequestEventReceived)
                .Then(ctx =>
                {
                    ctx.Saga.UserId = ctx.Message.UserId;
                    ctx.Saga.AccountCode = ctx.Message.AccountCode;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                    ctx.Saga.RequesterDeviceId = ctx.Message.DeviceId;
                    ctx.Saga.Channel = Channel.OnlineViaKKP;
                    ctx.Saga.TransactionNo = ctx.Message.TransactionNo;
                    ctx.Saga.Product = ctx.Message.Product;
                    ctx.Saga.CreatedAt = DateTime.Now;
                    ctx.Saga.RequestId = ctx.Message.RequestId;
                    ctx.Saga.ResponseAddress = ctx.Message.ResponseAddress ?? string.Empty;
                    ctx.Saga.BankAccountNo = ctx.Message.BankAccountNo;
                    ctx.Saga.BankName = ctx.Message.BankName;
                    ctx.Saga.BankCode = ctx.Message.BankCode;
                })
                .Request(LogSnapshot, ctx => new LogWithdrawTransaction(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.WithdrawReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(IntegrationEvents.Models.WithdrawState.Received),
            When(OtpValidationNotReceived)
                .LogSnapshot()
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .SendMetric(Metrics.WithdrawCancelled, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.OtpValidationNotReceived, null))
                .TransitionTo(IntegrationEvents.Models.WithdrawState.OtpValidationNotReceived)
        );

        During(IntegrationEvents.Models.WithdrawState.Received,
            When(LogSnapshot?.Completed)
                .IfElse(
                    ctx => string.IsNullOrEmpty(ctx.Saga.TransactionNo),
                    binder => binder.TransitionTo(IntegrationEvents.Models.WithdrawState.TransactionNoGenerating),
                    binder => binder.TransitionTo(IntegrationEvents.Models.WithdrawState.RequestingOtpValidation)
                ),
            When(LogSnapshot?.Faulted)
                .SendErrorResponse()
        );

        BeforeEnter(IntegrationEvents.Models.WithdrawState.TransactionNoGenerating,
            binder => binder
                .Request(GenerateTransactionNoRequest,
                    ctx => new GenerateTransactionNo(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.Product,
                        ctx.Saga.Channel,
                        TransactionType.Withdraw,
                        false
                    )
                )
        );
        During(IntegrationEvents.Models.WithdrawState.TransactionNoGenerating,
            When(GenerateTransactionNoRequest?.Completed)
                .Then(ctx => { ctx.Saga.TransactionNo = ctx.Message.TransactionNo; })
                .LogSnapshot()
                .TransitionTo(IntegrationEvents.Models.WithdrawState.RequestingOtpValidation),
            When(GenerateTransactionNoRequest?.Faulted)
                .SendErrorResponse()
        );


        BeforeEnter(IntegrationEvents.Models.WithdrawState.RequestingOtpValidation,
            binder => binder
                .Request(
                    RequestOtpValidation,
                    ctx =>
                        new RequestOtp(
                            ctx.Saga.UserId,
                            ctx.Saga.RequesterDeviceId!.Value
                        )
                )
        );
        During(IntegrationEvents.Models.WithdrawState.RequestingOtpValidation,
            When(RequestOtpValidation?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.OtpRequestRef = ctx.Message.RequestRef;
                    ctx.Saga.OtpRequestId = ctx.Message.RequestId;
                })
                .ThenAsync(async ctx =>
                {
                    var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

                    await responseEndpoint.Send(
                        new TransactionNoWithOtpGenerated(ctx.Saga.TransactionNo!, ctx.Message.OtpRef),
                        callback: context => context.RequestId = ctx.Saga.RequestId
                    );
                })
                .LogSnapshot()
                .TransitionTo(IntegrationEvents.Models.WithdrawState.WaitingForOtpValidation),
            When(RequestOtpValidation?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To Request OTP. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .LogSnapshot()
                .SendErrorResponse()
                .SendMetric(Metrics.WithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Otp, null))
                .TransitionTo(IntegrationEvents.Models.WithdrawState.RequestingOtpValidationFailed)
        );

        During(IntegrationEvents.Models.WithdrawState.WaitingForOtpValidation,
            When(OtpValidationReceived)
                .Then(ctx => { ctx.Saga.OtpConfirmedDateTime = DateTime.Now; })
                .Publish(ctx => new WithdrawOtpValidationSuccess(
                    ctx.Saga.RequestId,
                    ctx.Saga.OtpRequestRef!,
                    ctx.Saga.UserId,
                    ctx.Saga.TransactionNo!,
                    ctx.Saga.Product,
                    ctx.Saga.PaymentDisbursedDateTime,
                    ctx.Saga.PaymentDisbursedAmount,
                    ctx.Saga.CustomerCode,
                    ctx.Saga.AccountCode,
                    ctx.Saga.BankName
                ))
                .LogSnapshot()
                .TransitionTo(IntegrationEvents.Models.WithdrawState.WaitingForConfirmation),
            When(OtpValidationNotReceived)
                .LogSnapshot()
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason = ctx.Message.FailedReason;
                })
                .SendMetric(Metrics.WithdrawCancelled, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.OtpValidationNotReceived, null))
                .TransitionTo(IntegrationEvents.Models.WithdrawState.OtpValidationNotReceived)
        );

        During(IntegrationEvents.Models.WithdrawState.WaitingForConfirmation,
            When(WithdrawConfirmationReceived)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentConfirmedAmount = ctx.Message.ConfirmAmount;
                    ctx.Saga.PaymentDisbursedAmount = ctx.Message.ConfirmAmount - ctx.Message.BankFee;
                    ctx.Saga.BankFee = ctx.Message.BankFee;
                })
                .LogSnapshot()
                .TransitionTo(IntegrationEvents.Models.WithdrawState.WithdrawalProcessing)
        );

        BeforeEnter(IntegrationEvents.Models.WithdrawState.WithdrawalProcessing,
            binder => binder
                .Request(
                    KkpWithdrawRequest,
                    ctx =>
                        new KkpWithdrawRequest(
                            ctx.Saga.AccountCode,
                            ctx.Saga.PaymentDisbursedAmount!.Value,
                            ctx.Saga.BankAccountNo!,
                            ctx.Saga.BankCode!,
                            ctx.Saga.Product,
                            ctx.Saga.TransactionNo!
                        ))
        );
        During(IntegrationEvents.Models.WithdrawState.WithdrawalProcessing,
            When(KkpWithdrawRequest?.Completed)
                .Then(ctx => { ctx.Saga.PaymentDisbursedDateTime = ctx.Message.DateTime; })
                .Publish(ctx =>
                    new WithdrawSuccessEvent(
                        Guid.NewGuid(),
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.PaymentDisbursedAmount!.Value
                    )
                )
                .SendMetric(Metrics.WithdrawAmount, ctx => (double)ctx.Saga.PaymentDisbursedAmount!.Value, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .SendMetric(Metrics.WithdrawSuccess, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .Finalize(),
            When(KkpWithdrawRequest?.Faulted)
                .Then(ctx => { ctx.Saga.FailedReason = $"Unable To Withdraw Money from KKP. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}"; })
                .IfElse(
                    ctx => ctx.Saga.Product == Product.GlobalEquities,
                    binder => binder.Publish(ctx => new WithdrawFailedEvent(
                        Guid.NewGuid(),
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.PaymentDisbursedAmount!.Value,
                        "KKP Withdraw Failed"
                    )),
                    binder => binder.Publish(ctx => new NonGlobalWithdrawFailedEvent(
                        Guid.NewGuid(),
                        ctx.Saga.UserId,
                        ctx.Saga.TransactionNo!,
                        ctx.Saga.Product.ToString(),
                        ctx.Saga.PaymentDisbursedAmount!.Value,
                        "KKP Withdraw Failed"
                )))
                .SendMetric(Metrics.WithdrawFailed, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Kkp, null))
                .TransitionTo(IntegrationEvents.Models.WithdrawState.WithdrawalFailed)
        );
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : WithdrawState
        where TData : class
    {
        return source.ThenAsync(async ctx =>
        {
            var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

            var exceptionInfo = ctx.Message.Exceptions.FirstOrDefault();
            var errorMessage = exceptionInfo?.ExceptionType switch
            {
                "Pi.WalletService.Domain.Exceptions.UserOtpRequestLimitReachedException"
                    => new BusRequestFailed(exceptionInfo, ErrorCodes.UserOtpRequestLimitReached, "UserOtpRequestLimitReached"),
                "Pi.WalletService.Domain.Exceptions.UserOtpVerificationLimitReachedException"
                    => new BusRequestFailed(exceptionInfo, ErrorCodes.UserOtpVerificationLimitReached, "UserOtpVerificationLimitReached"),
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
        where TInstance : WithdrawState
        where TData : class
    {
        return source.Publish(
            ctx => new LogWithdrawTransaction(ctx.Saga.ToSnapshot())
        );
    }

    public static WithdrawTransactionSnapshot ToSnapshot(this WithdrawState state)
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
            state.PaymentDisbursedDateTime,
            state.PaymentDisbursedAmount,
            state.BankName,
            state.BankCode,
            state.BankAccountNo,
            state.FailedReason,
            state.RequesterDeviceId
        );
    }
}