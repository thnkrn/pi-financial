using System.Xml.Serialization;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class StockSectorAllocation
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("GSSBRP-EquitySectorBasicMaterialsLongRescaled")]
    public double BasicMaterialsLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorCommunicationServicesLongRescaled")]
    public double CommunicationServicesLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorConsumerCyclicalLongRescaled")]
    public double ConsumerCyclicalLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorConsumerDefensiveLongRescaled")]
    public double ConsumerDefensiveLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorEnergyLongRescaled")]
    public double EnergyLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorFinancialServicesLongRescaled")]
    public double FinancialServicesLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorHealthcareLongRescaled")]
    public double HealthcareLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorIndustrialsLongRescaled")]
    public double IndustrialsLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorRealEstateLongRescaled")]
    public double RealEstateLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorTechnologyLongRescaled")]
    public double TechnologyLongRescaled { get; init; }

    [XmlElement("GSSBRP-EquitySectorUtilitiesLongRescaled")]
    public double UtilitiesLongRescaled { get; init; }

    [XmlElement("GSSBRP-GlobalSectorPortfolioDate")]
    public DateTime? PortfolioDate { get; init; }

    public Dictionary<string, double> ToDictionary()
    {
        return new Dictionary<string, double>
        {
            { EquitySector.BasicMaterials, BasicMaterialsLongRescaled },
            { EquitySector.Communications, CommunicationServicesLongRescaled },
            { EquitySector.ConsumerCyclical, ConsumerCyclicalLongRescaled },
            { EquitySector.ConsumerDefensive, ConsumerDefensiveLongRescaled },
            { EquitySector.Energy, EnergyLongRescaled },
            { EquitySector.Financial, FinancialServicesLongRescaled },
            { EquitySector.Healthcare, HealthcareLongRescaled },
            { EquitySector.Industrials, IndustrialsLongRescaled },
            { EquitySector.RealEstate, RealEstateLongRescaled },
            { EquitySector.Technology, TechnologyLongRescaled },
            { EquitySector.Utilities, UtilitiesLongRescaled }
        };
    }
}
