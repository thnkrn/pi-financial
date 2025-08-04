using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Domain.Events.Ticket;

public record TicketPendingEvent(Guid CorrelationId, Guid UserId, Method Method, string? Remark);
