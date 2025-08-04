using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities;

public class InitialMargin
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [BsonElement("product_type")]
    public string ProductType { get; set; } = string.Empty;

    [BsonElement("im")]
    public string Im { get; set; } = string.Empty;

    [BsonElement("as_of_date")]
    public DateTime? AsOfDate { get; set; }

    [BsonElement("created_at")]
    public DateTime? CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}
