namespace Pi.BackofficeService.Domain.Events.Ticket;

public record TicketNoGeneratedResponse(Guid CorrelationId, string TicketNo);
