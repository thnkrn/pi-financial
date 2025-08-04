using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class TradingSign
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("trading_sign_id")]
        public int TradingSignId { get; set; }

        [BsonElement("instrument_id")]
        public int InstrumentId { get; set; }

        [BsonElement("sign")]
        public string? Sign { get; set; }

        [BsonElement("order_book_id")]
        public int? OrderBookId { get; set; }

        [BsonElement("instrument")]
        public virtual Instrument? Instrument { get; set; }
    }
}