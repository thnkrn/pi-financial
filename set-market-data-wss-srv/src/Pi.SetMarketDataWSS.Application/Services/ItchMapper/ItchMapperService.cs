using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Helpers;
using Pi.SetMarketDataWSS.Application.Interfaces;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.OrderBookMapper;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Redis;
using PublicTrade = Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper.PublicTrade;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;

public class ItchMapperService : IItchMapperService
{
    private readonly IOrderBookMapper _itchOrderBookMapperService;
    private readonly IItchPublicTradeMapper _itchPublicTradeMapperService;
    private readonly IItchOpenInterestMapperService _openInterestMapperService;
    private readonly IItchPriceInfoMapperService _priceInfoMapperService;
    private readonly IItchTickSizeTableEntryMapperService _tickSizeTableEntryMapperService;

    /// <summary>
    /// </summary>
    /// <param name="itchPriceInfoMapperService"></param>
    /// <param name="itchPublicTradeMapperService"></param>
    /// <param name="itchOrderBookMapperService"></param>
    /// <param name="itchTickSizeTableEntryMapperService"></param>
    /// <param name="openInterestMapperService"></param>
    public ItchMapperService(
        IItchPriceInfoMapperService itchPriceInfoMapperService,
        IItchPublicTradeMapper itchPublicTradeMapperService,
        IOrderBookMapper itchOrderBookMapperService,
        IItchTickSizeTableEntryMapperService itchTickSizeTableEntryMapperService,
        IItchOpenInterestMapperService openInterestMapperService
    )
    {
        _priceInfoMapperService = itchPriceInfoMapperService;
        _itchPublicTradeMapperService = itchPublicTradeMapperService;
        _itchOrderBookMapperService = itchOrderBookMapperService;
        _tickSizeTableEntryMapperService = itchTickSizeTableEntryMapperService;
        _openInterestMapperService = openInterestMapperService;
    }

    public RedisValueResult? MapToDataCache(
        ItchMessage message,
        Dictionary<string, string> currentCacheValue
    )
    {
        return message.MsgType switch
        {
            ItchMessageType.i => MapTradeTickerMessage(message, currentCacheValue),
            ItchMessageType.I => MapTradeStatisticsMessage(message, currentCacheValue),
            ItchMessageType.R => MapOrderBookDirectoryMessage(message, currentCacheValue),
            ItchMessageType.O => MapOrderBookStateMessage(message, currentCacheValue),
            ItchMessageType.b => MapMarketByPriceLevelMessage(message, currentCacheValue),
            ItchMessageType.Q => MapReferencedPriceMessage(message, currentCacheValue),
            ItchMessageType.J => MapIndexPriceMessage(message, currentCacheValue),
            ItchMessageType.k => MapPriceLimitMessage(message, currentCacheValue),
            ItchMessageType.m => MapMarketDirectoryMessage(message, currentCacheValue),
            ItchMessageType.L => MapTickSizeTableEntryMessage(message, currentCacheValue),
            ItchMessageType.h => MapOpenInterestMessage(message, currentCacheValue),
            ItchMessageType.Z => MapEquilibriumPriceMessage(message, currentCacheValue),
            _ => null
        };
    }

    private RedisValueResult MapTradeTickerMessage(ItchMessage message, Dictionary<string, string> currentCacheValue)
    {
        var currentTradeTickerMessage =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-");
        var priceQUpperInfo = DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}Q-");
        var cachePublicTrade = DeserializeOrDefault<PublicTrade[]>(currentCacheValue, CacheKey.PublicTrade);
        var tradeTickerMessageConvert = (TradeTickerMessageWrapper)message;

