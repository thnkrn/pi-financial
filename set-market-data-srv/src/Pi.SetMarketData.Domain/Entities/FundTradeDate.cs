using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class FundTradeDate
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("fund_trade_date_id")]
        public int FundTradeDateId { get; set; }

        [BsonElement("instrument_id")]
        public int InstrumentId { get; set; }

        [BsonElement("tradable_date")]
        public string? TradableDate { get; set; }

        [BsonElement("instrument")]
        public virtual Instrument? Instrument { get; set; }
    }
}