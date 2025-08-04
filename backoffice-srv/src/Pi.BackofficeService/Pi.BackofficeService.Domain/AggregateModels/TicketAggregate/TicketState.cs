using MassTransit;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.Exceptions;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

public class TicketState : SagaStateMachineInstance, IAggregateRoot
{
    public TicketState()
    {

    }

    public TicketState(string transactionNo, TransactionType transactionType)
    {
        TransactionNo = transactionNo;
        TransactionType = transactionType;
    }

    public Guid CorrelationId { get; set; }
    public string? TicketNo { get; set; }

    public Guid? TransactionId { get; set; }
    public string TransactionNo { get; set; } = null!;
    public string? TransactionState { get; set; }
    public TransactionType TransactionType { get; set; }
    public string? CurrentState { get; set; }
    public Status? Status { get; set; }
    public Method? RequestAction { get; set; }
    public DateTime? RequestedAt { get; set; }
    public Guid? MakerId { get; set; }
    public string? MakerRemark { get; set; }
    public Method? CheckerAction { get; set; }
    public DateTime? CheckedAt { get; set; }
    public Guid? CheckerId { get; set; }
    public string? CheckerRemark { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerCode { get; set; }
    public Guid? ResponseCodeId { get; set; }
    public string? ResponseAddress { get; set; }
    public Guid? RequestId { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? Payload { get; set; }

    public void Request(Guid userId, Method method, string? remark)
    {
        Status = TicketAggregate.Status.Pending;
        RequestAction = method;
        MakerId = userId;
        MakerRemark = remark;
        RequestedAt = DateTime.UtcNow;
    }

    public void Check(Guid userId, Method method, string? remark)
    {
        if (MakerId == userId)
        {
            throw new ActionNotAllowException("Maker can't check own ticket");
        }

        Status = method == RequestAction ? TicketAggregate.Status.Approved : TicketAggregate.Status.Rejected;
        CheckerAction = method;
        CheckerId = userId;
        CheckerRemark = remark;
        CheckedAt = DateTime.UtcNow;
    }
}
