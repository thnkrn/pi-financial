using System.Globalization;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;

namespace Pi.GlobalMarketDataWSS.Application.Services.FixMapper;

public static class OrderBookMapperService
{
    public static OrderBook? Map(Entry? entry)
    {
        // Guard clause
        if (entry == null)
            return null;

        var orderBook = new OrderBook();

        switch (entry.MdEntryType)
        {
            case FixMessageType.Bid:
                orderBook.BidPrice = entry.MdEntryPx.ToString(CultureInfo.InvariantCulture);
                orderBook.BidQuantity = entry.MdEntrySize.ToString();
                break;
            case FixMessageType.Offer:
                orderBook.OfferPrice = entry.MdEntryPx.ToString(CultureInfo.InvariantCulture);
                orderBook.OfferQuantity = entry.MdEntrySize.ToString();
                break;
        }

        return orderBook;
    }

    public static List<string>? ConvertBidToList(OrderBook? orderBook)
    {
        // Guard clause
        if (orderBook == null)
            return null;

        return string.IsNullOrEmpty(orderBook.BidPrice)
            ? null
            : [orderBook.BidPrice, orderBook.BidQuantity ?? "0"];
    }

    public static List<string>? ConvertOfferToList(OrderBook? orderBook)
    {
        // Guard
        if (orderBook == null)
            return null;

        return string.IsNullOrEmpty(orderBook.OfferPrice)
            ? null
            : [orderBook.OfferPrice, orderBook.OfferQuantity ?? "0"];
    }
}