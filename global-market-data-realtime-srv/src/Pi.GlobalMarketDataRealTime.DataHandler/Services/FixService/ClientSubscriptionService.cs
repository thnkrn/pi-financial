using System.Collections.Concurrent;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Application.Constants;
using Pi.GlobalMarketDataRealTime.DataHandler.Converters;
using Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;
using Pi.GlobalMarketDataRealTime.DataHandler.Models;
using Pi.GlobalMarketDataRealTime.DataHandler.Models.FixModel;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;
using Polly;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;

public class ClientSubscriptionService : BackgroundService
{
    private static readonly Random Random = new();
    private readonly ConcurrentDictionary<string, DateTime>? _activeRequests;
    private readonly IClient _client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ClientSubscriptionService> _logger;
    private readonly TimeSpan _marketDataCacheDuration = TimeSpan.FromHours(12);
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly ICacheService _redisCache;
    private readonly TimeSpan _whiteListDataCacheDuration = TimeSpan.FromHours(4);
    private readonly IMongoService<WhiteList> _whitelistService;
    private CancellationTokenSource _clientCts = new();
    private volatile bool _isClientRunning;
    private volatile bool _isSubscriptionRunning;
    private Task? _subscriptionTask;

    /// <summary>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    /// <param name="whitelistService"></param>
    /// <param name="marketScheduleService"></param>
    /// <param name="redisCache"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public ClientSubscriptionService(
        IClient client,
        IConfiguration configuration,
        IMongoService<WhiteList> whitelistService,
        IMongoService<MarketSchedule> marketScheduleService,
        ICacheService redisCache,
        ILogger<ClientSubscriptionService> logger)
    {
        _isSubscriptionRunning = false;
        _activeRequests = new ConcurrentDictionary<string, DateTime>();
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    await StartSubscriptionTask();
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
                catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "Service is stopping due to cancellation request.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the main execution loop. Continuing execution.");
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
        }
        catch (OperationCanceledException ex) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug(ex, "Service execution was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unhandled exception occurred in the main execution loop");
        }
        finally
        {
            await StopSubscriptionTask();
            _logger.LogDebug("Service execution completed.");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _activeRequests?.Clear();
        await base.StopAsync(cancellationToken);
    }

    private bool ShouldSendRequest(string symbol, string exchange)
    {
        var requestKey = $"{symbol}.{exchange}";
        var currentTime = DateTime.UtcNow;

        if (_activeRequests != null && _activeRequests.TryGetValue(requestKey, out var lastRequestTime))
        {
            _logger.LogDebug(
                "Skipping duplicate request for {Symbol}.{Exchange}, last request was at {LastRequestTime}",
                symbol, exchange, lastRequestTime);
            return false;
        }

        _activeRequests?.AddOrUpdate(requestKey, currentTime, (_, _) => currentTime);
        return true;
    }

    private void RemoveInactiveRequest(string symbol, string exchange)
    {
        var requestKey = $"{symbol}.{exchange}";
        if (_activeRequests != null && _activeRequests.TryRemove(requestKey, out var lastRequestTime))
            _logger.LogDebug(
                "Remove {Symbol}.{Exchange} at {LastRequestTime}",
                symbol, exchange, lastRequestTime);
    }

    private async Task MonitorClientAsync(IClient client, CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);


        while (!stoppingToken.IsCancellationRequested)
            try
            {
                List<WhiteList> data;

                var instanceConfigProfile = _configuration.GetValue<string>(ConfigurationKeys.InstanceConfigProfile);
                var cacheKey = $"{ConstantKeys.MarketWhiteListDataCacheKey}_{instanceConfigProfile}";
                var whiteListDataCache = await _redisCache.GetAsync<string>(cacheKey) ?? string.Empty;
                var jsonSettings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new ObjectIdConverter() }
                };

                if (!string.IsNullOrEmpty(whiteListDataCache.Trim()))
                {
                    data = JsonConvert.DeserializeObject<List<WhiteList>>(whiteListDataCache, jsonSettings) ?? [];
                }
                else
                {
                    var list = await _whitelistService.GetListByFilterAsync(target =>
                        !string.IsNullOrEmpty(target.InstanceConfigProfile)
                        && !string.IsNullOrEmpty(target.Symbol)
                        && target.InstanceConfigProfile.Equals(instanceConfigProfile,
                            StringComparison.OrdinalIgnoreCase));
                    data = list.ToList();
                    var copyData = data;

                    _ = Task.Run(async () =>
                    {
                        var dataJson = JsonConvert.SerializeObject(copyData, jsonSettings);
                        await _redisCache.SetAsync(cacheKey, dataJson, _whiteListDataCacheDuration);
                    }, stoppingToken);
                }

                var whitelistData = data;
                await ParallelMonitorClientAsync(client, whitelistData, stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in MonitorClientAsync: {Exception}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }

    private async Task ParallelMonitorClientAsync(IClient client, IEnumerable<WhiteList> whitelistData,
        CancellationToken stoppingToken)
    {
        var data = whitelistData
            .Select(e => new WhiteListData
            {
                Symbol = e.Symbol,
                SecurityId = $"{e.Symbol}.{e.Exchange}",
                Exchange = e.Exchange ?? string.Empty,
                SecurityIdSource = "111"
            }).ToList();

        if (!await ValidateClientState(client))
        {
            _activeRequests?.Clear();
            _logger.LogWarning("Client is not in valid state.");

            // Wait before Stop subscription task
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            await StopSubscriptionTask();
            return;
        }

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            CancellationToken = stoppingToken
        };

        await Parallel.ForEachAsync(data, parallelOptions, async (symbol, token) =>
        {
            var isTradingAllowed =
                await CheckMarketSession(symbol.Symbol ?? string.Empty, symbol.Exchange ?? string.Empty);
            if (!isTradingAllowed)
            {
                _logger.LogWarning("{Symbol} not allowed for feed because not in online session",
                    symbol.Symbol);

                RemoveInactiveRequest(symbol.Symbol ?? string.Empty, symbol.Exchange ?? string.Empty);
                return;
            }

            if (!ShouldSendRequest(symbol.Symbol ?? string.Empty, symbol.Exchange ?? string.Empty)) return;

            try
            {
                var symbols = new List<(string symbol, string securityID, string securityIDSource)>
                {
                    (symbol.Symbol ?? string.Empty,
                        symbol.SecurityId ?? string.Empty,
                        symbol.SecurityIdSource ?? string.Empty)
                };

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var jitter = Random.Next(10000); // 0-9999 random number
                var requestId = $"reqID{timestamp}{jitter:D4}";
                var marketDataRequest = FixUtil.CreateMarketDataRequest(requestId, symbols);

                _logger.LogDebug("{Symbol} sending market data request", symbol.Symbol);
                await client.SendAsync(marketDataRequest, token);
                await Task.Delay(50, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending market data request for {Symbol}", symbol.Symbol);
            }
        });
    }

    private static async Task<bool> ValidateClientState(IClient client)
    {
        // Verify session is logged in
        return await client.CheckListenerSession();
    }

    private async Task StartSubscriptionTask()
    {
        if (_isSubscriptionRunning)
        {
            _logger.LogWarning("Subscription task is already running. Skipping start request.");
            return;
        }

        if (_subscriptionTask == null)
        {
            _isSubscriptionRunning = true;
            _subscriptionTask = Task.Run(async () =>
            {
                try
                {
                    await StartClient(_clientCts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogWarning(ex, "Subscription task was canceled.");
                    _isSubscriptionRunning = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the subscription task.");
                    _isSubscriptionRunning = false;
                }
            }, _clientCts.Token);
        }

        await Task.CompletedTask;
    }

    private async Task StopSubscriptionTask()
    {
        if (!_isSubscriptionRunning)
        {
            _logger.LogWarning("Subscription task is not running. Skipping stop request.");
            return;
        }

        try
        {
            await StopClient();
            await _clientCts.CancelAsync();

            if (_subscriptionTask != null)
            {
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));
                _ = await Task.WhenAny(_subscriptionTask, timeoutTask);
                _subscriptionTask = null;
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Subscription task cancellation completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while stopping the subscription task.");
        }
        finally
        {
            _isSubscriptionRunning = false;
            _isClientRunning = false;
            _clientCts.Dispose();
            _clientCts = new CancellationTokenSource();
            _subscriptionTask = null;

            await SafeShutdown();
        }
    }

    private async Task StartClient(CancellationToken stoppingToken)
    {
        if (_isClientRunning)
        {
            _logger.LogWarning("Client is already running. Skipping start request.");
            return;
        }

        try
        {
            await SafeShutdown(stoppingToken);

            var fixConfig = new FIX();
            _configuration.GetSection(ConfigurationKeys.FixConfig).Bind(fixConfig);

            var fixConfigFiles = _configuration.GetValue(ConfigurationKeys.FixConfigFiles, string.Empty);
            if (!string.IsNullOrWhiteSpace(fixConfigFiles))
            {
                var files = JsonConvert.DeserializeObject<List<FixConfig>>(fixConfigFiles);
                if (files != null)
                    fixConfig.CONFIG_FILES = [.. files];
            }

            if (fixConfig.CONFIG_FILES == null)
                throw new SubscriptionServiceException(nameof(fixConfig.CONFIG_FILES));

            _logger.LogDebug("Found {Count} FIX's configuration files", fixConfig.CONFIG_FILES.Count);

            var currentConfigIndex = 0;
            var retryPolicy = CreateRetryPolicy();

            _isClientRunning = true;

            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    var config = fixConfig.CONFIG_FILES[currentConfigIndex];
                    await ConnectToServer(config, retryPolicy, stoppingToken);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred with server.");
                    currentConfigIndex = (currentConfigIndex + 1) % fixConfig.CONFIG_FILES.Count;
                }

            _logger.LogDebug("Client started successfully");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Client start operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while starting the client.");
        }
        finally
        {
            _isClientRunning = false;
        }
    }

    private async Task StopClient()
    {
        if (!_isClientRunning)
        {
            _logger.LogWarning("Client is not running. Skipping stop request.");
            return;
        }

        _logger.LogDebug("Stopping client");

        try
        {
            await _clientCts.CancelAsync();
            await Task.Delay(TimeSpan.FromSeconds(5));
            await SafeShutdown();
            _logger.LogDebug("Client shut down successfully");
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Client stop operation was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during client logout");
        }
        finally
        {
            _isClientRunning = false;
        }
    }

    private async Task ConnectToServer(FixConfig configFile, IAsyncPolicy retryPolicy, CancellationToken stoppingToken)
    {
        if (configFile == null)
            throw new SubscriptionServiceException(nameof(configFile));

        var config = configFile.ConfigFile;
        await retryPolicy.ExecuteAsync(async () =>
        {
            if (config == null)
                throw new SubscriptionServiceException(nameof(config));

            if (!_isSubscriptionRunning)
            {
                _client.Setup(config);
                await _client.StartAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            var isSessionLogin = await _client.CheckListenerSession();
            if (!isSessionLogin)
            {
                await SafeShutdown(stoppingToken);
                throw new SubscriptionServiceException("Session not Log in.");
            }

            if (_client.State is ClientState.ShuttingDown or ClientState.Disconnected)
                return;

            _logger.LogDebug("Start client by config file ({Config})", config);

            await MonitorClientAsync(_client, _clientCts.Token);
        });
    }

    private async Task SafeShutdown(CancellationToken stoppingToken = default)
    {
        try
        {
            _activeRequests?.Clear();

            var instanceConfigProfile = _configuration.GetValue(ConfigurationKeys.InstanceConfigProfile, string.Empty);
            var cacheKey = $"{ConstantKeys.MarketWhiteListDataCacheKey}_{instanceConfigProfile}";

            await _redisCache.RemoveAsync(cacheKey);
            await _client.ShutdownAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Error during client shut down. This may be normal if the client was already shut down.");
        }
        finally
        {
            _isSubscriptionRunning = false;
            _isClientRunning = false;
        }
    }

    private IAsyncPolicy CreateRetryPolicy()
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(exception, "Attempt {RetryCount} failed. Waiting {TimeSpan} before next retry.",
                        retryCount, timeSpan);
                });
    }

    private async Task<bool> CheckMarketSession(string symbol, string exchange)
    {
        if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(exchange))
            throw new ArgumentException("Symbol and exchange must not be empty");

        try
        {
            var marketSchedules = await GetMarketSchedulesAsync(symbol, exchange);

            if (marketSchedules.Length == 0)
            {
                _logger.LogWarning("No market schedules found for {Symbol} {Exchange}", symbol, exchange);
                return false;
            }

            return IsCurrentlyTradingSession(marketSchedules, symbol, exchange);
        }
        catch (TimeZoneNotFoundException ex)
        {
            _logger.LogError(ex, "TimeZone not found in the container");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking market session for {Symbol} {Exchange}", symbol, exchange);
            return false;
        }
    }

    private async Task<MarketSchedule[]> GetMarketSchedulesAsync(string symbol, string exchange)
    {
        var instanceConfigProfile = _configuration.GetValue(ConfigurationKeys.InstanceConfigProfile, string.Empty);
        var cacheKey = $"{ConstantKeys.MarketScheduleCacheKey}_{exchange}_{symbol}_{instanceConfigProfile}";
        var jsonSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new ObjectIdConverter() }
        };

        try
        {
            var cachedData = await _redisCache.GetAsync<string>(cacheKey) ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(cachedData))
                return JsonConvert.DeserializeObject<MarketSchedule[]>(cachedData, jsonSettings) ?? [];

            var marketScheduleData = await GetMarketScheduleData(symbol, exchange);
            var marketSchedules = marketScheduleData as MarketSchedule[] ?? marketScheduleData.ToArray();

            _ = Task.Run(async () =>
            {
                var marketSchedulesJson = JsonConvert.SerializeObject(marketSchedules, jsonSettings);
                await _redisCache.SetAsync(cacheKey, marketSchedulesJson, _marketDataCacheDuration);
            });

            return marketSchedules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving market schedules for {Symbol} {Exchange}", symbol, exchange);

            var marketScheduleData = await GetMarketScheduleData(symbol, exchange);
            var marketSchedules = marketScheduleData as MarketSchedule[] ?? marketScheduleData.ToArray();

            _ = Task.Run(async () =>
            {
                var marketSchedulesJson = JsonConvert.SerializeObject(marketSchedules, jsonSettings);
                await _redisCache.SetAsync(cacheKey, marketSchedulesJson, _marketDataCacheDuration);
            });

            return marketSchedules;
        }
    }

    private async Task<IEnumerable<MarketSchedule>> GetMarketScheduleData(string symbol, string exchange)
    {
        var utcNow = DateTime.UtcNow;
        var rangeStart = utcNow.AddHours(-48);
        var rangeEnd = utcNow.AddHours(48);

        var marketScheduleData = await _marketScheduleService.GetListByFilterAsync(target =>
            !string.IsNullOrEmpty(target.Exchange)
            && target.Exchange.Equals(exchange)
            && !string.IsNullOrEmpty(target.Symbol)
            && target.Symbol.Equals(symbol)
            && target.UTCStartTime >= rangeStart && target.UTCEndTime <= rangeEnd
        );
        return marketScheduleData;
    }

    private bool IsCurrentlyTradingSession(MarketSchedule[] marketSchedules, string symbol, string exchange)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(ConstantKeys.DefaultTimezone);
        var currentUtcTime = DateTime.UtcNow;
        var currentLocalTime = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, timeZone);

        var validSchedules = marketSchedules
            .Where(e => string.Equals(e.Symbol, symbol, StringComparison.InvariantCultureIgnoreCase)
                        && string.Equals(e.Exchange, exchange, StringComparison.InvariantCultureIgnoreCase)
                        && e is { UTCStartTime: not null, UTCEndTime: not null })
            .Select(e => new
            {
                e.MarketSession,
                LocalStartTime = TimeZoneInfo.ConvertTimeFromUtc(e.UTCStartTime ?? default, timeZone),
                LocalEndTime = TimeZoneInfo.ConvertTimeFromUtc(e.UTCEndTime ?? default, timeZone)
            })
            .ToList();

        if (validSchedules.Count == 0)
        {
            _logger.LogWarning("No valid market schedule entries found for {Symbol} {Exchange}", symbol, exchange);
            return false;
        }

        var tradingSession = validSchedules.Find(e =>
            currentLocalTime >= e.LocalStartTime &&
            currentLocalTime <= e.LocalEndTime);

        if (tradingSession == null)
        {
            _logger.LogDebug("No active trading session found for {Symbol} {Exchange} at {CurrentTime}",
                symbol, exchange, currentLocalTime);
            return false;
        }

        var notAllowedState = new[] { "offline", "clearing" };
        var marketSession = (tradingSession.MarketSession ?? string.Empty).ToLowerInvariant();
        var isFeedAllowed = !notAllowedState.Contains(marketSession);

        _logger.LogDebug(
            "Market session check result: {IsFeedAllowed} for {Symbol} {Exchange} at {CurrentTime} with session {MarketSession}",
            isFeedAllowed, symbol, exchange, currentLocalTime, marketSession);

        return isFeedAllowed;
    }
}