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
using AtsDepositState = Pi.WalletService.IntegrationEvents.Models.AtsDepositState;

namespace Pi.WalletService.Application.StateMachines.AtsDeposit;

public interface DepositAtsOtpValidationExpired
{
    Guid DepositAtsStateId { get; }
}

public class StateMachine : MassTransitStateMachine<Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState>
{
    public Event<AtsDepositRequest>? AtsDepositRequestReceived { get; set; }
    public Event<OtpValidationSuccess>? OtpValidationReceived { get; private set; }
    public Event<AtsGatewayCallbackSuccessEvent>? GatewayCallbackSuccessEvent { get; set; }
    public Event<AtsGatewayCallbackFailedEvent>? GatewayCallbackFailedEvent { get; set; }

    public Request<Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState, RequestOtpV2, RequestOtpV2Success>?
        RequestOtpValidation
    { get; private set; }

    public Request<Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState, LogActivity, LogActivitySuccess>?
        LogSnapshot
    { get; set; }

    public Request<Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState, DepositAtsRequest,
        AtsDepositRequestSuccess>? DepositAtsRequest
    { get; private set; }

    public Schedule<Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState, DepositAtsOtpValidationExpired>?
        OtpValidationExpire
    { get; private set; }

    public StateMachine()
    {
        InstanceState(x => x.CurrentState);
        Event(() => AtsDepositRequestReceived, x => x.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(
            () => OtpValidationReceived,
            x =>
            {
                x.CorrelateBy((state, ctx) =>
                    state.OtpRequestId == ctx.Message.RequestId && state.OtpRequestRef == ctx.Message.RefNo);
                x.OnMissingInstance(e => e.Discard());
            }
        );
        Event(() => GatewayCallbackSuccessEvent, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Event(() => GatewayCallbackFailedEvent, x => x.CorrelateById(ctx => ctx.Message.TicketId));
        Request(() => RequestOtpValidation, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => LogSnapshot, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        Request(() => DepositAtsRequest, x => { x.Timeout = Timeout.InfiniteTimeSpan; });
        State(() => AtsDepositState.Received);
        State(() => AtsDepositState.RequestingOtpValidation);
        State(() => AtsDepositState.WaitingForOtpValidation);
        State(() => AtsDepositState.RequestingOtpValidationFailed);
        State(() => AtsDepositState.AtsDepositFailed);
        State(() => AtsDepositState.RequestingDepositAts);
        State(() => AtsDepositState.WaitingForAtsGatewayConfirmation);
        State(() => AtsDepositState.AtsDepositCompleted);
        State(() => AtsDepositState.OtpValidationNotReceived);
        Schedule(
            () => OtpValidationExpire,
            instance => instance.OtpValidationExpireTokenId,
            s =>
            {
                s.Delay = TimeSpan.FromDays(1);
                s.Received = r => r.CorrelateById(context => context.Message.DepositAtsStateId);
            });

        // Initial State
        // responsibility: receive request and log snapshot
        Initially(
            When(AtsDepositRequestReceived)
                .Request(LogSnapshot, ctx => new LogActivity(ctx.Saga.ToSnapshot()))
                .SendMetric(Metrics.DepositAtsReceived,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, null))
                .TransitionTo(AtsDepositState.Received)
        );

        // Received
        // condition: log snapshot is completed
        // responsibility: request otp
        During(AtsDepositState.Received,
            When(LogSnapshot?.Completed)
                .Request(RequestOtpValidation, ctx => new RequestOtpV2(ctx.Saga.CorrelationId, TransactionType.Deposit))
                .TransitionTo(AtsDepositState.RequestingOtpValidation)
        );

        // RequestingOtpValidation
        // condition: otp is requested
        // responsibility: response otp ref to client then wait for otp validation
        During(AtsDepositState.RequestingOtpValidation,
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
                    context => context.Init<DepositAtsOtpValidationExpired>(new
                    {
                        DepositAtsStateId = context.Saga.CorrelationId
                    }),
                    context => TimeSpan.FromMinutes(15))
                .TransitionTo(AtsDepositState.WaitingForOtpValidation),
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
                .TransitionTo(AtsDepositState.RequestingOtpValidationFailed)
        );

        // RequestingOtpValidationFailed
        // condition: otp request is failed
        // responsibility: response error to client then end saga
        BeforeEnter(AtsDepositState.RequestingOtpValidationFailed,
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
        During(AtsDepositState.WaitingForOtpValidation,
            When(OtpValidationReceived)
                .Then(ctx => { ctx.Saga.OtpConfirmedDateTime = DateTime.Now; })
                .LogSnapshot()
                .Unschedule(OtpValidationExpire)
                .TransitionTo(AtsDepositState.RequestingDepositAts),
            When(OtpValidationExpire?.Received)
                .Then(ctx => { ctx.Saga.FailedReason = "OTP Validation Expired"; })
                .Publish(ctx => new OtpValidationNotReceived(
                    ctx.Saga.CorrelationId,
                    ctx.Saga.FailedReason!
                ))
                .TransitionTo(AtsDepositState.OtpValidationNotReceived)
        );

        // Pre - RequestingDepositAts
        // responsibility: request depositAts to freewill
        BeforeEnter(AtsDepositState.RequestingDepositAts,
            binder => binder
                .Request(DepositAtsRequest, ctx => new DepositAtsRequest(ctx.Saga.CorrelationId))
        );

        // RequestingDepositAts
        // condition: deposit ats request is completed
        // responsibility: transition to waiting for gateway callback (freewill) or publish failed event
        During(AtsDepositState.RequestingDepositAts,
            When(DepositAtsRequest?.Completed)
                .Then(ctx => { ctx.Saga.Fee = ctx.Message.Fee; })
                .Publish(ctx =>
                    new DepositAtsRequestEmail(
                        ctx.Saga.CorrelationId
                    ))
                .LogSnapshot()
                .TransitionTo(AtsDepositState.WaitingForAtsGatewayConfirmation),
            When(DepositAtsRequest?.Faulted)
                .Then(ctx =>
                {
                    ctx.Saga.FailedReason =
                        $"Deposit ats request failed with error: " +
                        $"{string.Join(", ", ctx.Message.Exceptions.Select(e => e.Message))}";
                })
                .LogSnapshot()
                .Publish(
                    ctx =>
                        new DepositAtsFailedEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.FailedReason!
                        )
                )
                .SendMetric(Metrics.DepositAtsFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.Freewill,
                        TransactionType.Deposit))
                .TransitionTo(AtsDepositState.AtsDepositFailed));

        // WaitingForGatewayCallback
        // condition: gateway callback success or failed
        // responsibility: publish deposit ats result event based on callback
        During(AtsDepositState.WaitingForAtsGatewayConfirmation,
            When(GatewayCallbackSuccessEvent)
                .Then(ctx =>
                {
                    ctx.Saga.PaymentReceivedDateTime = ctx.Message.TransactionDateTime;
                    ctx.Saga.PaymentReceivedAmount = ctx.Message.Amount;
                })
                .LogSnapshot()
                .Publish(ctx => new DepositAtsSuccessEvent(ctx.Saga.CorrelationId, ctx.Message.Amount))
                .SendMetric(Metrics.DepositAtsSuccess,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, null, TransactionType.Deposit))
                .TransitionTo(AtsDepositState.AtsDepositCompleted),
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
                        new DepositAtsFailedEvent(
                            ctx.Saga.CorrelationId,
                            ctx.Saga.FailedReason!
                        )
                )
                .SendMetric(Metrics.DepositAtsFailed,
                    ctx => new MetricTags(ctx.Saga.Product, ctx.Saga.Channel, TagFailedReason.FreewillCallback,
                        TransactionType.Deposit))
                .TransitionTo(AtsDepositState.AtsDepositFailed));
    }
}

static class EventActivityBinderExtensions
{
    public static EventActivityBinder<TInstance, Fault<TData>> SendErrorResponse<TInstance, TData>(
        this EventActivityBinder<TInstance, Fault<TData>> source
    )
        where TInstance : Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState
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
        where TInstance : Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState
        where TData : class
    {
        return source.Publish(
            ctx => new LogActivity(ctx.Saga.ToSnapshot())
        );
    }

    public static ActivityLogSnapshot ToSnapshot(this Domain.AggregatesModel.AtsDepositAggregate.AtsDepositState state)
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
            "AtsDeposit",
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