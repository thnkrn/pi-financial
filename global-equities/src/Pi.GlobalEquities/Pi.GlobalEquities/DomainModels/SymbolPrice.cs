using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class SymbolPrice
{
    public string Symbol { get; init; }
    public string Venue { get; init; }
    public decimal Price { get; init; }
    public Currency Currency { get; init; }
}
