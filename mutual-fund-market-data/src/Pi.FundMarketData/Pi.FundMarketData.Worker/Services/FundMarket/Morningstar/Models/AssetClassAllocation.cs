using System.Xml.Serialization;
using Pi.FundMarketData.Constants;


namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class AssetClassAllocation
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("AABRP-AssetAllocBondNet")]
    public double BondNet { get; init; }

    [XmlElement("AABRP-AssetAllocCashNet")]
    public double CashNet { get; init; }

    [XmlElement("AABRP-AssetAllocEquityNet")]
    public double EquityNet { get; init; }

    [XmlElement("AABRP-OtherNet")]
    public double OtherNet { get; init; }

    [XmlElement("AABRP-PortfolioDate")]
    public DateTime? PortfolioDate { get; init; }

    public Dictionary<string, double> ToDictionary()
    {
        return new Dictionary<string, double>
        {
            { AssetClass.Bond, BondNet },
            { AssetClass.Cash, CashNet },
            { AssetClass.Equity, EquityNet },
            { AssetClass.Others, OtherNet }
        };
    }
}
