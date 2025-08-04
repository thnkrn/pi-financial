namespace Pi.BackofficeService.Domain.Events.Ticket;

public record ExecuteActionFailed(Guid CorrelationId, string FailedResponse);
