namespace Pi.FundMarketData.DomainModels;

public class Distribution
{
    public string DividendPolicy { get; init; }
    public DateTime? ExDivDate { get; init; }
    public DateTime AsOfDate { get; init; }
    public List<HistoricalDividend> HistoricalDividends { get; init; }
}
