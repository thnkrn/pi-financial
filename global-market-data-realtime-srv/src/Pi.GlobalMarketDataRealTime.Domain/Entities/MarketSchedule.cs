using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataRealTime.Domain.Entities;

public class MarketSchedule
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("symbol")] public string? Symbol { get; set; }
    [BsonElement("exchange")] public string? Exchange { get; set; }

    [BsonElement("marketsession")] public string? MarketSession { get; set; }

    [BsonElement("utcstarttime")] public DateTime? UTCStartTime { get; set; }

    [BsonElement("utcendtime")] public DateTime? UTCEndTime { get; set; }
}