using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class Position
{
    public string SymbolId => $"{Symbol}.{Venue}";
    public string Symbol { get; init; }
    public string Venue { get; init; }
    public Currency Currency { get; init; }
    public decimal EntireQuantity { get; init; }
    public decimal AvailableQuantity { get; init; }
    public string SymbolType { get; init; }
    public decimal LastPrice { get; init; }
    public decimal MarketValue { get; init; }
    public decimal ConvertedMarketValue { get; init; }
    public decimal AveragePrice { get; init; }
    public decimal Upnl { get; init; }
    public decimal ConvertedUpnl { get; init; }
    public decimal Cost { get; init; }
    public decimal ConvertedCost { get; init; }
    public decimal UpnlPercentage { get; init; }
    public string Logo => Path.Combine(StaticUrl.PiLogoUrl, $"{Venue}/{Symbol}_{Venue}.svg");

    public bool CanSell(decimal quantity)
    {
        return quantity > 0 && quantity <= AvailableQuantity;
    }
}
