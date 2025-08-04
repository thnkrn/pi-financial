using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.FundMarketData.DomainModels;

public class Fee
{
    public decimal? SwitchInFee { get; init; }
    [BsonRepresentation(BsonType.String)]
    public FeeUnit? SwitchInFeeUnit { get; init; }
    public DateTime? SwitchInEffectiveDate { get; init; }
    public double? ManagementFee { get; init; }
    public double? FrontendFee { get; init; }
    public double? BackendFee { get; init; }
    public double? TotalExpense { get; init; }
    public DateTime? AsOfDate { get; init; }
}

public enum FeeUnit
{
    AumPercentage = 1,
    THB,
    TradingAmountPercentage
}
