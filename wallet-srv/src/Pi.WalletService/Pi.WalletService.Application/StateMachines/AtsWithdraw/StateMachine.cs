using MassTransit;
using Pi.OtpService.IntegrationEvents;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Ats;
using Pi.WalletService.Application.Commands.SendEmail;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ATS;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.AtsEvents;
using AtsWithdrawState = Pi.WalletService.IntegrationEvents.Models.AtsWithdrawState;

namespace Pi.WalletService.Application.StateMachines.AtsWithdraw;

public interface WithdrawAtsOtpValidationExpired
{
    Guid WithdrawAtsStateId { get; }
}

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState>
{
    public Event<AtsWithdrawValidationRequest>? AtsWithdrawValidationRequestReceived { get; set; }
    public Event<OtpValidationSuccess>? OtpValidationReceived { get; private set; }
    public Event<AtsWithdrawRequest>? AtsWithdrawRequestReceived { get; set; }
    public Event<AtsGatewayCallbackSuccessEvent>? GatewayCallbackSuccessEvent { get; set; }
    public Event<AtsGatewayCallbackFailedEvent>? GatewayCallbackFailedEvent { get; set; }

    public Request<Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState, RequestOtpV2, RequestOtpV2Success>?
        RequestOtpValidation
    { get; private set; }

    public Request<Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState, LogActivity, LogActivitySuccess>?
        LogSnapshot
    { get; set; }

    public Request<Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState, WithdrawAtsRequest,
        AtsWithdrawRequestSuccess>? WithdrawAtsRequest
    { get; private set; }

    public Schedule<Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState, WithdrawAtsOtpValidationExpired>?
        OtpValidationExpire
    { get; private set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => AtsWithdrawValidationRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(
            () => OtpValidationReceived,
            x =>
            {
                x.CorrelateBy((state, ctx) =>
                    state.OtpRequestId == ctx.Message.RequestId && state.OtpRequestRef == ctx.Message.RefNo);
                x.OnMissingInstance(e => e.Discard());
            }
        );
        Event(() => AtsWithdrawRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => GatewayCallbackSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => GatewayCallbackFailedEvent, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => RequestOtpValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => WithdrawAtsRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => AtsWithdrawState.Received);
        State(() => AtsWithdrawState.RequestingOtpValidation);
        State(() => AtsWithdrawState.RequestingOtpValidationFailed);
        State(() => AtsWithdrawState.WaitingForOtpValidation);
        State(() => AtsWithdrawState.WaitingForConfirmation);
        State(() => AtsWithdrawState.RequestingWithdrawAts);
        State(() => AtsWithdrawState.WaitingForAtsGatewayConfirmation);
        State(() => AtsWithdrawState.AtsWithdrawFailed);
        State(() => AtsWithdrawState.AtsWithdrawCompleted);
        State(() => AtsWithdrawState.OtpValidationNotReceived);
        Schedule(
            () => OtpValidationExpire,
            instance => instance.OtpValidationExpireTokenId,
            s =>
            {
                s.Delay = TimeSpan.FromDays(1);
                s.Received = r => r.CorrelateById(context => context.Message.WithdrawAtsStateId);
            });

        // Initial State
        // responsibility: receive request and log snapshot
        Initially(
            When(AtsWithdrawValidationRequestReceived)
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.WithdrawAtsReceived,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(AtsWithdrawState.Received)
        );

        // Received
        // condition: log snapshot is completed
        // responsibility: request otp
        During(AtsWithdrawState.Received,
            When(LogSnapshot?.Completed)
                .Request(RequestOtpValidation, ctx => new RequestOtpV2(ctx.Saga.CorrelationId, TransactionType.Withdraw))
                .TransitionTo(AtsWithdrawState.RequestingOtpValidation)
        );

        // RequestingOtpValidation
        // condition: otp is requested
        // responsibility: response otp ref to client then wait for otp validation
        During(AtsWithdrawState.RequestingOtpValidation,
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
                    context => context.Init<WithdrawAtsOtpValidationExpired>(new
                    {
                        WithdrawAtsStateId = context.Saga.CorrelationId
                    }),
                    context => TimeSpan.FromMinutes(15))
                .TransitionTo(AtsWithdrawState.WaitingForOtpValidation),
            When(RequestOtpValidation?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Unable To Request OTP. Exception: {string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .SendErrorResponse()
                .SendMetric(Metrics.WithdrawAtsFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Otp, null))
                .TransitionTo(AtsWithdrawState.RequestingOtpValidationFailed)
        );

        // RequestingOtpValidationFailed
        // condition: otp request is failed
        // responsibility: response error to client then end saga
        BeforeEnter(AtsWithdrawState.RequestingOtpValidationFailed,
            binder => binder
                .Publish(ctx =>
                    new WithdrawAtsValidationFailedEvent(
                        ctx.Saga.CorrelationId,
                        ctx.Saga.FailedReason!
                    )
                )
        );

        // WaitingForOtpValidation
        // condition: otp validation is received
        // responsibility: publish otp validated event then transition to next state
        During(AtsWithdrawState.WaitingForOtpValidation,
            When(OtpValidationReceived)
                .Then(ctx => { ctx.Saga.OtpConfirmedDateTime = DateTime.Now; })
                .Publish(ctx => new WithdrawAtsValidationSuccessEvent(ctx.Saga.CorrelationId))
                .Unschedule(OtpValidationExpire)
                .TransitionTo(AtsWithdrawState.WaitingForConfirmation),
            When(OtpValidationExpire?.Received)
                .Then(ctx => { ctx.Saga.FailedReason = "OTP Validation Expired"; })
                .Publish(ctx => new OtpValidationNotReceived(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.FailedReason!
                ))
                .TransitionTo(AtsWithdrawState.OtpValidationNotReceived)
        );

        // WaitingForConfirmation
        // condition: withdraw request is received
        // responsibility: set withdraw amount then transition to next state
        During(AtsWithdrawState.WaitingForConfirmation,
            When(AtsWithdrawRequestReceived)
                .LogSnapshot()
                .TransitionTo(AtsWithdrawState.RequestingWithdrawAts)
        );

        // Pre - RequestingWithdrawAts
        // responsibility: request withdrawAts to freewill
        BeforeEnter(AtsWithdrawState.RequestingWithdrawAts,
            binder => binder
                .Request(WithdrawAtsRequest, ctx => new WithdrawAtsRequest(ctx.Saga.CorrelationId))
        );

        // RequestingWithdrawAts
        // condition: withdraw ats request is completed
        // responsibility: transition to waiting for gateway callback (freewill) or publish failed event
        During(AtsWithdrawState.RequestingWithdrawAts,
            When(WithdrawAtsRequest?.Completed)
                .Then(ctx => { ctx.Saga.Fee = ctx.Message.Fee; })
                .Publish(ctx =>
                    new WithdrawAtsRequestEmail(
                        ctx.Saga.CorrelationId
                    ))
                .LogSnapshot()
                .TransitionTo(AtsWithdrawState.WaitingForAtsGatewayConfirmation),
            When(WithdrawAtsRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Withdraw ats request failed with error: " +
                        $"{string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .Publish(
                    ctx =>
                        new WithdrawAtsFailedEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.FailedReason!
                        )
                )
                .SendMetric(Metrics.WithdrawAtsFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill,
                        TransactionType.Withdraw))
                .TransitionTo(AtsWithdrawState.AtsWithdrawFailed));

        // WaitingForGatewayCallback
        // condition: gateway callback success or failed
        // responsibility: publish withdraw ats result event based on callback
        During(AtsWithdrawState.WaitingForAtsGatewayConfirmation,
            When(GatewayCallbackSuccessEvent)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentDisbursedDateTime = ctx.Message.TransactionDateTime;
                    ctx.Saga.PaymentDisbursedAmount = ctx.Message.Amount;
                })
                .LogSnapshot()
                .Publish(ctx => new WithdrawAtsSuccessEvent(ctx.Saga.CorrelationId, ctx.Message.Amount))
                .SendMetric(Metrics.WithdrawAtsSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Withdraw))
                .TransitionTo(AtsWithdrawState.AtsWithdrawCompleted),
            When(GatewayCallbackFailedEvent)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Freewill callback update failed. ResultCode: " +
                        $"{string.Join(", ", ctx.Message.ResultCode)}";
                })
                .LogSnapshot()
                .Publish(
                    ctx =>
                        new WithdrawAtsFailedEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.FailedReason!
                        )
                )
                .SendMetric(Metrics.WithdrawAtsFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback,
                        TransactionType.Withdraw))
                .TransitionTo(AtsWithdrawState.AtsWithdrawFailed));
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState
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
        where TInstance : Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this Domain.AggregatesModel.AtsWithdrawAggregate.AtsWithdrawState state)
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
            "AtsWithdraw",
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