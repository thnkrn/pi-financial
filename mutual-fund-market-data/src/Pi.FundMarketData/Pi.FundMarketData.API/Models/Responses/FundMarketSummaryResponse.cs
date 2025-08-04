using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.API.Models.Responses;

public class FundMarketSummaryResponse
{
    public string Symbol { get; init; }
    public DateTime AsOfDate { get; init; }
    public int? RiskLevel { get; init; }
    public string Name { get; init; }
    public decimal Nav { get; init; }
    public decimal? ReturnPercentage { get; init; }
    public string Currency { get; init; }
    public string FundCategory { get; init; }
    public string AmcLogo { get; init; }

    public FundMarketSummaryResponse(Fund fund, string logo, Interval interval)
    {
        Symbol = fund.Symbol;
        AsOfDate = fund.Fundamental?.AsOfDate ?? default;
        RiskLevel = fund.Fundamental?.RiskLevel;
        Name = fund.Name;
        Nav = fund.AssetValue?.Nav ?? 0;
        var returnPercentageByInterval = fund.Performance?.HistoricalReturnPercentages?
            .Where(x => x.Interval == interval)
            .Select(x => x.Value)
            .FirstOrDefault();
        ReturnPercentage = (decimal?)returnPercentageByInterval;
        Currency = fund.Fundamental?.Currency;
        FundCategory = fund.Category;
        AmcLogo = logo;
    }
}

public class FundWebMarketSummaryResponse
{
    public string Symbol { get; init; }
    public DateTime AsOfDate { get; init; }
    public int? RiskLevel { get; init; }
    public string Name { get; init; }
    public decimal FundSize { get; init; }
    public decimal Nav { get; init; }
    public decimal Aum { get; init; }
    public decimal? Over3MonthsReturnPercentage { get; init; }
    public decimal? Over6MonthsReturnPercentage { get; init; }
    public decimal? Over1YearReturnPercentage { get; init; }
    public decimal? Over3YearsReturnPercentage { get; init; }
    public decimal? Over5YearsReturnPercentage { get; init; }
    public decimal? YearToDateReturnPercentage { get; init; }
    public int? Rating { get; init; }
    public string Currency { get; init; }
    public string FundCategory { get; init; }
    public string AmcLogo { get; init; }

    public FundWebMarketSummaryResponse(Fund fund, string logo)
    {
        Symbol = fund.Symbol;
        AsOfDate = fund.Fundamental?.AsOfDate ?? default;
        RiskLevel = fund.Fundamental?.RiskLevel;
        Name = fund.Name;
        FundSize = fund.Fundamental?.FundSize ?? 0;
        Nav = fund.AssetValue?.Nav ?? 0;
        Aum = fund.AssetValue?.Aum ?? 0;
        Over3MonthsReturnPercentage = (decimal?)fund.Performance?.HistoricalReturnPercentages
            ?.Where(x => x.Interval == Interval.Over3Months).Select(x => x.Value).FirstOrDefault();
        Over6MonthsReturnPercentage = (decimal?)fund.Performance?.HistoricalReturnPercentages
            ?.Where(x => x.Interval == Interval.Over6Months).Select(x => x.Value).FirstOrDefault();
        Over1YearReturnPercentage = (decimal?)fund.Performance?.HistoricalReturnPercentages
            ?.Where(x => x.Interval == Interval.Over1Year).Select(x => x.Value).FirstOrDefault();
        Over3YearsReturnPercentage = (decimal?)fund.Performance?.HistoricalReturnPercentages
            ?.Where(x => x.Interval == Interval.Over3Years).Select(x => x.Value).FirstOrDefault();
        Over5YearsReturnPercentage = (decimal?)fund.Performance?.HistoricalReturnPercentages
            ?.Where(x => x.Interval == Interval.Over5Years).Select(x => x.Value).FirstOrDefault();
        YearToDateReturnPercentage = (decimal?)fund.Performance?.HistoricalReturnPercentages
            ?.Where(x => x.Interval == Interval.YearToDate).Select(x => x.Value).FirstOrDefault();
        Rating = fund.Rating;
        Currency = fund.Fundamental?.Currency;
        FundCategory = fund.Category;
        AmcLogo = logo;
    }
}
