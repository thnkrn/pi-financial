
using Pi.SetMarketData.Application.Utils;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketProfileOverview
{
    public class MarketTickerOrderBookMapper
    {
        public TickerOrderBook MapOrderBook(OrderBook message, int decimals)
        {
            foreach (var bid in message.Bid)
            {
                bid.Price = DataManipulation.FormatDecimals(bid.Price, decimals);
            }

            foreach (var offer in message.Offer)
            {
                offer.Price = DataManipulation.FormatDecimals(offer.Price, decimals);
            }

            var tickerOrderBook = new TickerOrderBook
            {
                Bid = ConvertToArray(message.Bid),
                Offer = ConvertToArray(message.Offer),
            };

            return tickerOrderBook;
        }
        private static List<List<string>> ConvertToArray(List<BidAsk> bidAsks)
        {
            List<List<string>> lstBidAsk = new List<List<string>>();
            if (bidAsks == null)
            {
                return lstBidAsk;
            }
            for(int i = 0; i < bidAsks.Count; i++)
            {
                lstBidAsk.Add(new List<string>() {bidAsks[i].Price, bidAsks[i].Quantity});
            }
            return lstBidAsk;
        }
    }
}