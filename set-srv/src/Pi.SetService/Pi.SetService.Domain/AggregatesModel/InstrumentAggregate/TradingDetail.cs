namespace Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

public record TradingDetail
{
    public required decimal Price { get; init; }
    public decimal MarketPrice { get; init; }
    public decimal AvgPrice { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Open { get; init; }
    public decimal PrevClose { get; init; }
    public int Volume { get; init; }
    public decimal Ceiling { get; init; }
    public decimal Floor { get; init; }
}
