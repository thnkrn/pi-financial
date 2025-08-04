using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Pi.GlobalMarketData.Domain.Entities;

public class MarketSchedule
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("symbol")] public string? Symbol { get; set; }

    [BsonElement("exchange")] public string? Exchange { get; set; }

    [BsonElement("marketsession")] public string? MarketSession { get; set; }

    [BsonElement("utcstarttime")] public DateTime? UTCStartTime { get; set; }

    [BsonElement("utcendtime")] public DateTime? UTCEndTime { get; set; }
}