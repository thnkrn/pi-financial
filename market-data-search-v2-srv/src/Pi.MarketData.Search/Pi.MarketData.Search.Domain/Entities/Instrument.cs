using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.MarketData.Search.Domain.Entities;

[BsonIgnoreExtraElements]
public class Instrument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("symbol")]
    public string? Symbol { get; set; }

    [BsonElement("venue")]
    public string? Venue { get; set; }

    [BsonElement("order_book_id")]
    public int OrderBookId { get; set; }
}