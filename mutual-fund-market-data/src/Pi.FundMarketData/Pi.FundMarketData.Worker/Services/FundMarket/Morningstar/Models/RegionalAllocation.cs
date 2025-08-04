using System.Xml.Serialization;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class RegionalAllocation
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("REBRP-EquityRegionAfrica")]
    public double EquityRegionAfrica { get; init; }

    [XmlElement("REBRP-EquityRegionAsiadevLongRescaled")]
    public double EquityRegionAsiadevLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionAsiaemrgLongRescaled")]
    public double EquityRegionAsiaemrgLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionAustralasiaLongRescaled")]
    public double EquityRegionAustralasiaLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionCanada")]
    public double EquityRegionCanada { get; init; }

    [XmlElement("REBRP-EquityRegionDevelopedLongRescaled")]
    public double EquityRegionDevelopedLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionEmergingLongRescaled")]
    public double EquityRegionEmergingLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionEuropeemrgLongRescaled")]
    public double EquityRegionEuropeemrgLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionEuropeexeuro")]
    public double EquityRegionEuropeexeuro { get; init; }

    [XmlElement("REBRP-EquityRegionEurozone")]
    public double EquityRegionEurozone { get; init; }

    [XmlElement("REBRP-EquityRegionJapanLongRescaled")]
    public double EquityRegionJapanLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionLatinAmericaLongRescaled")]
    public double EquityRegionLatinAmericaLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionMiddleEast")]
    public double EquityRegionMiddleEast { get; init; }

    [XmlElement("REBRP-EquityRegionNotClassifiedLongRescaled")]
    public double EquityRegionNotClassifiedLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionUnitedKingdomLongRescaled")]
    public double EquityRegionUnitedKingdomLongRescaled { get; init; }

    [XmlElement("REBRP-EquityRegionUnitedStates")]
    public double EquityRegionUnitedStates { get; init; }

    [XmlElement("REBRP-PortfolioDate")]
    public DateTime? PortfolioDate { get; init; }

    public Dictionary<string, double> ToDictionary()
    {
        return new Dictionary<string, double>
        {
            { EquityRegion.Africa, EquityRegionAfrica },
            { EquityRegion.AsiaDeveloped, EquityRegionAsiadevLongRescaled },
            { EquityRegion.AsiaEmerging, EquityRegionAsiaemrgLongRescaled },
            { EquityRegion.Australasia, EquityRegionAustralasiaLongRescaled },
            { EquityRegion.Canada, EquityRegionCanada },
            { EquityRegion.Developed, EquityRegionDevelopedLongRescaled },
            { EquityRegion.Emerging, EquityRegionEmergingLongRescaled },
            { EquityRegion.EuropeEmerging, EquityRegionEuropeemrgLongRescaled },
            { EquityRegion.EuropeExEuro, EquityRegionEuropeexeuro },
            { EquityRegion.Eurozone, EquityRegionEurozone },
            { EquityRegion.Japan, EquityRegionJapanLongRescaled },
            { EquityRegion.LatinAmerican, EquityRegionLatinAmericaLongRescaled },
            { EquityRegion.MiddleEast, EquityRegionMiddleEast },
            { EquityRegion.NotClassified, EquityRegionNotClassifiedLongRescaled },
            { EquityRegion.UnitedKingdom, EquityRegionUnitedKingdomLongRescaled },
            { EquityRegion.UnitedStates, EquityRegionUnitedStates }
        };
    }

}
