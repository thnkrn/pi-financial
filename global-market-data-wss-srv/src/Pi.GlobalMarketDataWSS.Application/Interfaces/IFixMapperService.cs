using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Domain.Models.Redis;

namespace Pi.GlobalMarketDataWSS.Application.Interfaces;

public interface IFixMapperService
{
    Task MapToStreamingBody(
        FixMessage data,
        CallbackOrderBookMessageAsync callbackOrderBookMessageAsync,
        CallbackPriceInfoMessageAsync callbackPriceInfoMessageAsync,
        CallbackPublicTradeMessageAsync callbackPublicTradeMessageAsync
    );

    RedisValueResult MapToCache(PriceInfo message, string? currentCacheValue);
}

public delegate Task CallbackOrderBookMessageAsync(OrderBook? orderBook, string symbol);

public delegate Task CallbackPriceInfoMessageAsync(PriceInfo? priceInfo, string symbol);

public delegate Task CallbackPublicTradeMessageAsync(PublicTrade? publicTrade, string symbol);