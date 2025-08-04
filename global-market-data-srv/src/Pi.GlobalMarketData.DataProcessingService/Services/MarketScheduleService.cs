using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketData.DataProcessingService.Services;

public class MarketScheduleService : BackgroundService
{
    private readonly ILogger<MarketScheduleService> _logger;
    private readonly TimeSpan _marketDataCacheCheckerDuration = TimeSpan.FromHours(1);

    private readonly IMarketScheduleDataService _marketScheduleService;
    private readonly TimeSpan _pollingInterval;
    private readonly IRedisV2Publisher _redisCache;
    private readonly IMongoService<WhiteList> _whitelistService;

    /// <summary>
    /// </summary>
    /// <param name="whitelistService"></param>
    /// <param name="marketScheduleService"></param>
    /// <param name="redisCache"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public MarketScheduleService(
        IMongoService<WhiteList> whitelistService,
        IMarketScheduleDataService marketScheduleService,
        IRedisV2Publisher redisCache,
        ILogger<MarketScheduleService> logger)
    {
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _pollingInterval = TimeSpan.FromMinutes(1);
    }

    /// <summary>
    ///     Executes the background service's main loop, checking and caching market schedules.
    /// </summary>
    /// <param name="stoppingToken">Token to signal service cancellation</param>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Market schedule service started.");

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
        try
        {
            const string scheduleCheckerKey = "schedule-checker-cached-all-instance-profiles";
            var checkerData = await _redisCache.GetAsync<string>(scheduleCheckerKey, true) ?? string.Empty;

            if (string.IsNullOrEmpty(checkerData))
            {
                var whitelists = await GetWhitelistAsync();
                _logger.LogInformation("Retrieved {Count} whitelist entries for all instance profiles",
                    whitelists.Count);

                var processedCount = 0;
                var symbolsWithoutSchedules = new List<(string Exchange, string Symbol)>();

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var whitelist in whitelists)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    if (string.IsNullOrEmpty(whitelist.Symbol) || string.IsNullOrEmpty(whitelist.Exchange))
                    {
                        _logger.LogWarning("Skipping whitelist entry with missing Symbol or Exchange");
                        continue;
                    }

                    processedCount++;

                    // Set exchange to cache
                    await SetWhiteListExchange(whitelist.Symbol, whitelist.Exchange);

                    // Get market schedules
                    var schedules =
                        await _marketScheduleService.GetMarketSchedulesAsync(whitelist.Symbol, whitelist.Exchange);

                    if (!schedules.Any())
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

    private async Task<List<WhiteList>> GetWhitelistAsync()
    {
        try
        {
            const string cachedKey = "white-list-cached-all-instance-profile";

            var cachedData = await _redisCache.GetAsync<List<WhiteList>>(cachedKey, true) ?? [];
            if (cachedData.Count > 0)
                return cachedData;

            var list = await _whitelistService.GetAllByFilterAsync(target =>
                target.IsWhitelist && !string.IsNullOrEmpty(target.InstanceConfigProfile)
                && !string.IsNullOrEmpty(target.Symbol)
                && !string.IsNullOrEmpty(target.Exchange));

            var data = list.ToList();
            if (data.Count > 0)
                await _redisCache.SetAsync(cachedKey, data, true, TimeSpan.FromHours(24));

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get whitelist for all InstanceProfile: {Message}", ex.Message);
            return [];
        }
    }

    private async Task SetScheduleCheckerCache(string scheduleCheckerKey)
    {
        try
        {
            // Store the current timestamp in Redis (explicitly UTC)
            var utcNow = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
            await _redisCache.SetAsync(scheduleCheckerKey, utcNow.Ticks.ToString(), true,
                _marketDataCacheCheckerDuration);

            _logger.LogInformation("Successfully set schedule checker cache with expiration of {Hours} hours",
                _marketDataCacheCheckerDuration.TotalHours);
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
                "Schedule checker all instance profiles cache hit. Last checked at {LastCheck}, expires in {TimeLeft} minutes",
                lastCheckTime, timeUntilExpiry.TotalMinutes);
        }
        else
        {
            _logger.LogWarning("Found invalid schedule checker cache data. Will be refreshed on next cycle.");
        }
    }

    private void LogSymbolsWithoutSchedules(List<(string Exchange, string Symbol)> symbolsWithoutSchedules)
    {
        if (symbolsWithoutSchedules.Count > 0)
            foreach (var (exchange, symbol) in symbolsWithoutSchedules)
                _logger.LogWarning("Missing schedule: {Exchange}.{Symbol}", exchange, symbol);
    }

    private async Task SetWhiteListExchange(string symbol, string exchange)
    {
        // Try to get from cache first
        var cacheKey = $"{CacheKey.WhiteListProcessor}-exchange-{symbol}";

        if (!string.IsNullOrEmpty(exchange))
            // Cache the result for future use
            await _redisCache.SetAsync(cacheKey, exchange, false, TimeSpan.FromDays(30));
    }
}