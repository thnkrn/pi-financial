using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Domain.Events;

public record OrderRejected
{
    public required Guid CorrelationId { get; init; }
    public required DateTime TransactionTime { get; init; }
    public string? Reason { get; init; }
    public Source? Source { get; init; }
}
