using MongoDB.Bson.Serialization.Attributes;

namespace Pi.FundMarketData.DomainModels;

public class ExtFundCategory
{
    [BsonId]
    public int Id { get; init; }

    public string ExternalId { get; init; }
}
