using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Application.Commands.Transaction;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Services.Measurement;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.Events;
using Pi.BackofficeService.Domain.Events.Ticket;
using Pi.BackofficeService.Domain.Exceptions;

namespace Pi.BackofficeService.Application.StateMachines.Ticket;

public class StateMachine : MassTransitStateMachine<TicketState>
{
    public Event<CreateTicketEvent>? CreateTicketEvent { get; private set; }
    public Event<TicketPendingEvent>? TicketPendingEvent { get; private set; }
    public Event<CheckTicketEvent>? CheckTicketEvent { get; private set; }
    public Event<TicketApprovedEvent>? TicketApprovedEvent { get; private set; }

    public Request<TicketState, GenerateTicketNoMessage, TicketNoGeneratedResponse>? GenerateTicketNoRequest { get; set; }
    public Request<TicketState, FetchTransactionMessage, FetchTransactionResponse>? FetchTransactionRequest { get; set; }
    public Request<TicketState, ExecuteTicketActionMessage, ExecuteTicketActionResponse>? ExecuteTicketActionRequest { get; set; }

    public State? Fetching { get; private set; }
    public State? TicketNoGenerating { get; private set; }
    public State? Todo { get; private set; }
    public State? Pending { get; private set; }
    public State? Approved { get; private set; }
    public State? Rejected { get; private set; }
    public State? Executing { get; private set; }
    public State? Success { get; private set; }
    public State? Failed { get; private set; }

