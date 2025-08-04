using MongoDB.Bson.Serialization.Attributes;

namespace Pi.FundMarketData.DomainModels;

public class FundCategory
{
    [BsonId]
    public string Name { get; init; }

    public List<int> ExtFundCategoryIds { get; init; }

    public string AssetClassType { get; init; }
}
