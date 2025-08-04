using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Pi.GlobalMarketData.Domain.Entities;

public class MarketSessionStatus
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; }

    [BsonElement("exchange")]
    public string? Exchange { get; set; }

    [BsonElement("marketsession")]
    public string? MarketSession { get; set; }

    [BsonElement("utcstarttime")]
    public DateTime? UTCStartTime { get; set; }

    [BsonElement("utcendtime")]
    public DateTime? UTCEndTime { get; set; }
}
