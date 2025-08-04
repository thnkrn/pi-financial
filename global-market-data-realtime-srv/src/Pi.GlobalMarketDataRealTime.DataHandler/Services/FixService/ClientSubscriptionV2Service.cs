using System.Collections.Concurrent;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Application.Constants;
using Pi.GlobalMarketDataRealTime.DataHandler.Converters;
using Pi.GlobalMarketDataRealTime.DataHandler.Exceptions;
using Pi.GlobalMarketDataRealTime.DataHandler.Interfaces;
using Pi.GlobalMarketDataRealTime.DataHandler.Models;
using Pi.GlobalMarketDataRealTime.DataHandler.Models.FixModel;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Entities;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataRealTime.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;
using Polly;
using QuickFix;

namespace Pi.GlobalMarketDataRealTime.DataHandler.Services.FixService;

public class ClientSubscriptionV2Service : BackgroundService, ILogoutNotificationHandler
{
    private readonly ConcurrentDictionary<string, DateTime>? _activeRequests;
    private readonly int _batchIntervalMs;
    private readonly int _batchSize;
    private readonly IFixClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly string? _instanceConfigProfile;

    private readonly JsonSerializerSettings _jsonSettings = new()
    {
        Converters = new List<JsonConverter> { new ObjectIdConverter() }
    };

    private readonly ILogger<ClientSubscriptionV2Service> _logger;
    private readonly IMongoService<MarketSchedule> _marketScheduleService;
    private readonly IRedisV2Publisher _redisCache;
    private readonly int _requestIntervalMs;
    private readonly object _subscriptionLock = new();
    private readonly ConcurrentDictionary<string, DateTime> _symbolLastUpdatedTime;
    private readonly TimeSpan _symbolRefreshInterval;
    private readonly SemaphoreSlim _throttleSemaphore;
    private readonly IMongoService<WhiteList> _whitelistService;
    private IClient _client;
    private CancellationTokenSource _clientCts = new();
    private volatile bool _isClientRunning;
    private volatile bool _isSubscriptionRunning;
    private Task? _subscriptionTask;
    private volatile bool _tryToStopSubscription;

