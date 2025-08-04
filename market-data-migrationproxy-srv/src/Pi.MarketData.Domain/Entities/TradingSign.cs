using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Domain.Entities;

public class TradingSign
{
    [BsonId] [BsonElement("_id")] public ObjectId Id { get; set; }

    [BsonElement("trading_sign_id")] public int TradingSignId { get; set; }

    [BsonElement("instrument_id")] public int InstrumentId { get; set; }

    [BsonElement("sign")] public string? Sign { get; set; }

    [BsonElement("instrument")] public Instrument? Instrument { get; set; }
}