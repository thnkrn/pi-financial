namespace Pi.SetService.API.Models;

public class SetAccountAssetsResponse
{
    public required string Symbol { get; init; }
    public required decimal MarketPrice { get; init; }
    public required bool Nvdr { get; init; }
    public required string AssetType { get; init; }
    public required bool IsNew { get; init; }
    public required bool CaFlag { get; init; }
    public required decimal AverageCostPrice { get; init; }
    public required decimal CostValue { get; init; }
    public required decimal MarketValue { get; init; }
    public required decimal Upnl { get; init; }
    public required decimal UpnlPercentage { get; init; }
    public required string Side { get; init; }
    public required decimal AvailableVolume { get; set; }
    public required decimal SellableVolume { get; set; }
    public required List<CaEvent> CaEventList { get; init; }
    public decimal? LendingVolume { get; set; }
    public decimal? RealizedPnl { get; init; }
    public string? StockType { get; init; }
    public string? Logo { get; init; }
    public string? FriendlyName { get; init; }
    public string? InstrumentCategory { get; init; }
}

public class CaEvent
{
    public int EventId { get; set; }
    public required string Date { get; set; }
    public required string CaType { get; set; }
}
