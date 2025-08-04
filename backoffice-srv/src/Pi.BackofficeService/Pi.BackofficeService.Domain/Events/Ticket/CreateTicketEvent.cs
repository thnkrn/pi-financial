using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;

namespace Pi.BackofficeService.Domain.Events.Ticket;

public record CreateTicketEvent(Guid CorrelationId, string TransactionNo, TransactionType TransactionType, string? Payload);
