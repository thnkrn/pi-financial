using Pi.SetMarketDataWSS.Domain.Entities;

public static class MockData
{
    public static OrderBook GetMockOrderBook()
    {
        return new OrderBook
        {
            Symbol = "AAPL",
            OrderBookId = 12345,
            InstrumentId = 12345,
            BidPrice = "135.67",
            BidQuantity = "1000",
            Bid = new List<BidAsk>
            {
                new() { Price = "135.67", Quantity = "500" },
                new() { Price = "135.50", Quantity = "300" },
                new() { Price = "135.30", Quantity = "200" }
            },
            OfferPrice = "135.80",
            OfferQuantity = "1500",
            Offer = new List<BidAsk>
            {
                new() { Price = "135.80", Quantity = "700" },
                new() { Price = "135.90", Quantity = "500" },
                new() { Price = "136.00", Quantity = "300" }
            }
        };
    }
}