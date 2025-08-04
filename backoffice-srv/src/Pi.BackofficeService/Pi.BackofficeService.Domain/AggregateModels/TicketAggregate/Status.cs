namespace Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

public enum Status
{
    Todo,
    Pending,
    Approved,
    Rejected,
}

public enum TransactionStatus
{
    Pending,
    Success,
    Fail
}
