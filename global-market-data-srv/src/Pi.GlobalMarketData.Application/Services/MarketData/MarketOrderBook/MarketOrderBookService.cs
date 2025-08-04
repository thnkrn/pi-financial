using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketOrderBook;

public static class MarketOrderBookService
{
    public static MarketOrderBookResponse GetResult
    (
        List<StreamingBody> marketStreamingResponses
    )
    {
        var bidOfferList = new List<BidOfferList>();

        for (var i = 0; i < marketStreamingResponses.Count; i++)
        {
            var marketStreaming = marketStreamingResponses.ElementAtOrDefault(i) ?? new StreamingBody();
            var bid = GetBidAsk(marketStreaming.OrderBook?.Bid ?? []);
            var offer = GetBidAsk(marketStreaming.OrderBook?.Offer ?? []);

            bidOfferList.Add(new BidOfferList
            {
                Symbol = marketStreaming.Symbol,
                Venue = marketStreaming.Venue,
                Bid = bid.FirstOrDefault()?.Price,
                Offer = offer.FirstOrDefault()?.Price
            });
        }

        return new MarketOrderBookResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new OrderBookResponse
            {
                BidOfferList = bidOfferList
            }
        };
    }

    private static List<BidAsk> GetBidAsk(List<List<string>> bidOffer)
    {
        return bidOffer
            .Select(bidAsk =>
                new BidAsk
                {
                    Price = bidAsk.ElementAtOrDefault(0) ?? "0.00",
                    Quantity = bidAsk.ElementAtOrDefault(1) ?? "0"
                }).ToList();
    }
}