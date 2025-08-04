using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class MarketStatus
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("market_status_id")] public long MarketStatusId { get; set; }

    [BsonElement("status")] public string? Status { get; set; }

    [BsonElement("description")] public string? Description { get; set; }

    [BsonElement("timestamp")] public DateTime Timestamp { get; set; }
}