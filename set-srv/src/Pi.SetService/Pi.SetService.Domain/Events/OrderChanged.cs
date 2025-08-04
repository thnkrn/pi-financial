using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;

namespace Pi.SetService.Domain.Events;

public record OrderChanged
{
    public required Guid CorrelationId { get; init; }
    public required decimal Price { get; init; }
    public required int Volume { get; init; }
    public required DateTime TransactionTime { get; init; }
    public Ttf? Ttf { get; init; }
}
