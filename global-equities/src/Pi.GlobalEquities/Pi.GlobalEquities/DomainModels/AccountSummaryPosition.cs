using Pi.Common.CommonModels;

namespace Pi.GlobalEquities.DomainModels;

public class AccountSummaryPosition
{
    public decimal FreeMoney { get; init; }
    public decimal NetAssetValue { get; init; }
    public Currency Currency { get; init; }
    public string AccountId { get; init; }
    public IEnumerable<AvailableCurrency> Currencies { get; init; }
    public IEnumerable<PositionSummary> Positions { get; set; }
}

public class AvailableCurrency
{
    public Currency Currency { get; init; }
    public decimal ConvertedValue { get; init; }
    public decimal Value { get; init; }
}

public class PositionSummary
{
    public string SymbolId => $"{Symbol}.{Venue}";
    public string Symbol { get; init; }
    public string Venue { get; init; }
    public Currency Currency { get; init; }
    public decimal EntireQuantity { get; init; }
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
}
