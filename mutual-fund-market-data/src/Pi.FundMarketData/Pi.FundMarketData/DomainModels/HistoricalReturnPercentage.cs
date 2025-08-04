using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.DomainModels;

public class HistoricalReturnPercentage
{
    [BsonRepresentation(BsonType.String)]
    public Interval Interval { get; init; }
    public double? Value { get; init; }
    public decimal? RawValueChange { get; init; }
}