    /// <summary>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="configuration"></param>
    /// <param name="whitelistService"></param>
    /// <param name="marketScheduleService"></param>
    /// <param name="redisCache"></param>
    /// <param name="logger"></param>
    /// <param name="clientFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public ClientSubscriptionV2Service(
        IClient client,
        IConfiguration configuration,
        IMongoService<WhiteList> whitelistService,
        IMongoService<MarketSchedule> marketScheduleService,
        IRedisV2Publisher redisCache,
        ILogger<ClientSubscriptionV2Service> logger,
        IFixClientFactory clientFactory)
    {
        _isSubscriptionRunning = false;
        _tryToStopSubscription = false;
        _activeRequests = new ConcurrentDictionary<string, DateTime>();
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _whitelistService = whitelistService ?? throw new ArgumentNullException(nameof(whitelistService));
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));

        // Instance profile
        _instanceConfigProfile = _configuration.GetValue(ConfigurationKeys.InstanceConfigProfile, string.Empty);

        if (string.IsNullOrEmpty(_instanceConfigProfile))
            throw new InvalidOperationException("The 'InstanceConfigProfile' cannot be null or empty.");

        // Initialize performance optimization parameters
        _batchSize = _configuration.GetValue("SubscriptionService:BatchSize", 50);
        _requestIntervalMs = _configuration.GetValue("SubscriptionService:RequestIntervalMs", 10);
        _batchIntervalMs = _configuration.GetValue("SubscriptionService:BatchIntervalMs", 50);

        // Max concurrent requests based on CPU cores
        var maxConcurrency =
            _configuration.GetValue("SubscriptionService:MaxConcurrency", Environment.ProcessorCount * 2);
        _throttleSemaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

        // Track last updated time for each symbol to implement smart refresh
        _symbolLastUpdatedTime = new ConcurrentDictionary<string, DateTime>();
        _symbolRefreshInterval = TimeSpan.FromMinutes(
            _configuration.GetValue("SubscriptionService:SymbolRefreshIntervalMinutes", 15));
    }

    public async Task NotifyLogout(SessionID sessionId)
    {
        _logger.LogInformation("Received logout notification from FIX session {SessionID}", sessionId);

        try
        {
            // Clear active requests to prevent stale state
            _activeRequests?.Clear();

            // Stop subscription task
            await StopSubscriptionTask();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StopSubscriptionTask failed");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
                try
                {
                    bool shouldStart;
                    lock (_subscriptionLock)
                    {
                        shouldStart = !_tryToStopSubscription;
                    }

                    if (shouldStart)
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
        _symbolLastUpdatedTime.Clear();
        await base.StopAsync(cancellationToken);
    }

    private bool ShouldSendRequest(string symbol, string exchange)
    {
        var requestKey = $"{symbol}.{exchange}";
        var currentTime = DateTime.UtcNow;

        // Check if symbol is already in active requests
        if (_activeRequests != null && _activeRequests.TryGetValue(requestKey, out var lastRequestTime))
        {
            _logger.LogDebug(
                "Skipping duplicate request for {Symbol}.{Exchange}, last request was at {LastRequestTime}",
                symbol, exchange, lastRequestTime);
            return false;
        }

        // Check if symbol needs to be refreshed based on refresh interval
        if (_symbolLastUpdatedTime.TryGetValue(requestKey, out var lastUpdatedTime)
            && currentTime - lastUpdatedTime < _symbolRefreshInterval)
            return false;

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

    private void UpdateSymbolRequestTime(string symbol, string exchange)
    {
        var requestKey = $"{symbol}.{exchange}";
        _symbolLastUpdatedTime.AddOrUpdate(requestKey, DateTime.UtcNow, (_, _) => DateTime.UtcNow);
    }

    private async Task MonitorClientAsync(IClient client, CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);

        if (string.IsNullOrEmpty(_instanceConfigProfile))
        {
            _logger.LogError("The configuration of INSTANCE_CONFIG_PROFILE not found or not injected!");
            throw new InvalidOperationException();
        }

        _logger.LogInformation("InstanceConfigProfile is {InstanceConfigProfile}", _instanceConfigProfile);

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var list = await _whitelistService.GetListByFilterAsync(target =>
                    !string.IsNullOrEmpty(target.InstanceConfigProfile)
                    && target.IsWhitelist != null
                    && target.IsWhitelist == true
                    && !string.IsNullOrEmpty(target.Symbol)
                    && target.InstanceConfigProfile.ToUpper().Equals(_instanceConfigProfile.ToUpper()));
                var data = list.ToList();

                await OptimizedMonitorClientAsync(client, data, stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in MonitorClientAsync: {Exception}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }

    private async Task OptimizedMonitorClientAsync(IClient client, IEnumerable<WhiteList> whitelistData,
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

        // Group symbols by exchange to optimize processing
        // Use non-null exchange or empty string to avoid null reference issues
        var exchangeGroups = data
            .GroupBy(s => s.Exchange ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Process each exchange group
        foreach (var (exchange, symbols) in exchangeGroups)
        {
            _logger.LogInformation("Processing {Count} symbols for exchange {Exchange}",
                symbols.Count, exchange);

            // Process in batches
            for (var i = 0; i < symbols.Count; i += _batchSize)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                // Take a batch of symbols
                var batch = symbols.Skip(i).Take(_batchSize).ToList();
                await ProcessSymbolBatchAsync(client, batch, stoppingToken);

                // Delay between batches to avoid overwhelming the system
                if (i + _batchSize < symbols.Count)
                    await Task.Delay(_batchIntervalMs, stoppingToken);
            }
        }
    }

    private async Task ProcessSymbolBatchAsync(IClient client, List<WhiteListData> symbolBatch,
        CancellationToken stoppingToken)
    {
        // Create tasks for each symbol that can be processed in parallel
        var tasks = new List<Task>(symbolBatch.Count);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var symbol in symbolBatch)
        {
            // Skip symbols that don't need to be processed
            if (!ShouldSendRequest(symbol.Symbol ?? string.Empty, symbol.Exchange ?? string.Empty))
                continue;

            // Use semaphore to limit concurrency
            await _throttleSemaphore.WaitAsync(stoppingToken);

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    await ProcessSymbolAsync(client, symbol, stoppingToken);
                }
                finally
                {
                    _throttleSemaphore.Release();
                }
            }, stoppingToken));

            // Small delay between each request in the batch
            await Task.Delay(_requestIntervalMs, stoppingToken);
        }

        // Wait for all tasks in this batch to complete
        await Task.WhenAll(tasks);
    }

    private async Task ProcessSymbolAsync(IClient client, WhiteListData symbol, CancellationToken stoppingToken)
    {
        try
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

            var symbols = new List<(string symbol, string securityID, string securityIDSource)>
            {
                (symbol.Symbol ?? string.Empty,
                    symbol.SecurityId ?? string.Empty,
                    symbol.SecurityIdSource ?? string.Empty)
            };

            var requestKey = $"{symbol.Symbol}.{symbol.Exchange}";
            var requestId = $"{requestKey}_{Guid.NewGuid():N}";
            var marketDataRequest = FixUtil.CreateMarketDataRequest(requestId, symbols);

            _logger.LogDebug("{Symbol} sending market data request", symbol.Symbol);
            await client.SendAsync(marketDataRequest, stoppingToken);

            UpdateSymbolRequestTime(symbol.Symbol ?? string.Empty, symbol.Exchange ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending market data request for {Symbol}", symbol.Symbol);
            RemoveInactiveRequest(symbol.Symbol ?? string.Empty, symbol.Exchange ?? string.Empty);
        }
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
        var sessionLogon = await _client.CheckListenerSession();
        if (!_isSubscriptionRunning && sessionLogon)
        {
            _logger.LogWarning("Subscription task is not running. Skipping stop request.");
            return;
        }

        // Stopping the client
        lock (_subscriptionLock)
        {
            _tryToStopSubscription = true;
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

        // Stopped the client
        lock (_subscriptionLock)
        {
            _tryToStopSubscription = false;
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
            if (!string.IsNullOrEmpty(fixConfigFiles))
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

            // Get a fresh client from our factory
            IClient client;

            // Check if client is in a valid state
            bool needNewClient;
            try
            {
                needNewClient = _client.State == ClientState.Disconnected;
            }
            catch (ObjectDisposedException)
            {
                needNewClient = true;
            }

            if (needNewClient)
            {
                // Use the factory to get a fresh client instance
                client = _clientFactory.RecreateClient();
                // Update our reference
                _client = client;
                _logger.LogDebug("Created new client instance via factory");
            }
            else
            {
                client = _client;
            }

            if (!_isSubscriptionRunning)
            {
                // The factory already handles Setup, so we don't need to call it again
                await client.StartAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            var isSessionLogin = await client.CheckListenerSession();
            if (!isSessionLogin)
            {
                await SafeShutdown(stoppingToken);
                throw new SubscriptionServiceException("Session not Log in.");
            }

            if (client.State is ClientState.ShuttingDown or ClientState.Disconnected)
                return;

            _logger.LogDebug("Start client by config file ({Config})", config);

            await MonitorClientAsync(client, _clientCts.Token);
        });
    }

    private async Task SafeShutdown(CancellationToken stoppingToken = default)
    {
        try
        {
            _activeRequests?.Clear();
            _symbolLastUpdatedTime.Clear();

            // Try to shut down the current client gracefully
            try
            {
                await _client.ShutdownAsync(stoppingToken);
                await _client.Reset(stoppingToken);
            }
            catch (ObjectDisposedException)
            {
                // Get a fresh client through the factory
                _client = _clientFactory.RecreateClient();
            }
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
        var cacheKey = $"{ConstantKeys.MarketScheduleCacheKey}_{exchange}_{symbol}_{_instanceConfigProfile}";

        try
        {
            var cachedData = await _redisCache.GetAsync<string>(cacheKey, true) ?? string.Empty;

            if (!string.IsNullOrEmpty(cachedData))
                return JsonConvert.DeserializeObject<MarketSchedule[]>(cachedData, _jsonSettings) ?? [];

            var marketScheduleData = await GetMarketScheduleData(symbol, exchange);
            var marketSchedules = marketScheduleData.ToArray();

            return marketSchedules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving market schedules for {Symbol} {Exchange}", symbol, exchange);
            return [];
        }
    }

    private async Task<IEnumerable<MarketSchedule>> GetMarketScheduleData(string symbol, string exchange)
    {
        var normalizedSymbol = symbol.Trim();
        var normalizedExchange = exchange.Trim();

        try
        {
            // First try to get data within the date range
            var utcNow = DateTime.UtcNow;
            var rangeStart = utcNow.AddDays(-2).Date;
            var rangeEnd = utcNow.AddDays(2).Date.AddDays(1).AddTicks(-1);
            var marketScheduleData = await _marketScheduleService.GetListByFilterAsync(target =>
                target.Exchange == normalizedExchange &&
                target.Symbol == normalizedSymbol &&
                ((target.UTCStartTime >= rangeStart && target.UTCStartTime <= rangeEnd)
                 || (target.UTCEndTime >= rangeStart && target.UTCEndTime <= rangeEnd)
                 || (target.UTCStartTime <= rangeStart && target.UTCEndTime >= rangeEnd))
            );

            var result = marketScheduleData.ToList();

            _logger.LogInformation("Found {Count} market schedules in date range for {Exchange}.{Symbol}",
                result.Count, exchange, symbol);

            // If no results in date range, get all data for the symbol/exchange
            if (result.Count == 0)
            {
                _logger.LogDebug(
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

    private bool IsCurrentlyTradingSession(MarketSchedule[] marketSchedules, string symbol, string exchange)
    {
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(ConstantKeys.DefaultTimezone);
        var currentUtcTime = DateTime.UtcNow;
        var currentLocalTime = TimeZoneInfo.ConvertTimeFromUtc(currentUtcTime, timeZone);

        var validSchedules = marketSchedules
            .Where(e => string.Equals(e.Symbol, symbol)
                        && string.Equals(e.Exchange, exchange)
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