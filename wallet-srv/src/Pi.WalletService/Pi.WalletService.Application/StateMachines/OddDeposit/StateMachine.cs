using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.OtpService.IntegrationEvents;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.OddDeposit;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;

namespace Pi.WalletService.Application.StateMachines.OddDeposit;

public interface DepositOtpValidationExpired
{
    Guid OddDepositStateId { get; }
}

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.OddDepositAggregate.OddDepositState>
{
    public Event<OddDepositRequest>? OddDepositRequestReceived { get; set; }
    public Event<OtpValidationSuccess>? OtpValidationReceived { get; private set; }
    public Request<Domain.AggregatesModel.OddDepositAggregate.OddDepositState, RequestOtpV2, RequestOtpV2Success>? RequestOtpValidation { get; private set; }
    public Request<Domain.AggregatesModel.OddDepositAggregate.OddDepositState, LogActivity, LogActivitySuccess>? LogSnapshot { get; set; }

    public Request<Domain.AggregatesModel.OddDepositAggregate.OddDepositState, PaymentDepositOddRequest, OddDepositSucceed>? PaymentDepositOddRequest
    {
        get;
        private set;
    }

    public Schedule<Domain.AggregatesModel.OddDepositAggregate.OddDepositState, DepositOtpValidationExpired>?
        OtpValidationExpire
    { get; private set; }

    public StateMachine(ILogger<StateMachine> logger)
    {
        InstanceState(x => x.CurrentState);
        Event(() => OddDepositRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(
            () => OtpValidationReceived,
            x =>
            {
                x.CorrelateBy((state, ctx) =>
                    state.OtpRequestId == ctx.Message.RequestId && state.OtpRequestRef == ctx.Message.RefNo);
                x.OnMissingInstance(e => e.Discard());
            }
        );
        Request(() => RequestOtpValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => PaymentDepositOddRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => OddDepositState.Received);
        State(() => OddDepositState.RequestingOtpValidation);
        State(() => OddDepositState.WaitingForOtpValidation);
        State(() => OddDepositState.RequestingOtpValidationFailed);
        State(() => OddDepositState.OddDepositProcessing);
        State(() => OddDepositState.OddDepositCompleted);
        State(() => OddDepositState.OddDepositFailed);
        State(() => OddDepositState.OtpValidationNotReceived);
        Schedule(
            () => OtpValidationExpire,
            instance => instance.OtpValidationExpireTokenId,
            s =>
            {
                s.Delay = TimeSpan.FromDays(1);
                s.Received = r => r.CorrelateById(context => context.Message.OddDepositStateId);
            });

        // Initial State
        // responsibility: receive request and log snapshot
        Initially(
            When(OddDepositRequestReceived)
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.DepositOddReceived, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(OddDepositState.Received)
        );

        // Received
        // condition: log snapshot is completed
        // responsibility: request otp
        During(OddDepositState.Received,
            When(LogSnapshot?.Completed)
                .Request(RequestOtpValidation, ctx => new RequestOtpV2(ctx.Saga.CorrelationId, TransactionType.Deposit))
                .TransitionTo(OddDepositState.RequestingOtpValidation)
        );

        // RequestingOtpValidation
        // condition: otp is requested
        // responsibility: response otp ref to client then wait for otp validation
        During(OddDepositState.RequestingOtpValidation,
            When(RequestOtpValidation?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.OtpRequestRef = ctx.Message.RequestRef;
                    ctx.Saga.OtpRequestId = ctx.Message.OtpRequestId;
                })
                .ThenAsync(async ctx =>
                {
                    var responseEndpoint = await ctx.GetSendEndpoint(new Uri(ctx.Saga.ResponseAddress));

                    await responseEndpoint.Send(
                        new TransactionNoWithOtpGenerated(ctx.Message.TransactionNo, ctx.Message.OtpRef),
                        callback: context => context.RequestId = ctx.Message.RequestId
                    );
                })
                .LogSnapshot()
                .Schedule(
                    OtpValidationExpire,
                    context => context.Init<DepositOtpValidationExpired>(new
                    {
                        OddDepositStateId = context.Saga.CorrelationId
                    }),
                    context => TimeSpan.FromMinutes(15))
                .TransitionTo(OddDepositState.WaitingForOtpValidation),
            When(RequestOtpValidation?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Unable To Request OTP. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendErrorResponse()
                .SendMetric(Metrics.DepositOddFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Otp, null))
                .TransitionTo(OddDepositState.RequestingOtpValidationFailed)
        );

        // RequestingOtpValidationFailed
        // condition: otp request is failed
        // responsibility: response error to client then end saga
        BeforeEnter(OddDepositState.RequestingOtpValidationFailed,
            binder => binder
                .Publish(ctx =>
                     new DepositOtpValidationFailed(
                         ctx.Saga.CorrelationId,
                         ctx.Saga.FailedReason!
                         )
                )
        );

        // WaitingForOtpValidation
        // condition: otp validation is received
        // responsibility: publish otp validated event then transition to next state
        During(OddDepositState.WaitingForOtpValidation,
            When(OtpValidationReceived)
                .Then(ctx => { ctx.Saga.OtpConfirmedDateTime = DateTime.Now; })
                .LogSnapshot()
                .Unschedule(OtpValidationExpire)
                .TransitionTo(OddDepositState.OddDepositProcessing),
            When(OtpValidationExpire?.Received)
                .Then(ctx => { ctx.Saga.FailedReason = "OTP Validation Expired"; })
                .Publish(ctx => new OtpValidationNotReceived(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.FailedReason!
                ))
                .TransitionTo(OddDepositState.OtpValidationNotReceived)
        );

        // Pre - OddDepositProcessing
        // responsibility: request deposit to payment-srv
        BeforeEnter(OddDepositState.OddDepositProcessing,
            binder => binder
                .Request(
                    PaymentDepositOddRequest,
                    ctx =>
                        new PaymentDepositOddRequest(
                            ctx.Saga.CorrelationId
                        ))
        );

        // OddDepositProcessing
        // condition: deposit is completed
        // responsibility: publish deposit success event then transition to next state
        During(OddDepositState.OddDepositProcessing,
            When(PaymentDepositOddRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.OddProcessedDateTime;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.PaymentReceivedAmount;
                    ctx.Saga.Fee = ctx.Message.Fee;
                })
                .Publish(ctx =>
                    new DepositOddSuccessEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Message.UserId,
                        ctx.Saga.Product.ToString(),
                        ctx.Message.PaymentReceivedAmount
                    )
                )
                .SendMetric(Metrics.WithdrawAmount, ctx => (double)ctx.Saga.PaymentReceivedAmount!.Value, ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .SendMetric(Metrics.DepositOddSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(OddDepositState.OddDepositCompleted),
            When(PaymentDepositOddRequest?.Faulted)
                .Then(ctx =>
                {
                    var exceptionInfo = ctx.Message.Exceptions.FirstOrDefault();
                    ctx.Saga.FailedReason =
                            $"Unable to deposit ODD from PaymentService Exception: {exceptionInfo?.Message}";

                    bool shouldNotify;
                    string? failedCode;
                    switch (exceptionInfo?.ExceptionType)
                    {
                        case "Pi.WalletService.Domain.Exceptions.FinnetInsufficientBalanceException":
                            shouldNotify = true;
                            failedCode = PaymentErrorCodes.FinnetInsufficientBalance;
                            break;
                        default:
                            shouldNotify = false;
                            failedCode = null;
                            break;
                    }

                    ctx.Publish(
                        new DepositOddFailedEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.FailedReason!,
                            shouldNotify,
                            failedCode
                        )
                    );
                })
                .SendMetric(Metrics.DepositOddFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Kkp, null))
                .TransitionTo(OddDepositState.OddDepositFailed)
        );
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : Domain.AggregatesModel.OddDepositAggregate.OddDepositState
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
        where TInstance : Domain.AggregatesModel.OddDepositAggregate.OddDepositState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this Domain.AggregatesModel.OddDepositAggregate.OddDepositState state)
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
            "OddDeposit",
            state.CurrentState!,
            null,
            null,
            null,
            state.PaymentReceivedDateTime,
            state.PaymentReceivedAmount,
            null,
            state.OtpRequestRef,
            state.OtpRequestId.ToString(),
            state.OtpConfirmedDateTime,
            state.Fee,
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
