namespace Pi.SetMarketDataWSS.Domain.Entities;

public class OrderBook
{
    public int OrderBookId { get; set; }
    public int InstrumentId { get; set; }
    public string? Symbol { get; set; }
    public string? BidPrice { get; set; }
    public string? BidQuantity { get; set; }
    public string? OfferPrice { get; set; }
    public string? OfferQuantity { get; set; }
    public List<BidAsk>? Bid { get; set; }
    public List<BidAsk>? Offer { get; set; }
    public int RoundLotSize {get; set; }
    public virtual Instrument Instrument { get; set; }
}

public class BidAsk
{
    public string? Price { get; set; }
    public string? Quantity { get; set; }
}