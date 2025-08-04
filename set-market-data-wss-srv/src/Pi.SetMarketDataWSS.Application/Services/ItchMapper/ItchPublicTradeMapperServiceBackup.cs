using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchPublicTradeMapperServiceBackup
{
    public PublicTradeResult Map(TradeTickerMessageWrapper message, PublicTrade[]? cachePublicTrade)
    {
        var result = new PublicTradeResult(Array.Empty<PublicTrade>());

        // if there is exist cache for public trade then append the new public trade to the cache
        if (cachePublicTrade != null)
        {
            result.PublicTrade = cachePublicTrade;
            _ = result.PublicTrade.Append(new PublicTrade(message.DealDateTime.ToUnixTimestampSeconds(), message.DealDateTime, message.Aggressor,
                message.Quantity, message.Price)).Reverse();
        }
        else // if there is no cache for public trade then return new value
        {
            result.PublicTrade =
            [
                new PublicTrade(message.Nanos, message.DealDateTime, message.Aggressor, message.Quantity, message.Price)
            ];
        }

        return result;
    }
}