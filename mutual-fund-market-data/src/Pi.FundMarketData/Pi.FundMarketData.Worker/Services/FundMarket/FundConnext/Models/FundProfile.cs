using Pi.FundMarketData.Constants;
using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;
public class FundProfile
{
    public string Isin { get; init; }

    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }

    private readonly string _previousFundCode;
    public string PreviousFundCode { get => _previousFundCode; init => _previousFundCode = value.ToUpper(); }

    public string Name { get; init; }
    public string AmcCode { get; init; }

    private readonly string _fundClassCode;
    public string FundClassCode { get => _fundClassCode; init => _fundClassCode = value.ToUpper(); }

    public int FundRiskLevel { get; init; }
    public bool? FifFlag { get; init; }
    public bool FxRiskFlag { get; init; }
    public string FundPolicy { get; init; }
    public bool SwitchOutFlag { get; init; }
    public string BuyPeriodFlag { get; init; }
    public string SellPeriodFlag { get; init; }
    public string SwitchOutPeriodFlag { get; init; }
    public DateTime? RegistrationDate { get; init; }
    public bool? DividendFlag { get; init; }
    public string TaxType { get; init; }
    public decimal? FstLowBuyVal { get; init; }
    public decimal? NxtLowBuyVal { get; init; }
    public decimal? LowSellUnit { get; init; }
    public decimal? LowSellVal { get; init; }
    public decimal? LowBalUnit { get; init; }
    public decimal? LowBalVal { get; init; }
    public int? SellSettlementDay { get; init; }
    public int? BuyCutOffTime { get; init; }
    public int? SellCutOffTime { get; init; }
    public string Currency { get; init; }
    public string FundType { get; init; }
    public string ProjectRetailType { get; init; }
    public bool FatcaAllowFlag { get; init; }
    public bool? DerivativeFlag { get; init; }
    public bool HealthInsuranceBenefit { get; init; }
    public string InvestorAlerts { get; init; }
    public DateTime AsOfDate { get; init; }
    public string ComplexFundUrl { get; init; }
    public string ComplexFundRiskAckUrl { get; init; }
    public string RedemptionType { get; init; }

    public static readonly Func<string[], string, FundProfile> Mapper = (dataValues, asOfDate) =>
    {
        return new FundProfile
        {
            Isin = dataValues[42],
            FundCode = dataValues[0],
            PreviousFundCode = dataValues[40],
            Name = dataValues[3],
            AmcCode = dataValues[1],
            FundRiskLevel = int.Parse(dataValues[9]),
            FifFlag = UtilsMethod.StringToBool(dataValues[6]),
            FxRiskFlag = (bool)UtilsMethod.StringToBool(dataValues[10])!,
            FundPolicy = AssetClassFocus.GetAssetClassFocus(dataValues[4]),
            SwitchOutFlag = (bool)UtilsMethod.StringToBool(dataValues[22])!,
            BuyPeriodFlag = dataValues[25],
            SellPeriodFlag = dataValues[26],
            SwitchOutPeriodFlag = dataValues[28],
            FundClassCode = dataValues[24],
            RegistrationDate = UtilsMethod.StringToDateTime(dataValues[8]),
            DividendFlag = UtilsMethod.StringToBool(dataValues[7]),
            TaxType = dataValues[5],
            FstLowBuyVal = UtilsMethod.StringToDecimal(dataValues[13]),
            NxtLowBuyVal = UtilsMethod.StringToDecimal(dataValues[14]),
            LowSellUnit = UtilsMethod.StringToDecimal(dataValues[17]),
            LowSellVal = UtilsMethod.StringToDecimal(dataValues[16]),
            LowBalUnit = UtilsMethod.StringToDecimal(dataValues[19]),
            LowBalVal = UtilsMethod.StringToDecimal(dataValues[18]),
            SellSettlementDay = int.TryParse(dataValues[20], out var sSettlement) ? sSettlement : null,
            BuyCutOffTime = int.TryParse(dataValues[12], out var bCutOff) ? bCutOff : null,
            SellCutOffTime = int.TryParse(dataValues[15], out var sCutOff) ? sCutOff : null,
            Currency = dataValues[58],
            FundType = dataValues[35],
            ProjectRetailType = dataValues[44],
            FatcaAllowFlag = (bool)UtilsMethod.StringToBool(dataValues[11])!,
            DerivativeFlag = UtilsMethod.StringToBool(dataValues[36])!,
            HealthInsuranceBenefit = (bool)UtilsMethod.StringToBool(dataValues[39]),
            InvestorAlerts = dataValues[41],
            AsOfDate = (DateTime)UtilsMethod.StringToDateTime(asOfDate)!,
            ComplexFundUrl = dataValues[59],
            ComplexFundRiskAckUrl = dataValues[60],
            RedemptionType = dataValues[61]
        };
    };
}
