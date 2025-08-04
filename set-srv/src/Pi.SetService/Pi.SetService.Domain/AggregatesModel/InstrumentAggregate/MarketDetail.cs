namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public record MarketDetail
{
    public required string Currency { get; init; }
    public required decimal TradingUnit { get; init; }
    public required decimal Multiplier { get; init; }
}
