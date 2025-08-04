using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class MarketCode
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("market")]
        public string? Market { get; set; }

        [BsonElement("order_book_id")]
        public int OrderBookId { get; set; }
    }
}