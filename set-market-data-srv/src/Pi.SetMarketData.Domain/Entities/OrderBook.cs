using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.SetMarketData.Domain.Entities
{
    public class OrderBook
    {
        [BsonId]
        [BsonElement("_id")]
        public ObjectId Id { get; set; }

        [BsonElement("symbol")]
        public string? Symbol { get; set; }

        [BsonElement("order_book_id")]
        public int OrderBookId { get; set; }

        [BsonElement("instrument_id")]
        public int InstrumentId { get; set; }

        [BsonElement("bid_price")]
        public string? BidPrice { get; set; }

        [BsonElement("bid_quantity")]
        public string? BidQuantity { get; set; }

        [BsonElement("bid")]
        public List<BidAsk> Bid { get; set; }

        [BsonElement("offer_price")]
        public string? OfferPrice { get; set; }

        [BsonElement("offer_quantity")]
        public string? OfferQuantity { get; set; }

        [BsonElement("offer")]
        public List<BidAsk> Offer { get; set; }

        [BsonElement("instrument")]
        public virtual Instrument? Instrument { get; set; }

        [BsonElement("round_lot_size")]
        public int RoundLotSize { get; set; }
    }
    public class BidAsk
    {
        [BsonElement("price")]
        public string Price { get; set; }

        [BsonElement("quantity")]
        public string Quantity { get; set; }
    }
}
