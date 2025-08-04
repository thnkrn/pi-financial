using Newtonsoft.Json;
using Pi.SetMarketData.DataProcessingService.Exceptions;
using Pi.SetMarketData.DataProcessingService.Interface;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.SetMarketData.DataProcessingService.Helpers;

public class CacheServiceHelper : ICacheServiceHelper
{
    private readonly IRedisV2Publisher _cacheService;
    private readonly IMongoService<Instrument> _instrumentService;
    private readonly ILogger<CacheServiceHelper> _logger;
    private readonly IMongoService<OrderBook> _orderBookService;

    /// <summary>
    /// </summary>
    /// <param name="cacheService"></param>
    /// <param name="orderBookService"></param>
    /// <param name="instrumentService"></param>
    /// <param name="logger"></param>
    public CacheServiceHelper(
        IRedisV2Publisher cacheService,
        IMongoService<OrderBook> orderBookService,
        IMongoService<Instrument> instrumentService,
        ILogger<CacheServiceHelper> logger
    )
    {
        _cacheService = cacheService;
        _orderBookService = orderBookService;
        _instrumentService = instrumentService;
        _logger = logger;
    }

    public async Task<PriceInfo?> GetPriceInfoByOrderBookId(int orderBookId, string messageType)
    {
        var orderBookCache = await _cacheService.GetAsync<string>(
            $"{CacheKey.PriceInfo}{messageType}-{orderBookId}", true
        );

        return JsonConvert.DeserializeObject<PriceInfo>(orderBookCache ?? string.Empty);
    }

    public async Task<string?> GetSymbolByOrderBookId(int orderBookId)
    {
        var symbolCachedKey = $"symbol-order-book-{orderBookId}";
        var streamingBodyCachedKey = $"{CacheKey.StreamingBody}{orderBookId}";
        var streamingBody = await TryGetAsync(streamingBodyCachedKey);

        if (!string.IsNullOrEmpty(streamingBody))
        {
            var data = JsonConvert.DeserializeObject<MarketStreamingResponse>(streamingBody);
            if (data is { Response.Data.Count: > 0 })
            {
                var symbol = data.Response.Data[0].Symbol;

                // Set cache
                await _cacheService.SetAsync(symbolCachedKey, symbol ?? string.Empty, false, TimeSpan.FromDays(1));
                return symbol;
            }
        }
        else
        {
            // Get cache
            var symbolCached = await TryGetAsync(symbolCachedKey);
            if (!string.IsNullOrEmpty(symbolCached))
                return symbolCached;

            var orderBook = await _orderBookService.GetByFilterAsync(target => target.OrderBookId.Equals(orderBookId));
            var symbol = orderBook?.Symbol ?? string.Empty;

            // Set cache
            await _cacheService.SetAsync(symbolCachedKey, symbol, false, TimeSpan.FromDays(1));
            return symbol;
        }

        return string.Empty;
    }

    public async Task<string?> GetVenueBySymbol(string symbol)
    {
        var venue = await TryGetAsync($"{CacheKey.SymbolVenue}{symbol}");
        if (venue == null)
            try
            {
                var instrument = await _instrumentService.GetByFilterAsync(target =>
                    target.Symbol == symbol
                );
                venue = instrument?.Venue;

                await _cacheService.SetAsync(
                    $"{CacheKey.SymbolVenue}{symbol}",
                    venue ?? string.Empty,
                    false,
                    TimeSpan.FromDays(1)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: {Exception}", ex.Message);
                throw new DataProcessingServiceException($"Exception: {ex.Message}", ex);
            }

        return venue;
    }

    public async Task<string?> GetVenueByOrderBookId(int orderBookId, string symbol)
    {
        var venue = string.Empty;

        try
        {
            var streamingBody = await TryGetAsync(
                $"{CacheKey.StreamingBody}{orderBookId}"
            );
            if (string.IsNullOrEmpty(streamingBody))
            {
                var instrument = await _instrumentService.GetByFilterAsync(target =>
                    target.Symbol == symbol
                );
                if (instrument is { Venue: not null })
                    venue = instrument.Venue;
            }
            else
            {
                var data = JsonConvert.DeserializeObject<MarketStreamingResponse>(streamingBody);
                if (data is { Response.Data.Count: > 0 })
                    venue = data.Response.Data[0].Venue;
            }

            await _cacheService.SetAsync(
                $"{CacheKey.SymbolVenue}{symbol}",
                venue ?? string.Empty,
                false,
                TimeSpan.FromDays(1)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception: {OrderBookId}, {Symbol} - {Exception}",
                orderBookId,
                symbol,
                ex.Message
            );
            throw new DataProcessingServiceException($"Exception: {ex.Message}", ex);
        }

        return venue;
    }

    public async Task<string?> TryGetAsync(string cacheKey)
    {
        string result;

        try
        {
            result = await _cacheService.GetAsync<string>(cacheKey) ?? string.Empty;
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            try
            {
                result = await _cacheService.GetAsync<string>(cacheKey, true) ?? string.Empty;
            }
            catch
            {
                result = string.Empty;
            }
        }

        return result;
    }
}