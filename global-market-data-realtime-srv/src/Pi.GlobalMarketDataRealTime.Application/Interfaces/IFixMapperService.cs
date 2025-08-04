using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using Pi.GlobalMarketDataRealTime.Domain.Models.Redis;

namespace Pi.GlobalMarketDataRealTime.Application.Interfaces;

public interface IFixMapperService
{
    Task MapToDatabase(
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