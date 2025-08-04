using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities.SetSmart;

public class BrokerInfo
{
    [BsonId]
    [BsonElement("_id")]
    public ObjectId Id { get; set; }

    [BsonElement("broker_code")]
    public string? BrokerCode { get; set; }

    [BsonElement("broker_id")]
    public string? BrokerId { get; set; }

    [BsonElement("as_of_date")]
    public DateTime AsOfDate { get; set; }

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;
}