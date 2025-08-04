using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class FeeAndExpense
{
    private readonly string _thailandFundCode;
    [XmlElement("FSCBI-ThailandFundCode")]
    public string ThailandFundCode { get => _thailandFundCode; init => _thailandFundCode = value.ToUpper(); }

    [XmlElement("PF-ActualManagementFee")]
    public double? ActualManagementFee { get; init; }

    [XmlElement("AT-ActualFrontLoad")]
    public double? ActualFrontLoad { get; init; }

    [XmlElement("AT-ActualDeferLoad")]
    public double? ActualDeferLoad { get; init; }

    [XmlElement("PF-NetExpenseRatio")]
    public double? NetExpenseRatio { get; init; }

    [XmlElement("PF-ProspectusDate")]
    public DateTime? ProspectusDate { get; init; }
}
