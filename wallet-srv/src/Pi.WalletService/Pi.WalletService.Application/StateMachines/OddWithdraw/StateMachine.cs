using MassTransit;
using Microsoft.Extensions.Configuration;
using Pi.OtpService.IntegrationEvents;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.OddWithdraw;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;

namespace Pi.WalletService.Application.StateMachines.OddWithdraw;

public interface WithdrawOtpValidationExpired
{
    Guid OddWithdrawStateId { get; }
}

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState>
{
    public Event<OddWithdrawValidationRequest>? OddWithdrawValidationRequestReceived { get; set; }
    public Event<OtpValidationSuccess>? OtpValidationReceived { get; private set; }
    public Event<OddWithdrawRequest>? OddWithdrawRequestReceived { get; private set; }

    public Request<Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState, LogActivity, LogActivitySuccess>?
        LogSnapshot
    { get; private set; }

    public Request<Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState, RequestOtpV2, RequestOtpV2Success>?
        RequestOtpValidation
    { get; private set; }

    public Request<Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState, PaymentWithdrawRequest,
        OddWithdrawSucceed>? PaymentWithdrawRequest
    { get; private set; }

    public Schedule<Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState, WithdrawOtpValidationExpired>?
        OtpValidationExpire
    { get; private set; }

    public StateMachine(IConfiguration configuration)
    {
        var kkpBankFee = Convert.ToDecimal(configuration["Bank:KKP:WithdrawFee"]);
        InstanceState(x => x.CurrentState);
        Event(() => OddWithdrawValidationRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(
            () => OtpValidationReceived,
            x =>
            {
                x.CorrelateBy((state, ctx) =>
                    state.OtpRequestId == ctx.Message.RequestId && state.OtpRequestRef == ctx.Message.RefNo);
                x.OnMissingInstance(e => e.Discard());
            }
        );
        Event(() => OddWithdrawRequestReceived,
            x => x.CorrelateBy((state, ctx) => state.CorrelationId == ctx.Message.CorrelationId));
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => RequestOtpValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => PaymentWithdrawRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => OddWithdrawState.Received);
        State(() => OddWithdrawState.RequestingOtpValidation);
        State(() => OddWithdrawState.RequestingOtpValidationFailed);
        State(() => OddWithdrawState.WaitingForOtpValidation);
        State(() => OddWithdrawState.WaitingForConfirmation);
        State(() => OddWithdrawState.OddWithdrawProcessing);
        State(() => OddWithdrawState.OddWithdrawCompleted);
        State(() => OddWithdrawState.OddWithdrawFailed);
        State(() => OddWithdrawState.OtpValidationNotReceived);
        Schedule(
            () => OtpValidationExpire,
            instance => instance.OtpValidationExpireTokenId,
            s =>
            {
                s.Delay = TimeSpan.FromDays(1);
                s.Received = r => r.CorrelateById(context => context.Message.OddWithdrawStateId);
            });

        // Initial State
        // responsibility: receive request and log snapshot
        Initially(
            When(OddWithdrawValidationRequestReceived)
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.WithdrawOddReceived,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(OddWithdrawState.Received)
        );

        // Received
        // condition: log snapshot is completed
        // responsibility: request otp
        During(OddWithdrawState.Received,
            When(LogSnapshot?.Completed)
                .Request(RequestOtpValidation,
                    ctx => new RequestOtpV2(ctx.Saga.CorrelationId, TransactionType.Withdraw))
                .TransitionTo(OddWithdrawState.RequestingOtpValidation)
        );

        // RequestingOtpValidation
        // condition: otp is requested
        // responsibility: response otp ref to client then wait for otp validation
        During(OddWithdrawState.RequestingOtpValidation,
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
                    context => context.Init<WithdrawOtpValidationExpired>(new
                    {
                        OddWithdrawStateId = context.Saga.CorrelationId
                    }),
                    context => TimeSpan.FromMinutes(15))
                .TransitionTo(OddWithdrawState.WaitingForOtpValidation),
            When(RequestOtpValidation?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Unable To Request OTP. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendErrorResponse()
                .SendMetric(Metrics.WithdrawOddFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Otp, null))
                .TransitionTo(OddWithdrawState.RequestingOtpValidationFailed)
        );

        // Pre - RequestingOtpValidationFailed
        // responsibility: publish failed event
        BeforeEnter(OddWithdrawState.RequestingOtpValidationFailed,
            binder => binder
                .Publish(ctx =>
                    new WithdrawOddValidationFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.FailedReason!
                    )
                )
        );

        // WaitingForOtpValidation
        // condition: otp validation is received
        // responsibility: publish otp validated event then transition to next state
        During(OddWithdrawState.WaitingForOtpValidation,
            When(OtpValidationReceived)
                .Then(ctx => { ctx.Saga.OtpConfirmedDateTime = DateTime.Now; })
                .Publish(ctx => new WithdrawOddValidationSuccessEvent(
                    ctx.Saga.CorrelationId
                ))
                .LogSnapshot()
                .Unschedule(OtpValidationExpire)
                .TransitionTo(OddWithdrawState.WaitingForConfirmation),
            When(OtpValidationExpire?.Received)
                .Then(ctx => { ctx.Saga.FailedReason = "OTP Validation Expired"; })
                .Publish(ctx => new OtpValidationNotReceived(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.FailedReason!
                ))
                .TransitionTo(OddWithdrawState.OtpValidationNotReceived)
        );

        // WaitingForConfirmation
        // condition: withdraw request is received
        // responsibility: set withdraw amount then transition to next state
        During(OddWithdrawState.WaitingForConfirmation,
            When(OddWithdrawRequestReceived)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentDisbursedAmount = ctx.Message.Amount - kkpBankFee;
                    ctx.Saga.Fee = kkpBankFee;
                })
                .LogSnapshot()
                .TransitionTo(OddWithdrawState.OddWithdrawProcessing)
        );

        // Pre - OddWithdrawProcessing
        // responsibility: request withdraw from kkp
        BeforeEnter(OddWithdrawState.OddWithdrawProcessing,
            binder => binder
                .Request(
                    PaymentWithdrawRequest,
                    ctx =>
                        new PaymentWithdrawRequest(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.PaymentDisbursedAmount!.Value
                        ))
        );

        // OddWithdrawProcessing
        // condition: withdraw request is completed
        // responsibility: if success, publish success event
        //                 if failed, publish failed event
        During(OddWithdrawState.OddWithdrawProcessing,
            When(PaymentWithdrawRequest?.Completed)
                .Then(ctx => { ctx.Saga.PaymentDisbursedDateTime = ctx.Message.OddProcessedDateTime; })
                .Publish(ctx =>
                    new WithdrawOddSuccessEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.PaymentDisbursedAmount!.Value
                    )
                )
                .SendMetric(Metrics.WithdrawAmount, ctx => (double)ctx.Saga.PaymentDisbursedAmount!.Value,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .SendMetric(Metrics.WithdrawSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(OddWithdrawState.OddWithdrawCompleted),
            When(PaymentWithdrawRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Unable To Withdraw Money from KKP. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .Publish(ctx => new WithdrawOddFailedEvent(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.FailedReason!
                ))
                .SendMetric(Metrics.WithdrawFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Kkp, null))
                .TransitionTo(OddWithdrawState.OddWithdrawFailed)
        );
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState
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
        where TInstance : Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(
        this Domain.AggregatesModel.OddWithdrawAggregate.OddWithdrawState state)
    {
        return new ActivityLogSnapshot(
            state.CorrelationId,
            null,
            TransactionType.Withdraw,
            null,
            null,
            null,
            state.Channel,
            state.Product,
            null,
            "OddWithdraw",
            state.CurrentState!,
            null,
            null,
            null,
            state.PaymentDisbursedDateTime,
            state.PaymentDisbursedAmount,
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