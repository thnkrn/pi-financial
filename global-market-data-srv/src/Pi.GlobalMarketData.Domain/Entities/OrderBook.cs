using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Pi.GlobalMarketData.Domain.Entities;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
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

    [BsonElement("offer_price")]
    public string? OfferPrice { get; set; }

    [BsonElement("offer_quantity")]
    public string? OfferQuantity { get; set; }

    [BsonElement("instrument")]
    public Instrument? Instrument { get; set; }
}

public class BidAsk
{
    [BsonElement("price")]
    public string Price { get; set; }

    [BsonElement("quantity")]
    public string Quantity { get; set; }
}