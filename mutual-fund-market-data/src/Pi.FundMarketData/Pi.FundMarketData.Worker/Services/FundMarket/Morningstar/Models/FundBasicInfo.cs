using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class FundBasicInfo
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("FSCBI-MStarID")]
    public string MStarID { get; init; }

    [XmlElement("MR-RatingOverall")]
    public int? RatingOverall { get; init; }

    [XmlElement("IC-InvestmentStrategy")]
    public string InvestmentStrategy { get; init; }

    [XmlElement("FSCBI-ProviderCompanyName")]
    public string ProviderCompanyName { get; init; }

    [XmlElement("FSCBI-CategoryName")]
    public string CategoryName { get; init; }

    [XmlElement("FSCBI-InceptionDate")]
    public DateTime? InceptionDate { get; init; }

    [XmlElement("NA-ShareClassNetAssets")]
    public decimal ShareClassNetAssets { get; init; }

    [XmlElement("FSCBI-DistributionFrequency")]
    public string DistributionFrequency { get; init; }

    [XmlElement("DP-DividendDate")]
    public DateTime? DividendDate { get; init; }
}
