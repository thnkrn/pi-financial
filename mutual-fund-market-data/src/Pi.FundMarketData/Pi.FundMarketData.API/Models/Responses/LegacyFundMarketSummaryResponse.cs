using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.API.Models.Responses;

public class LegacyFundMarketSummaryResponse
{
    public string Venue { get; init; } = "Fund";
    public string Symbol { get; init; }
    public string FriendlyName { get; init; }
    public string Logo { get; init; }
    public decimal Price { get; init; }
    public decimal PriceChange { get; init; }
    public double PriceChangeRatio { get; init; }
    public string Currency { get; init; }

    public LegacyFundMarketSummaryResponse(Fund fund, string logo)
    {
        Symbol = fund.Symbol;
        FriendlyName = fund.Name;
        Logo = logo;
        Price = fund.AssetValue?.Nav ?? 0;
        PriceChange = fund.AssetValue?.NavChange?.ValueChange ?? 0;
        PriceChangeRatio = fund.AssetValue?.NavChange?.NavChangePercentage ?? 0;
        Currency = fund.Fundamental?.Currency;
    }
}
