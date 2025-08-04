namespace Pi.BackofficeService.Domain.Events.Transaction;

public record FailedTransactionEvent(Guid CorrelationId, Guid TransactionId);
