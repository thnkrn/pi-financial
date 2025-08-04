namespace Pi.FundMarketData.DomainModels;

public class AssetValue
{
    public decimal Aum { get; init; }
    public decimal? Nav { get; init; }
    public decimal TotalUnit { get; init; }
    public NavChange NavChange { get; init; }
    public DateTime? AsOfDate { get; init; }
}
