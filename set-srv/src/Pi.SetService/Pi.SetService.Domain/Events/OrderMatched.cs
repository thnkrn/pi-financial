namespace Pi.SetService.Domain.Events;

public record OrderMatched
{
    public required Guid CorrelationId { get; init; }
    public required string Symbol { get; init; }
    public required decimal Volume { get; init; }
    public required decimal Price { get; init; }
    public required DateTime TransactionTime { get; init; }
}
