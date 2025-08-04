
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.Domain.Events.Ticket;

public record CheckTicketEvent(string TicketNo, Guid UserId, Method Method, string? Remark);
public record CheckTicketEventResponse(string TicketNo, Guid UserId, Method Method, string? Remark, string? ResponseAddress, Guid? RequestId);
