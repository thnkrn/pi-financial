using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;

namespace Pi.GlobalMarketDataRealTime.Application.Services.FixMapper;

public static class OrderBookMapperService
{
    public static OrderBook Map(Entry entry)
    {
        var bidPrice = entry.MdEntryPx.ToString();
        var bidQuantity = entry.MdEntrySize.ToString();
        string? offerPrice = null;
        string? offerQuantity = null;

        return new OrderBook
        {
            BidPrice = bidPrice,
            BidQuantity = bidQuantity,
            OfferPrice = offerPrice,
            OfferQuantity = offerQuantity
        };
    }
}