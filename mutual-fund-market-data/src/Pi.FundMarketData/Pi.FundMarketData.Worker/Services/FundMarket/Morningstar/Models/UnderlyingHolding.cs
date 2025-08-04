using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class UnderlyingHolding
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("T25FUH-PortfolioDate")]
    public DateTime? PortfolioDate { get; init; }

    [XmlArray("T25FUH-Holdings")]
    [XmlArrayItem("HoldingDetail")]
    public List<HoldingDetail> Holdings { get; init; }

    public class HoldingDetail
    {
        [XmlElement("HoldingName")]
        public string HoldingName { get; init; }

        [XmlElement("Weighting")]
        public double Weighting { get; init; }
    }

}
