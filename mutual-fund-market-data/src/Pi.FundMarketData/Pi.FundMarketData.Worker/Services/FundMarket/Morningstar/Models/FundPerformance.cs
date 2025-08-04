using System.Xml.Serialization;
using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class FundPerformance
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("DP-NAVChange")]
    public decimal NAVChange { get; init; }

    [XmlElement("DP-NAVChangePercentage")]
    public double NAVChangePercentage { get; init; }

    [XmlElement("DP-Return3Mth")]
    public double? Return3Mth { get; init; }

    [XmlElement("DP-Return6Mth")]
    public double? Return6Mth { get; init; }

    [XmlElement("DP-Return1Yr")]
    public double? Return1Yr { get; init; }

    [XmlElement("DP-Return3Yr")]
    public double? Return3Yr { get; init; }

    [XmlElement("DP-Return5Yr")]
    public double? Return5Yr { get; init; }

    [XmlElement("DP-ReturnYTD")]
    public double? ReturnYTD { get; init; }

    [XmlElement("DP-ReturnSinceInception")]
    public double? ReturnSinceInception { get; init; }

    [XmlElement("YLD-Yield1Yr")]
    public double? Yield1Yr { get; init; }
    [XmlElement("DP-DayEndNAV")]
    public decimal? Nav { get; init; }

    [XmlElement("DP-DayEndDate")]
    public DateTime? NavAsOfDate { get; init; }

    public List<HistoricalReturnPercentage> GetHistoricalReturnPercentages()
    {
        return new()
        {
            new(){ Interval = Interval.Over3Months, Value = Return3Mth },
            new(){ Interval = Interval.Over6Months, Value = Return6Mth },
            new(){ Interval = Interval.Over1Year, Value = Return1Yr },
            new(){ Interval = Interval.Over3Years, Value = Return3Yr },
            new(){ Interval = Interval.Over5Years, Value = Return5Yr },
            new(){ Interval = Interval.YearToDate, Value = ReturnYTD },
            new(){ Interval = Interval.SinceInception, Value = ReturnSinceInception }
        };
    }

}
