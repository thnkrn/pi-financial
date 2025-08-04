namespace Pi.Financial.FundService.API.Models;

public record AccountSummaryResponse
{
    public required string CustomerCode { get; init; }
    public required string TradingAccountNo { get; init; }
    public required DateTime AsOfDate { get; init; }
    public required decimal TotalMarketValue { get; init; }
    public required decimal TotalCostValue { get; init; }
    public required decimal TotalUpnl { get; init; }
    public required IEnumerable<SiriusFundAssetResponse> Assets { get; init; }
}