    public StateMachine(ILogger<StateMachine> logger, IMetric metric)
    {
        #region Setup
        InstanceState(x => x.CurrentState);
        Event(() => CreateTicketEvent, e =>
        {
            e.SetSagaFactory(ctx => new TicketState(ctx.Message.TransactionNo, ctx.Message.TransactionType)
            {
                CorrelationId = ctx.Message.CorrelationId,
            });
            e.CorrelateBy((state, ctx) => state.TransactionNo == ctx.Message.TransactionNo && !(state.Status == Status.Approved || state.Status == Status.Rejected));
        });
        Event(() => TicketPendingEvent, e => e.CorrelateById(ctx => ctx.Message.CorrelationId));
        Event(() => CheckTicketEvent, e => e.CorrelateBy((state, ctx) => state.TicketNo == ctx.Message.TicketNo));
        Event(() => TicketApprovedEvent, e => e.CorrelateById(ctx => ctx.Message.CorrelationId));
        Request(() => FetchTransactionRequest, e =>
        {
            e.Timeout = TimeSpan.FromSeconds(30);
        });
        Request(() => GenerateTicketNoRequest, e => { e.Timeout = TimeSpan.Zero; });
        Request(() => ExecuteTicketActionRequest, e => { e.Timeout = TimeSpan.Zero; });
        #endregion

        #region Activities
        #region Init State
        Initially(When(CreateTicketEvent)
            .Then(q =>
            {
                q.Saga.ResponseAddress = q.ResponseAddress?.ToString()!;
                q.Saga.RequestId = q.RequestId;
                q.Saga.CreatedAt = DateTime.Now;
                q.Saga.TransactionNo = q.Message.TransactionNo;
                q.Saga.TransactionType = q.Message.TransactionType;
                q.Saga.Payload = q.Message.Payload;
            })
            .Request(FetchTransactionRequest, ctx =>
            {
                return new FetchTransactionMessage(ctx.Saga.TransactionNo, ctx.Saga.TransactionType);
            })
            .Then(_ => metric.Send(Metrics.TicketReceived))
            .TransitionTo(Fetching)
        );
        #endregion

        #region Fetching State
        During(Fetching,
            When(FetchTransactionRequest?.Completed)
                .Then(ctx =>
                {
                    ctx.Saga.TransactionId = ctx.Message.TransactionId;
                    ctx.Saga.TransactionState = ctx.Message.TransactionState;
                    ctx.Saga.CustomerName = ctx.Message.CustomerName;
                    ctx.Saga.ResponseCodeId = ctx.Message.ResponseCodeId;
                    ctx.Saga.CustomerCode = ctx.Message.CustomerCode;
                })
                .Request(GenerateTicketNoRequest, ctx =>
                {
                    return new GenerateTicketNoMessage(ctx.Saga.CorrelationId);
                })
                .TransitionTo(TicketNoGenerating),
            When(FetchTransactionRequest?.Faulted)
                .Then(ctx => ctx.SetCompleted())
                .ThenAsync(async q =>
                {
                    var errorMsg = string.Join(", ", q.Message.Exceptions.Select(e => e.Message));
                    logger.LogError("Ticket {CorrelationId} fetch transaction with error: {ErrorMsg}",
                        q.Saga.CorrelationId.ToString(), errorMsg);
                    await q.SendResponse(new FailedErrorResponse(errorMsg));
                }),
            When(FetchTransactionRequest?.TimeoutExpired)
                .Then(ctx => ctx.SetCompleted())
                .ThenAsync(async q =>
                {
                    logger.LogError("Ticket {CorrelationId} fetch transaction with error: Fetch transaction timeout",
                        q.Saga.CorrelationId.ToString());
                    await q.SendResponse(new FailedErrorResponse("Failed To Retrieved Transaction Info"));
                })
        );
        #endregion

        #region Ticket No Generating State
        During(TicketNoGenerating,
            When(GenerateTicketNoRequest?.Completed)
                .Then(q => q.Saga.Status = Status.Todo)
                .TransitionTo(Todo)
                .ThenAsync(async q =>
                {
                    metric.Send(Metrics.TicketCreated, MetricFactory.NewTag("transaction_state", q.Saga.TransactionState));
                    logger.LogInformation($"Metric from State Machine {Metrics.TicketCreated} Should Sent");
                    await q.SendResponse(q.Message);
                }),
            When(GenerateTicketNoRequest?.Faulted)
                .Then(ctx => ctx.SetCompleted())
                .ThenAsync(async q =>
                {
                    var errorMsg = string.Join(", ", q.Message.Exceptions.Select(e => e.Message));
                    logger.LogError("Ticket {CorrelationId} generate ticket no failed with error: {ErrorMsg}",
                        q.Saga.CorrelationId.ToString(), errorMsg);
                    await q.SendResponse(new FailedErrorResponse(errorMsg));
                })
        );
        #endregion

        #region Todo State
        During(Todo, When(TicketPendingEvent)
            .Then(q =>
            {
                var payload = q.Message;
                q.Saga.Request(payload.UserId, payload.Method, payload.Remark);
            })
            .TransitionTo(Pending)
            .Then(_ =>
            {
                metric.Send(Metrics.TicketPending);
                logger.LogInformation($"Metric from State Machine {Metrics.TicketPending} Should Sent");
            })
            .RespondAsync(q => q.Init<TicketState>(q.Saga))
        );
        #endregion

        #region Pending State
        During(Pending,
            When(CheckTicketEvent)
                .Then(q =>
                {
                    q.Saga.ResponseAddress = q.ResponseAddress?.ToString()!;
                    q.Saga.RequestId = q.RequestId;

                    var payload = q.Message;
                    q.Saga.Check(payload.UserId, payload.Method, payload.Remark);
                })
                .Catch<ActionNotAllowException>(binder => binder.ThenAsync(async q =>
                    {
                        await q.SendResponse(new ExecuteActionFailed(q.Saga.CorrelationId, q.Exception.Message));
                    })
                )
                .Then(q =>
                {
                    if (q.Saga.RequestedAt == null) return;
                    metric.Send(Metrics.TicketRequestToCheckTime, DateTime.Now.Subtract((DateTime)q.Saga.RequestedAt!).Duration().TotalMilliseconds);
                })
                .If(
                    q => q.Saga.Status == Status.Approved,
                    callback => callback
                        .Publish(q => new TicketApprovedEvent(q.Saga.CorrelationId))
                        .Then(_ => metric.Send(Metrics.TicketApproved))
                        .TransitionTo(Approved)
                )
                .If(
                    q => q.Saga.Status == Status.Rejected,
                    callback =>
                    {
                        return callback.TransitionTo(Rejected)
                            .Then(_ => metric.Send(Metrics.TicketRejected))
                            .RespondAsync(q => q.Init<TicketState>(q.Saga));
                    })
        );
        #endregion

        #region Approved State
        During(Approved,
            When(TicketApprovedEvent)
                .Then(q =>
                {
                    q.Saga.ResponseAddress ??= q.ResponseAddress?.ToString()!;
                    q.Saga.RequestId ??= q.RequestId;
                })
                .Request(ExecuteTicketActionRequest, ctx => new ExecuteTicketActionMessage(
                    ctx.Saga.TransactionNo!,
                    ctx.Saga.CustomerCode!,
                    (Method)ctx.Saga.RequestAction!,
                    ctx.Saga.Payload
                ))
                .TransitionTo(Executing)
        );
        #endregion

        #region Excuting State
        During(Executing,
            When(ExecuteTicketActionRequest?.Completed)
            .TransitionTo(Success)
            .Then(q =>
            {
                metric.Send(Metrics.TicketSuccess, MetricFactory.NewTag("Action", q.Saga.CheckerAction.ToString()));
                if (q.Saga.CheckedAt != null)
                {
                    metric.Send(
                        Metrics.TicketExecutionTime,
                        DateTime.Now.Subtract((DateTime)q.Saga.CheckedAt!).Duration().TotalMilliseconds,
                        MetricFactory.NewTag("Action", q.Saga.CheckerAction.ToString())
                    );
                }
            })
            .ThenAsync(async q =>
            {
                await q.SendResponse(q.Saga);
            }),
            When(ExecuteTicketActionRequest?.Faulted)
                .TransitionTo(Failed)
                .Then(q =>
                {
                    metric.Send(Metrics.TicketFailed, MetricFactory.NewTag("Action", q.Saga.CheckerAction.ToString()));
                    if (q.Saga.CheckedAt != null)
                    {
                        metric.Send(
                            Metrics.TicketExecutionTime,
                            DateTime.Now.Subtract((DateTime)q.Saga.CheckedAt!).Duration().TotalMilliseconds,
                            MetricFactory.NewTag("Action", q.Saga.CheckerAction.ToString())
                        );
                    }
                })
                .ThenAsync(async q =>
                {
                    var errorMsg = string.Join(", ", q.Message.Exceptions.Select(e => e.Message));
                    logger.LogError("Ticket {CorrelationId} executed but failed with error: {ErrorMsg}",
                        q.Saga.CorrelationId.ToString(), errorMsg);

                    await q.SendResponse(new ExecuteActionFailed(q.Saga.CorrelationId, errorMsg));
                })
        );
        #endregion
        #endregion
    }
}

public static class BehaviorContextExtension
{
    public static async Task SendResponse<T, TMessage>(this BehaviorContext<TicketState, T> context, TMessage message)
        where T : class
        where TMessage : class
    {
        if (context.Saga.ResponseAddress != null && context.Saga.RequestId != null)
        {
            var endpoint = await context.GetSendEndpoint(new Uri(context.Saga.ResponseAddress!));
            await endpoint.Send(
                message,
                ctx => ctx.RequestId = context.Saga.RequestId
            );
        }
    }
}

