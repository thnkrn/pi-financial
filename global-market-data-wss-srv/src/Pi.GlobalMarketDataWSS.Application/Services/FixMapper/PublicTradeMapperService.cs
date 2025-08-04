using System.Globalization;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Application.Helpers;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;

namespace Pi.GlobalMarketDataWSS.Application.Services.FixMapper;

public static class PublicTradeMapperService
{
    private static readonly List<object> EmptyList = [];

    public static PublicTrade? Map(Entry? entry)
    {
        // Guard clause, return null if entry is null or entry type is not '2'
        if (entry is not { MdEntryType: FixMessageType.Trade })
            return null;

        var price = entry.MdEntryPx.ToString("0.00");
        var quantity = entry.MdEntrySize.ToString();
        var tradeTime = entry.MdEntryTime.ToString("yyyy-MM-dd HH:mm:ss.FFF", CultureInfo.InvariantCulture);

        return new PublicTrade
        {
            InstrumentId = 0,
            Price = price,
            Quantity = quantity,
            TradeTime = tradeTime
        };
    }

    public static List<object> ConvertToList(PublicTrade? publicTrade, string? preClose)
    {
        if (publicTrade == null)
            return EmptyList;

        try
        {
            var nanoTimeStamp = publicTrade.TradeTime?.ToNanosTimestamp() ?? 0;
            var timestamp = publicTrade.TradeTime?.ConvertToThailandTime() ?? string.Empty;

            return
            [
                nanoTimeStamp, // nanoTimeStamp
                timestamp, // timestamp
                string.Empty, // aggressor
                publicTrade.Quantity ?? "0", // volume
                publicTrade.Price ?? "0.00", // price
                CalculatePriceChange(publicTrade.Price ?? "0.00", preClose ?? "0.00") // priceChanged
            ];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERR]: {ex.Message}");
            return EmptyList;
        }
    }

    private static string CalculatePriceChange(string strPrice, string strPreClose)
    {
        var price = double.Parse(strPrice);
        var preClose = double.Parse(strPreClose);

        return (price - preClose).ToString("0.00");
    }
}