        var publicTradeValue = _itchPublicTradeMapperService.Map(tradeTickerMessageConvert, cachePublicTrade);
        var tradeTickerPriceInfo = _priceInfoMapperService.Map(message, currentTradeTickerMessage, priceQUpperInfo);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
            [
                new RedisValue
                    { Key = $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-", Value = tradeTickerPriceInfo },
                new RedisValue { Key = CacheKey.PublicTrade, Value = publicTradeValue.PublicTrade }
            ]
        };
    }

    private RedisValueResult MapTradeStatisticsMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var currentTradeStatisticsMessage =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-");
        var currentOrderBookStateStatusMessage =
            DeserializeOrDefault<MarketStatus>(currentCacheValue, CacheKey.MarketStatus);

        var tradeStatisticsPriceInfo = _priceInfoMapperService.Map(
            message,
            currentTradeStatisticsMessage,
            null,
            currentOrderBookStateStatusMessage?.Status ?? string.Empty
        );

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
            [
                new RedisValue
                    { Key = $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-", Value = tradeStatisticsPriceInfo }
            ]
        };
    }

    private RedisValueResult MapOrderBookDirectoryMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var orderBookDirectoryMessageConvert = (OrderBookDirectoryMessageWrapper)message;

        var currentOrderBookDirectoryForPriceInfoMessage =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, CacheKey.PriceInfo);
        var currentOrderBookDirectoryForOrderBookMessage =
            DeserializeOrDefault<OrderBook>(currentCacheValue, CacheKey.OrderBook);
        var currentOrderBookDirectoryForInstrumentDetailMessage =
            DeserializeOrDefault<InstrumentDetail>(currentCacheValue, CacheKey.InstrumentDetail);
        var priceForUnderlying = DeserializeOrDefault<PriceInfo>(currentCacheValue,
            $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-{orderBookDirectoryMessageConvert.UnderlyingOrderBookID}");

        UpdatePriceInfo(ref currentOrderBookDirectoryForPriceInfoMessage, orderBookDirectoryMessageConvert);
        UpdateOrderBook(ref currentOrderBookDirectoryForOrderBookMessage, orderBookDirectoryMessageConvert);
        UpdateInstrumentDetail(ref currentOrderBookDirectoryForInstrumentDetailMessage,
            orderBookDirectoryMessageConvert, priceForUnderlying?.Price ?? "0.00");

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
            [
                new RedisValue
                {
                    Key = CacheKey.PriceInfo,
                    Value = currentOrderBookDirectoryForPriceInfoMessage
                },
                new RedisValue { Key = CacheKey.OrderBook, Value = currentOrderBookDirectoryForOrderBookMessage },
                new RedisValue
                    { Key = CacheKey.InstrumentDetail, Value = currentOrderBookDirectoryForInstrumentDetailMessage }
            ]
        };
    }

    private RedisValueResult MapOrderBookStateMessage(ItchMessage message, Dictionary<string, string> currentCacheValue)
    {
        var currentOrderBookStateMessage = DeserializeOrDefault<MarketStatus>(currentCacheValue, CacheKey.MarketStatus);
        var currentOrderBookDirectoryForOrderBookStateMessage =
            DeserializeOrDefault<InstrumentDetail>(currentCacheValue, CacheKey.InstrumentDetail);

        var orderBookStateMessage = (OrderBookStateMessageWrapper)message;
        var marketSegment = currentOrderBookDirectoryForOrderBookStateMessage?.MarketSegment;
        var stateName =
            OrderBookStateMappingsHelper.MapStateName(orderBookStateMessage.StateName?.Value, marketSegment);

        UpdateMarketStatus(ref currentOrderBookStateMessage, orderBookStateMessage, stateName);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue = [new RedisValue { Key = CacheKey.MarketStatus, Value = currentOrderBookStateMessage }]
        };
    }

    private RedisValueResult MapMarketByPriceLevelMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var cacheOrderBook = DeserializeOrDefault<OrderBook>(currentCacheValue, CacheKey.OrderBook);
        var orderBookMessage = (MarketByPriceLevelWrapper)message;
        var orderBook = _itchOrderBookMapperService.Map(orderBookMessage, cacheOrderBook);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue = [new RedisValue { Key = CacheKey.OrderBook, Value = orderBook }]
        };
    }

    private RedisValueResult MapReferencedPriceMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var cachePriceInfo =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-");
        var referencedPricePriceInfo = _priceInfoMapperService.Map(message, cachePriceInfo, null);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
            [
                new RedisValue
                    { Key = $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-", Value = referencedPricePriceInfo }
            ]
        };
    }

    private RedisValueResult MapIndexPriceMessage(ItchMessage message, Dictionary<string, string> currentCacheValue)
    {
        var cacheIndexPriceInfo =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-");
        var indexPricePriceInfo = _priceInfoMapperService.Map(message, cacheIndexPriceInfo, null);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
            [
                new RedisValue
                    { Key = $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-", Value = indexPricePriceInfo }
            ]
        };
    }

    private RedisValueResult MapPriceLimitMessage(ItchMessage message, Dictionary<string, string> currentCacheValue)
    {
        var cachePriceLimitInfo =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-");
        var priceLimitInfo = _priceInfoMapperService.Map(message, cachePriceLimitInfo, null);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
                [new RedisValue { Key = $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-", Value = priceLimitInfo }]
        };
    }

    private RedisValueResult MapMarketDirectoryMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var currentMarketDirectoryStateMessage =
            DeserializeOrDefault<MarketDirectory>(currentCacheValue, CacheKey.MarketDirectory);
        var marketDirectoryStateMessage = (MarketDirectoryMessageWrapper)message;

        UpdateMarketDirectory(ref currentMarketDirectoryStateMessage, marketDirectoryStateMessage);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue = [new RedisValue { Key = CacheKey.MarketDirectory, Value = currentMarketDirectoryStateMessage }]
        };
    }

    private RedisValueResult MapTickSizeTableEntryMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var cacheTickSizeTableEntry =
            DeserializeOrDefault<TickSizeTableEntry>(currentCacheValue, CacheKey.TickSizeTableEntry);
        var cacheInstrumentDetail =
            DeserializeOrDefault<InstrumentDetail>(currentCacheValue, CacheKey.InstrumentDetail);
        var tickSizeTableEntry =
            _tickSizeTableEntryMapperService.Map(message, cacheTickSizeTableEntry, cacheInstrumentDetail);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue = [new RedisValue { Key = CacheKey.TickSizeTableEntry, Value = tickSizeTableEntry }]
        };
    }

    private RedisValueResult MapOpenInterestMessage(ItchMessage message, Dictionary<string, string> currentCacheValue)
    {
        var cachedOpenInterest = DeserializeOrDefault<OpenInterest>(currentCacheValue, CacheKey.OpenInterest) ??
                                 new OpenInterest();

        _openInterestMapperService.Map(message, ref cachedOpenInterest);
        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
            [
                new RedisValue
                {
                    Key = CacheKey.OpenInterest, Value = cachedOpenInterest
                }
            ]
        };
    }

    private RedisValueResult MapEquilibriumPriceMessage(ItchMessage message,
        Dictionary<string, string> currentCacheValue)
    {
        var cachePriceInfo =
            DeserializeOrDefault<PriceInfo>(currentCacheValue, $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-");
        var marketStatus = DeserializeOrDefault<MarketStatus>(currentCacheValue, CacheKey.MarketStatus);

        var equilibriumPriceMessage = (EquilibriumPriceMessageWrapper)message;
        var priceInfo =
            _priceInfoMapperService.Map(equilibriumPriceMessage, cachePriceInfo, null, marketStatus?.Status ?? string.Empty);

        return new RedisValueResult
        {
            RedisChannel = RedisChannel.PubSubCache,
            RedisValue =
                [new RedisValue { Key = $"{CacheKey.PriceInfo}{message.MsgType.ToString()}-", Value = priceInfo }]
        };
    }

    private static T? DeserializeOrDefault<T>(Dictionary<string, string> cache, string key) where T : class
    {
        if (cache.TryGetValue(key, out var value))
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonException)
            {
                return null;
            }

        return null;
    }

    private static void UpdatePriceInfo(ref PriceInfo? priceInfo, OrderBookDirectoryMessageWrapper message)
    {
        priceInfo ??= new PriceInfo();
        priceInfo.Currency = message.TradingCurrency?.Value;
        priceInfo.Symbol = message.Symbol?.Value;
        priceInfo.LastTrade = message.LastTradingDate?.Value;
        priceInfo.SecurityType = message.FinancialProduct?.Value;
        priceInfo.ExercisePrice = message.StrikePrice?.Value.ToString();
    }

    private static void UpdateOrderBook(ref OrderBook? orderBook, OrderBookDirectoryMessageWrapper message)
    {
        if (orderBook == null && message.OrderBookID != null) 
            orderBook = new OrderBook();

        if (orderBook != null && message.OrderBookID != null)
        {
            orderBook.OrderBookId = message.OrderBookID.Value;
            orderBook.Symbol = message.Symbol?.Value;
            orderBook.RoundLotSize = message.RoundLotSize?.Value ?? 0;
        }
    }

    private static void UpdateInstrumentDetail(ref InstrumentDetail? instrumentDetail,
        OrderBookDirectoryMessageWrapper message, string underlyingPrice)
    {
        if (instrumentDetail == null && message.OrderBookID != null)
            instrumentDetail = new InstrumentDetail();

        if (instrumentDetail != null && message.OrderBookID != null)
        {
            instrumentDetail.Decimals = message.DecimalsInPrice?.Value ?? 0;
            instrumentDetail.DecimalInStrikePrice = message.DecimalsInStrikePrice?.Value ?? 0;
            instrumentDetail.DecimalInContractSizePQF = message.DecimalsInContractSizePQF?.Value ?? 0;
            instrumentDetail.StrikePrice = message.StrikePrice?.Value.ToString();
            instrumentDetail.ContractSize = message.ContractSize?.Value.ToString();
            instrumentDetail.MarketSegment = message.MarketSegment?.Value;
            instrumentDetail.MarketCode = message.MarketCode?.Value ?? 0;
            instrumentDetail.InstrumentId = message.OrderBookID.Value;
            instrumentDetail.UnderlyingOrderBookID = message.UnderlyingOrderBookID?.Value.ToString();
            instrumentDetail.UnderlyingPrice = underlyingPrice;
        }
    }

    private static void UpdateMarketStatus(ref MarketStatus? marketStatus, OrderBookStateMessageWrapper message,
        string stateName)
    {
        if (marketStatus == null && message.OrderBookId != null)
            marketStatus = new MarketStatus();

        if (marketStatus != null && message.OrderBookId != null)
        {
            marketStatus.MarketStatusId = message.OrderBookId.Value;
            marketStatus.Status = stateName;
            marketStatus.Timestamp = DateTime.Now;
        }
    }

    private static void UpdateMarketDirectory(ref MarketDirectory? marketDirectory,
        MarketDirectoryMessageWrapper message)
    {
        marketDirectory ??= new MarketDirectory();
        marketDirectory.MarketCode = message.MarketCode?.Value ?? 0;
        marketDirectory.MarketName = message.MarketName?.Value ?? string.Empty;

        // If marketCode = 201, the response is string.Empty
        marketDirectory.MarketDescription = marketDirectory.MarketCode.Equals(201)
            ? string.Empty
            : message.MarketDescription?.Value ?? string.Empty;
    }
}