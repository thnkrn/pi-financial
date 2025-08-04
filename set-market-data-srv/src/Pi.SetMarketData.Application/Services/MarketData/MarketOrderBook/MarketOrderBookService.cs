using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketOrderBook;

public static class MarketOrderBookService
{
    public static MarketOrderBookResponse GetResult
    (
        List<Instrument> instruments, 
        List<string> venues,
        List<MarketStreamingResponse> marketStreamingResponses
    )
    {
        var bidOfferList = new List<BidOfferList>();

        for (var i = 0; i < instruments.Count; i++)
        {
            var symbol = instruments[i].Symbol;
            if (string.IsNullOrEmpty(symbol)) continue;
            var marketStreaming = marketStreamingResponses.ElementAtOrDefault(i)?.Response?.Data?.FirstOrDefault() ??
                                  new StreamingBody();
            var bid = GetBidAsk(marketStreaming.OrderBook?.Bid ?? []);
            var offer = GetBidAsk(marketStreaming.OrderBook?.Offer ?? []);

            bidOfferList.Add(new BidOfferList
            {
                Symbol = symbol,
                Venue = venues.ElementAtOrDefault(i) ?? string.Empty,
                Bid = bid.FirstOrDefault()?.Price,
                Offer = offer.FirstOrDefault()?.Price,
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