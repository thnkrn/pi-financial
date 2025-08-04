using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Converters;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketData.DataProcessingService.Services;

public interface IMarketScheduleDataService
{
    Task<IEnumerable<MarketSchedule>> GetMarketSchedulesAsync(string symbol, string exchange);
    Task<IEnumerable<MarketSchedule>> GetMarketScheduleDataAsync(string symbol, string exchange);
}

public class MarketScheduleDataService : IMarketScheduleDataService
{
    private readonly ILogger<MarketScheduleDataService> _logger;
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly IRedisV2Publisher _redisCache;
    private readonly JsonSerializerSettings _serializerSettings;

    /// <summary>
    /// </summary>
    /// <param name="marketScheduleService"></param>
    /// <param name="redisCache"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MarketScheduleDataService(IMongoService<MarketSchedule> marketScheduleService,
        IRedisV2Publisher redisCache,
        ILogger<MarketScheduleDataService> logger)
    {
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _serializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new ObjectIdConverter() },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<IEnumerable<MarketSchedule>> GetMarketSchedulesAsync(string symbol, string exchange)
    {
        var cacheKey = $"{CacheKey.GeMarketSchedule}{exchange}-{symbol}";
        try
        {
            var cachedSchedules = await _redisCache.GetAsync<string>(cacheKey, true);
            if (!string.IsNullOrEmpty(cachedSchedules))
            {
                var deserializedValue = JsonConvert.DeserializeObject<IEnumerable<MarketSchedule>>(
                    cachedSchedules, _serializerSettings);

                if (deserializedValue != null)
                {
                    var schedules = deserializedValue.ToList();
                    if (schedules.Count > 0)
                    {
                        _logger.LogDebug("Retrieved {Count} market schedules from cache", schedules.Count);
                        return schedules;
                    }
                }
            }

            // Cache miss - fetch from database
            var marketScheduleList = await GetMarketScheduleDataAsync(symbol, exchange);
            var scheduleList = marketScheduleList.ToList();
            if (scheduleList.Count > 0)
            {
                // Serialize with our custom settings that handle ObjectId properly
                var serializedValue = JsonConvert.SerializeObject(scheduleList, _serializerSettings);
                if (!string.IsNullOrEmpty(serializedValue))
                    await _redisCache.SetAsync(
                        cacheKey,
                        serializedValue,
                        true,
                        TimeSpan.FromHours(24)
                    );
            }
            else
            {
                _logger.LogWarning("No market schedules found in database");
            }

            return scheduleList;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing market schedules from cache");

            // Fallback to database on serialization error
            var marketScheduleList = await GetMarketScheduleDataAsync(symbol, exchange);
            return marketScheduleList.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving market schedules");

            // Return empty list on general error
            return [];
        }
    }

    public async Task<IEnumerable<MarketSchedule>> GetMarketScheduleDataAsync(string symbol, string exchange)
    {
        try
        {
            // Set a 3-day window: yesterday, today, and tomorrow
            var utcNow = DateTime.UtcNow;
            var rangeStart = utcNow.AddDays(-1).Date; // Yesterday at midnight
            var rangeEnd = utcNow.AddDays(1).Date.AddDays(1).AddTicks(-1); // End of tomorrow

            // Get market schedules that overlap with our time window
            var marketScheduleData = await _marketScheduleService.GetAllByFilterAsync(target =>
                    target.Exchange == exchange &&
                    target.Symbol == symbol &&
                    ((target.UTCStartTime >= rangeStart && target.UTCStartTime <= rangeEnd) || // Starts in range
                     (target.UTCEndTime >= rangeStart && target.UTCEndTime <= rangeEnd) || // Ends in range
                     (target.UTCStartTime <= rangeStart && target.UTCEndTime >= rangeStart)) // Spans start of range
            );

            var result = marketScheduleData.ToList();

            _logger.LogInformation("Found {Count} market schedules in date range for {Exchange}.{Symbol}",
                result.Count, exchange, symbol);

            // If no results in date range, get all data for the symbol/exchange
            if (result.Count == 0)
            {
                _logger.LogInformation(
                    "No schedules found in date range for {Exchange}.{Symbol}, retrieving all schedules",
                    exchange, symbol);

                result = (await _marketScheduleService.GetAllByFilterAsync(target =>
                    target.Exchange == exchange &&
                    target.Symbol == symbol
                )).ToList();

                _logger.LogInformation("Found {Count} total market schedules for {Exchange}.{Symbol}",
                    result.Count, exchange, symbol);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get market schedule data for {Symbol} at {Exchange}: {Message}",
                symbol, exchange, ex.Message);
            return [];
        }
    }
}