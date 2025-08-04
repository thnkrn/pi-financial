namespace Pi.MarketData.Search.Domain.Models;

public record InstrumentSummary
{
    public required string Venue { get; init; }
    public required string Symbol { get; init; }
    public required string FriendlyName { get; init; }
    public required string Logo { get; init; }
    public required decimal Price { get; init; }
    public decimal PriceChange { get; init; }
    public required decimal PriceChangeRatio { get; init; }
    public required string Currency { get; init; }
    public required InstrumentType Type { get; init; }
    public required InstrumentCategory Category { get; init; }
}