using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;

namespace Pi.BackofficeService.API.Models;

public record PayloadResponse
{
    public string? Action { get; init; }
    public string? Payload { get; init; }
}