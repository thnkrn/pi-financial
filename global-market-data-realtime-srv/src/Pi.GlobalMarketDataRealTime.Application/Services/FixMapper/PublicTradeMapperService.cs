using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;

namespace Pi.GlobalMarketDataRealTime.Application.Services.FixMapper;

public static class PublicTradeMapperService
{
    public static PublicTrade Map(Entry entry)
    {
        var price = entry.MdEntryPx.ToString();
        var quantity = entry.MdEntrySize.ToString();
        string? tradeTime = null;

        return new PublicTrade
        {
            InstrumentId = 0,
            Price = price,
            Quantity = quantity,
            TradeTime = tradeTime
        };
    }
}