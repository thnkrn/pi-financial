using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Domain.Events;

public record OrderCancelled
{
    public required Guid CorrelationId { get; init; }
    public required string Symbol { get; init; }
    public required decimal CancelledVolume { get; init; }
    public required Source Source { get; init; }
    public required DateTime TransactionTime { get; init; }
}
