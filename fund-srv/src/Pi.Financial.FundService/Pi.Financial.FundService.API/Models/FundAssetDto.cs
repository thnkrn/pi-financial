namespace Pi.Financial.FundService.API.Models;

public record FundAssetResponse
{
    public required string CustCode { get; init; }
    public required decimal MarketPrice { get; init; }
    public required decimal RemainUnit { get; init; }
    public required decimal MarketValue { get; init; }
    public required decimal CostValue { get; init; }
    public required decimal Upnl { get; init; }
    public required decimal UpnlPercentage { get; init; }
    public string? FriendlyName { get; set; }
    public string? InstrumentCategory { get; set; }
    public required string UnitHolderId { get; init; }
}

// TODO: Remove this class and create new DTO when migrated FE from Sirius is done
public record SiriusFundAssetResponse : FundAssetResponse
{
    public required decimal AverageCostPrice { get; init; }
    public required string AsOfDate { get; init; }
    public required string Symbol { get; init; }
    public required string? Logo { get; init; }

    public required decimal RemainAmount { get; init; }
    public required decimal AvailableVolume { get; init; }
}

public record InternalFundAssetResponse : FundAssetResponse
{
    public required DateOnly AsOfDate { get; init; }
    public required string FundCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required decimal Unit { get; init; }
    public required decimal AvgCostPrice { get; init; }
    public required decimal RemainAmount { get; init; }
    public required decimal PendingAmount { get; init; }
    public required decimal PendingUnit { get; init; }
}
