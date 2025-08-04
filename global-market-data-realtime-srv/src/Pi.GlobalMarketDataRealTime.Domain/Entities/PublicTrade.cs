using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketDataRealTime.Domain.Entities
{
    public class PublicTrade
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("Symbol")]
        public string? Symbol { get; set; }

        [BsonElement("public_trade_id")]
        public int PublicTradeId { get; set; }

        [BsonElement("instrument_id")]
        public int InstrumentId { get; set; }

        [BsonElement("price")]
        public string? Price { get; set; }

        [BsonElement("quantity")]
        public string? Quantity { get; set; }

        [BsonElement("trade_time")]
        public string? TradeTime { get; set; }

        [BsonElement("instrument")]
        public virtual Instrument? Instrument { get; set; }
    }
}
