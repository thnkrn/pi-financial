using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Search.Domain.Entities;

[BsonIgnoreExtraElements]
public class TradingSign
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("trading_sign_id")]
    public int? TradingSignId { get; set; }

    [BsonElement("instrument_id")]
    public int? InstrumentId { get; set; }

    [BsonElement("sign")]
    public string? Sign { get; set; }

    [BsonElement("order_book_id")]
    public int? OrderBookId { get; set; }
}