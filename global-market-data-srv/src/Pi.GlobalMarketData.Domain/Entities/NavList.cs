using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

public class NavList
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("nav_list_id")] public int NavListId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("price_change")] public double PriceChange { get; set; }

    [BsonElement("price_change_ratio")] public double PriceChangeRatio { get; set; }

    [BsonElement("first_candle_time")] public int FirstCandleTime { get; set; }

    [BsonElement("nav_time")] public int NavTime { get; set; }

    [BsonElement("nav")] public double Nav { get; set; }

    [BsonElement("instrument")] public virtual Instrument? Instrument { get; set; }
}