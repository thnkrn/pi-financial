using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Application.Constants;
using Pi.GlobalMarketDataRealTime.DataHandler.Converters;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;

public class MarketScheduleService : BackgroundService
{
    private readonly string _instanceConfigProfile;

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        Converters = new List<JsonConverter> { new ObjectIdConverter() }
    };

    private readonly ILogger<MarketScheduleService> _logger;
    private readonly int _lookBackDays;
    private readonly int _lookForwardDays;
    private readonly TimeSpan _marketDataCacheCheckerDuration;

    // Configuration values
    private readonly TimeSpan _marketDataCacheDuration;
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly TimeSpan _pollingInterval;
    private readonly IRedisV2Publisher _redisCache;
    private readonly IMongoService<WhiteList> _whitelistService;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="whitelistService"></param>
    /// <param name="marketScheduleService"></param>
    /// <param name="redisCache"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public MarketScheduleService(
        IConfiguration configuration,
        IMongoService<WhiteList> whitelistService,
        IMongoService<MarketSchedule> marketScheduleService,
        IRedisV2Publisher redisCache,
        ILogger<MarketScheduleService> logger)
    {
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Load configuration values
        _instanceConfigProfile = configuration[ConfigurationKeys.InstanceConfigProfile] ?? string.Empty;
        _marketDataCacheDuration = TimeSpan.FromHours(12);
        _marketDataCacheCheckerDuration = TimeSpan.FromMinutes(750);
        _pollingInterval = TimeSpan.FromMinutes(1);
        _lookBackDays = 2;
        _lookForwardDays = 2;

        if (string.IsNullOrEmpty(_instanceConfigProfile))
            throw new InvalidOperationException("The 'InstanceConfigProfile' cannot be null or empty.");
    }

    /// <summary>
    ///     Executes the background service's main loop, checking and caching market schedules.
    /// </summary>
    /// <param name="stoppingToken">Token to signal service cancellation</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Market Schedule Service started with instance profile: {InstanceConfigProfile}",
            _instanceConfigProfile);

        Task? executeAsyncTask = null;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for the previous task to complete if it exists and is not faulted/completed/canceled
                if (executeAsyncTask is { IsCompleted: false, IsFaulted: false, IsCanceled: false })
                    await executeAsyncTask;

                // Create a new task
                executeAsyncTask = Task.Factory.StartNew(
                    async () => { await ProcessWhitelistedSymbols(stoppingToken); },
                    stoppingToken,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default).Unwrap();
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "Unhandled exception in Market Schedule Service: {Message}", ex.Message);

                // Optionally, you might want to handle or reset the task
                executeAsyncTask = null;
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Market Schedule Service stopping");
    }

    private async Task ProcessWhitelistedSymbols(CancellationToken stoppingToken)
    {
        var scheduleCheckerKey = $"schedule-checker-cached-{_instanceConfigProfile}";
        _logger.LogInformation("Checking schedule checker key: {Key}", scheduleCheckerKey);

        try
        {
            var checkerData = await _redisCache.GetAsync<string>(scheduleCheckerKey, true) ?? string.Empty;

            if (string.IsNullOrEmpty(checkerData))
            {
                var whitelists = await GetWhitelistAsync();
                _logger.LogInformation("Retrieved {Count} whitelist entries for profile {Profile}",
                    whitelists.Count, _instanceConfigProfile);

                var processedCount = 0;
                var symbolsWithoutSchedules = new List<(string Exchange, string Symbol)>();

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var whitelist in whitelists)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    if (string.IsNullOrEmpty(whitelist.Symbol) || string.IsNullOrEmpty(whitelist.Exchange))
                    {
                        _logger.LogWarning("Skipping whitelist entry with missing Symbol or Exchange: {Id}",
                            whitelist.Id);
                        continue;
                    }

                    processedCount++;

                    var schedules = await GetMarketSchedulesAsync(whitelist.Symbol, whitelist.Exchange);

                    if (schedules.Length == 0)
                        symbolsWithoutSchedules.Add((whitelist.Exchange, whitelist.Symbol));
                }

                await SetScheduleCheckerCache(scheduleCheckerKey);

                _logger.LogInformation(
                    "Completed processing {Count} symbols. Found {MissingCount} symbols without schedules.",
                    processedCount, symbolsWithoutSchedules.Count);

                LogSymbolsWithoutSchedules(symbolsWithoutSchedules);
            }
            else
            {
                GetCheckerCacheValue(checkerData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessWhitelistedSymbols: {Message}", ex.Message);
        }
    }

    private async Task SetScheduleCheckerCache(string scheduleCheckerKey)
    {
        try
        {
            // Store the current timestamp in Redis (explicitly UTC)
            var utcNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            var result = await _redisCache.SetAsync(scheduleCheckerKey, utcNow.Ticks.ToString(), true,
                _marketDataCacheCheckerDuration);

            if (result)
                _logger.LogInformation("Successfully set schedule checker cache with expiration of {Hours} hours",
                    _marketDataCacheCheckerDuration.TotalHours);
            else
                _logger.LogWarning("Failure to set schedule checker cache!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set schedule checker cache: {Message}", ex.Message);
        }
    }

    private void GetCheckerCacheValue(string checkerData)
    {
        if (long.TryParse(checkerData, out var ticks))
        {
            var lastCheckTime = new DateTime(ticks, DateTimeKind.Utc);
            var timeUntilExpiry = lastCheckTime.Add(_marketDataCacheCheckerDuration) - DateTime.UtcNow;

            _logger.LogDebug(
                "Schedule checker cache hit. Last checked at {LastCheck}, expires in {TimeLeft} minutes",
                lastCheckTime, timeUntilExpiry.TotalMinutes);
        }
        else
        {
            _logger.LogWarning("Found invalid schedule checker cache data. Will be refreshed on next cycle.");
        }
    }

    private async Task<MarketSchedule[]> GetMarketSchedulesAsync(string symbol, string exchange)
    {
        var normalizedSymbol = symbol.Trim();
        var normalizedExchange = exchange.Trim();
        var cacheKey =
            $"{ConstantKeys.MarketScheduleCacheKey}_{normalizedExchange}_{normalizedSymbol}_{_instanceConfigProfile}";

        try
        {
            var cachedData = await _redisCache.GetAsync<string>(cacheKey, true) ?? string.Empty;

            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedSchedules = JsonConvert.DeserializeObject<MarketSchedule[]>(cachedData, _jsonSettings) ?? [];
                _logger.LogDebug("Cache hit for {Exchange}.{Symbol}: {Count} schedules",
                    normalizedExchange, normalizedSymbol, cachedSchedules.Length);
                return cachedSchedules;
            }

            _logger.LogDebug("Cache miss for {Exchange}.{Symbol}, querying database",
                normalizedExchange, normalizedSymbol);

            var marketSchedules = await GetMarketScheduleDataAsync(normalizedSymbol, normalizedExchange);
            var schedules = marketSchedules.ToArray();
            var marketSchedulesJson = JsonConvert.SerializeObject(schedules, _jsonSettings);
            _ = await _redisCache.SetAsync(cacheKey, marketSchedulesJson, true, _marketDataCacheDuration);

            return schedules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving market schedules for {Symbol} at {Exchange}: {Message}",
                normalizedSymbol, normalizedExchange, ex.Message);
            return [];
        }
    }

    private void LogSymbolsWithoutSchedules(List<(string Exchange, string Symbol)> symbolsWithoutSchedules)
    {
        if (symbolsWithoutSchedules.Count > 0)
            foreach (var (exchange, symbol) in symbolsWithoutSchedules)
                _logger.LogWarning("Missing schedule: {Exchange}.{Symbol}", exchange, symbol);
    }

    private async Task<List<WhiteList>> GetWhitelistAsync()
    {
        try
        {
            var list = await _whitelistService.GetListByFilterAsync(target =>
                !string.IsNullOrEmpty(target.InstanceConfigProfile)
                && target.IsWhitelist == true
                && !string.IsNullOrEmpty(target.Symbol)
                && target.InstanceConfigProfile.ToUpperInvariant() == _instanceConfigProfile.ToUpperInvariant());

            return list.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get whitelist for profile {InstanceConfigProfile}: {Message}",
                _instanceConfigProfile, ex.Message);
            return [];
        }
    }

    private async Task<IEnumerable<MarketSchedule>> GetMarketScheduleDataAsync(string symbol, string exchange)
    {
        try
        {
            // First try to get data within the date range
            var utcNow = DateTime.UtcNow;
            var rangeStart = utcNow.AddDays(-_lookBackDays).Date;
            var rangeEnd = utcNow.AddDays(_lookForwardDays).Date.AddTicks(-1);
            var marketScheduleData = await _marketScheduleService.GetListByFilterAsync(target =>
                target.Exchange == exchange &&
                target.Symbol == symbol &&
                ((target.UTCStartTime >= rangeStart && target.UTCStartTime <= rangeEnd)
                 || (target.UTCEndTime >= rangeStart && target.UTCEndTime <= rangeEnd)
                 || (target.UTCStartTime <= rangeStart && target.UTCEndTime >= rangeStart))
            );

            var result = marketScheduleData.ToList();

            _logger.LogInformation("Found {Count} market schedules in date range for {Exchange}.{Symbol}",
                result.Count, exchange, symbol);

            // If no results in date range, get all data for the symbol/exchange
            if (result.Count == 0)
            {
                _logger.LogInformation(
                    "No schedules found in date range for {Exchange}.{Symbol}, retrieving all schedules", exchange,
                    symbol);
                result = (await _marketScheduleService.GetListByFilterAsync(target =>
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