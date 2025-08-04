using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class Fundamental
{
    public string InvestmentPolicy { get; init; }
    public string Objective { get; init; }
    public DateTime? InceptionDate { get; init; }
    public decimal? FundSize { get; init; }
    public string Currency { get; init; }
    public int RiskLevel { get; init; }
    public bool IsForeignInvestment { get; init; }
    public bool HasCurrencyRisk { get; init; }
    public bool AllowSwitchOut { get; init; }
    [BsonRepresentation(BsonType.String)]
    public TradeCalendarType? BuyCalendarType { get; init; }
    [BsonRepresentation(BsonType.String)]
    public TradeCalendarType? SellCalendarType { get; init; }
    [BsonRepresentation(BsonType.String)]
    public TradeCalendarType? SwitchOutCalendarType { get; init; }
    public DateTime? RegistrationDate { get; init; }
    public bool IsDividend { get; init; }
    public string TaxType { get; init; }
    public string AssetClassFocus { get; init; }
    [BsonRepresentation(BsonType.String)]
    public FundType FundType { get; init; }
    [BsonRepresentation(BsonType.String)]
    public ProjectType ProjectType { get; init; }
    public bool IsFatcaAllow { get; init; }
    public bool IsDerivative { get; init; }
    public bool HasHealthInsuranceBenefit { get; init; }
    [BsonRepresentation(BsonType.String)]
    public IEnumerable<InvestorAlert> InvestorAlerts { get; init; } = Enumerable.Empty<InvestorAlert>();
    public DateTime AsOfDate { get; init; }
    public string ComplexFundUrl { get; init; }
    public string ComplexFundRiskAckUrl { get; init; }
    public string RedemptionType { get; init; }
    public TradeCalendarType? GetTradeCalendarType(TradeSide tradeSide) =>
        tradeSide switch
        {
            TradeSide.Buy => BuyCalendarType,
            TradeSide.Sell => SellCalendarType,
            TradeSide.Switch => SwitchOutCalendarType,
            _ => throw new ArgumentOutOfRangeException(nameof(tradeSide), $"Not expected TradeSide value: {tradeSide}.")
        };
}
