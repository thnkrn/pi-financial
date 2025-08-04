namespace Pi.FundMarketData.DomainModels;

public class AssetAllocation
{
    public Dictionary<string, double> AssetClassAllocations { get; init; }
    public Dictionary<string, double> RegionalAllocations { get; init; }
    public Dictionary<string, double> SectorAllocations { get; init; }
    public IEnumerable<KeyValuePair<string, double>> TopHoldings { get; init; }

    public DateTime? AssetClassAsOfDate { get; set; }
    public DateTime? RegionalAsOfDate { get; set; }
    public DateTime? SectorAsOfDate { get; set; }
    public DateTime? TopHoldingsAsOfDate { get; set; }
}
