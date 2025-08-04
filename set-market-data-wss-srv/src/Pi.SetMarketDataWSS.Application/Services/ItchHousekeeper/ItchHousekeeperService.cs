using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataWSS.Application.Helpers;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Redis;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using PublicTrade = Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper.PublicTrade;

namespace Pi.SetMarketDataWSS.Application.Services.ItchHousekeeper;

public class ItchHousekeeperService : IItchHousekeeperService
{
    private const string PreOpen1E = "PRE-OPEN1_E"; // SET
    private const string PreOpenE = "PRE-OPEN_E"; // SET
    private const string PreMorningD = "PRE-MORNING_D"; // TFEX
    private const string PreNightD = "PRE-NIGHT_D"; // TFEX
    private const string PreDayE = "PRE-DAY_E"; // DR
    private const string PreNightE = "PRE-NIGHT_E"; // DR

    private const string ResetStatPriceValue = "0.00";
    private const string ResetStatVolumeValue = "0";

    private static readonly HashSet<string> States = new(StringComparer.OrdinalIgnoreCase)
        { PreOpen1E, PreOpenE, PreMorningD, PreNightD, PreDayE, PreNightE };

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly ILogger<ItchHousekeeperService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ItchHousekeeperService" /> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <exception cref="ArgumentNullException">Thrown if logger is null.</exception>
    public ItchHousekeeperService(ILogger<ItchHousekeeperService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Resets statistics based on the provided ITCH message.
    /// </summary>
    /// <param name="message">The ITCH message.</param>
    /// <param name="currentCacheValue">The current cache values.</param>
    /// <returns>A <see cref="RedisValueResult" /> if reset is performed, default otherwise.</returns>
    public RedisValueResult? ResetStat(ItchMessage message, IReadOnlyDictionary<string, string> currentCacheValue)
    {
        try
        {
            if (message.MsgType != ItchMessageType.O)
                return null;

            var messageWrapper = (OrderBookStateMessageWrapper)message;
            if (!States.Contains(messageWrapper.StateName?.Value ?? string.Empty))
                return null;

            var currentPriceInfo = ResetPriceInfo(currentCacheValue);
            var currentMarketStreamingResponse = ResetMarketStreamingResponse(currentCacheValue, messageWrapper);
            var currentPublicTrade = Array.Empty<PublicTrade>();

            return new RedisValueResult
            {
                RedisChannel = RedisChannel.PubSubCache,
                RedisValue =
                [
                    new RedisValue { Key = CacheKey.PriceInfo, Value = currentPriceInfo },
                    new RedisValue { Key = CacheKey.StreamingBody, Value = currentMarketStreamingResponse },
                    new RedisValue { Key = CacheKey.PublicTrade, Value = currentPublicTrade }
                ]
            };
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JsonException while resetting stats: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while resetting stats: {Message}", ex.Message);
        }

        return null;
    }

    private static PriceInfo? ResetPriceInfo(IReadOnlyDictionary<string, string> currentCacheValue)
    {
        try
        {
            var currentPriceInfo = DeserializeOrDefault<PriceInfo>(currentCacheValue, CacheKey.PriceInfo);
            if (currentPriceInfo == null) return null;

            currentPriceInfo.Open = ResetStatPriceValue;
            currentPriceInfo.Open0 = ResetStatPriceValue;
            currentPriceInfo.Open1 = ResetStatPriceValue;
            currentPriceInfo.Open2 = ResetStatPriceValue;
            currentPriceInfo.High24H = ResetStatPriceValue;
            currentPriceInfo.Low24H = ResetStatPriceValue;
            currentPriceInfo.TotalAmount = ResetStatPriceValue;
            currentPriceInfo.TotalAmountK = ResetStatPriceValue;
            currentPriceInfo.TotalVolume = ResetStatVolumeValue;
            currentPriceInfo.TotalVolumeK = ResetStatVolumeValue;
            currentPriceInfo.Average = ResetStatPriceValue;
            currentPriceInfo.AverageBuy = ResetStatPriceValue;
            currentPriceInfo.AverageSell = ResetStatPriceValue;
            currentPriceInfo.AuctionPrice = ResetStatPriceValue;
            currentPriceInfo.AuctionVolume = ResetStatVolumeValue;

            return currentPriceInfo;
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Exception while resetting stats: {ex.Message}", ex);
        }
    }

    private static MarketStreamingResponse? ResetMarketStreamingResponse(
        IReadOnlyDictionary<string, string> currentCacheValue, OrderBookStateMessageWrapper messageWrapper)
    {
        try
        {
            var currentMarketStreamingResponse =
                DeserializeOrDefault<MarketStreamingResponse>(currentCacheValue, CacheKey.StreamingBody);
            if (currentMarketStreamingResponse is not { Response.Data.Count: > 0 })
                return currentMarketStreamingResponse;

            var data = currentMarketStreamingResponse.Response.Data[0];
            data.Status = OrderBookStateMappingsHelper.MapStateName(messageWrapper.StateName?.Value, null);
            data.Open = ResetStatPriceValue;
            data.Open0 = ResetStatPriceValue;
            data.Open1 = ResetStatPriceValue;
            data.Open2 = ResetStatPriceValue;
            data.High24H = ResetStatPriceValue;
            data.Low24H = ResetStatPriceValue;
            data.TotalAmount = ResetStatPriceValue;
            data.TotalAmountK = ResetStatPriceValue;
            data.TotalVolume = ResetStatVolumeValue;
            data.TotalVolumeK = ResetStatVolumeValue;
            data.Average = ResetStatPriceValue;
            data.AverageBuy = ResetStatPriceValue;
            data.AverageSell = ResetStatPriceValue;
            data.AuctionPrice = ResetStatPriceValue;
            data.AuctionVolume = ResetStatVolumeValue;
            data.PublicTrades = [];

            return currentMarketStreamingResponse;
        }
        catch (JsonException ex)
        {
            throw new JsonException($"Exception while resetting stats: {ex.Message}", ex);
        }
    }

    private static T? DeserializeOrDefault<T>(IReadOnlyDictionary<string, string> cache, string key) where T : class
    {
        return cache.TryGetValue(key, out var value) ? JsonSerializer.Deserialize<T>(value, JsonOptions) : null;
    }
}
