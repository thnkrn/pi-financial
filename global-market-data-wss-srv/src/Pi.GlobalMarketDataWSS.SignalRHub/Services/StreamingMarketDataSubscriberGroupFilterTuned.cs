using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.ObjectPool;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Domain.Models.Request;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;
using Pi.GlobalMarketDataWSS.Infrastructure.Helpers;
using Pi.GlobalMarketDataWSS.SignalRHub.Hubs;
using Pi.GlobalMarketDataWSS.SignalRHub.Interfaces;
using Polly;
using StackExchange.Redis;

namespace Pi.GlobalMarketDataWSS.SignalRHub.Services;

public class StreamingMarketDataSubscriberGroupFilterTuned : IStreamingMarketDataSubscriberGroupFilterTuned,
    IAsyncDisposable
{
    // Constants - tuned for performance
    private const int MaxConcurrentProcessing = 12;
    private const int BatchSize = 20;
    private const int MessageChannelCapacity = 100000;
    private const int MemoryThresholdMb = 1024;
    private static readonly TimeSpan ConnectionCleanupInterval = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan ConnectionExpirationTime = TimeSpan.FromHours(2);

    // Quick lookup for active symbols
    private readonly ConcurrentDictionary<string, byte> _activeSymbols = new();

    // Reduced for faster processing
    private readonly TimeSpan _batchWindow = TimeSpan.FromMilliseconds(10);

    // Core service dependencies
    private readonly string _channel;
    private readonly IAsyncPolicy _circuitBreakerPolicy;

    // Background tasks
    private readonly Timer _cleanupTimer;

    // Thread-safe collections for state management
    private readonly ConcurrentDictionary<string, DateTime> _connectionLastActivity = new();
    private readonly string _groupName;
    private readonly IHubContext<StreamingHubGroupFilterTuned> _hubContext;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<StreamingMarketDataSubscriberGroupFilterTuned> _logger;

    // Message processing pipeline
    private readonly Channel<RedisMessage> _messageChannel;
    private readonly string _methodName;
    private readonly SemaphoreSlim _processingThrottle;
    private readonly IConnectionMultiplexer _redisConnection;

    // Added Redis retry policy
    private readonly IAsyncPolicy _redisRetryPolicy;
    private readonly ObjectPool<MarketStreamingResponse> _responsePool;
    private readonly TimeSpan _statsReportingInterval = TimeSpan.FromMinutes(3);
    private readonly Timer _statsReportingTimer;

    // Message size monitoring (new)
    private readonly ConcurrentDictionary<string, int> _symbolMessageSizes = new();

    // Performance monitoring
    private readonly ConcurrentDictionary<string, SymbolStats> _symbolStats = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _symbolSubscriptions = new();

    private Task? _executingTask;
    private volatile bool _isSubscribed;
    private DateTime _lastGcCollectTime = DateTime.MinValue;
    private Task? _processingTask;
    private CancellationTokenSource? _stoppingCts;

    // Memory management

    public StreamingMarketDataSubscriberGroupFilterTuned(
        IConnectionMultiplexer redisConnection,
        IHubContext<StreamingHubGroupFilterTuned> hubContext,
        IConfiguration configuration,
        ILogger<StreamingMarketDataSubscriberGroupFilterTuned> logger,
        IAsyncPolicy circuitBreakerPolicy)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _circuitBreakerPolicy = circuitBreakerPolicy ?? throw new ArgumentNullException(nameof(circuitBreakerPolicy));

        // Configure Redis retry policy for handling timeouts
        _redisRetryPolicy = Policy
            .Handle<RedisTimeoutException>()
            .Or<RedisConnectionException>()
            .WaitAndRetryAsync(
                3, // Number of retries
                retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt - 1)), // Exponential backoff
                (exception, timeSpan, retryCount, _) =>
                {
                    _logger.LogWarning(
                        "Redis operation attempt {RetryCount} failed with: {ExceptionMessage}. Retrying in {RetryTimeSpan}ms",
                        retryCount, exception.Message, timeSpan.TotalMilliseconds);
                });

        // Configure JSON options once
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        // Initialize thread-safe bounded channel with overflow handling
        _messageChannel = Channel.CreateBounded<RedisMessage>(new BoundedChannelOptions(MessageChannelCapacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false // Prevent blocking Redis thread
        });

        // Create object pools for reusing common objects
        _responsePool =
            new DefaultObjectPool<MarketStreamingResponse>(new DefaultMarketStreamingResponsePooledObjectPolicy());
        _processingThrottle = new SemaphoreSlim(MaxConcurrentProcessing, MaxConcurrentProcessing);

        // Get configuration values
        _groupName = configuration[ConfigurationKeys.SignalRHubGroupName] ??
                     throw new ArgumentNullException(nameof(configuration),
                         $"{ConfigurationKeys.SignalRHubGroupName} is not configured");

        _methodName = configuration[ConfigurationKeys.SignalRHubMethodName] ??
                      throw new ArgumentNullException(nameof(configuration),
                          $"{ConfigurationKeys.SignalRHubMethodName} is not configured");

        _channel = configuration[ConfigurationKeys.RedisChannel] ??
                   throw new ArgumentNullException(nameof(configuration),
                       $"{ConfigurationKeys.RedisChannel} is not configured");

        if (string.IsNullOrEmpty(_channel))
            throw new ArgumentException("Redis channel cannot be empty", nameof(configuration));

        var keyspace = configuration[ConfigurationKeys.RedisKeyspace] ?? string.Empty;
        if (!string.IsNullOrEmpty(keyspace))
            _channel = $"{keyspace}{_channel}";

        // Initialize timers for maintenance tasks
        _cleanupTimer = new Timer(CleanupOldConnections, null, ConnectionCleanupInterval, ConnectionCleanupInterval);

        _statsReportingTimer =
            new Timer(ReportPerformanceStats, null, _statsReportingInterval, _statsReportingInterval);

        _logger.LogInformation("StreamingMarketDataSubscriberGroupFilterTuned initialized with channel: {Channel}",
            _channel);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stoppingCts != null)
        {
            try
            {
                await StopAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during disposal");
            }

            _stoppingCts.Dispose();
            _stoppingCts = null;
        }

        await _cleanupTimer.DisposeAsync();
        await _statsReportingTimer.DisposeAsync();
        _processingThrottle.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("StreamingMarketDataSubscriberGroupFilterTuned starting");
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_stoppingCts.Token);
        _processingTask = StartMessageProcessingAsync(_stoppingCts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null && _processingTask == null) return;

        _logger.LogInformation("StreamingMarketDataSubscriberGroupFilterTuned stopping");

        try
        {
            if (_stoppingCts is { IsCancellationRequested: false })
                await _stoppingCts.CancelAsync();
        }
        finally
        {
            var completedTasks = new List<Task>();

            if (_executingTask != null)
                completedTasks.Add(_executingTask);

            if (_processingTask != null)
                completedTasks.Add(_processingTask);

            if (completedTasks.Count > 0)
                await Task.WhenAny(
                    Task.WhenAll(completedTasks),
                    Task.Delay(TimeSpan.FromSeconds(30), cancellationToken) // Add timeout
                ).ConfigureAwait(false);
        }
    }

    public async Task UpdateSubscriptionAsync(string connectionId, MarketStreamingRequest request)
    {
        if (string.IsNullOrEmpty(connectionId))
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));

        ArgumentNullException.ThrowIfNull(request);

        // Extract valid symbols from the request
        var symbols = request.Data?.Param?
            .Select(p => p.Symbol)
            .Where(s => !string.IsNullOrEmpty(s))
            .ToHashSet(StringComparer.Ordinal) ?? [];

        // Get existing symbols for this connection
        var oldSymbols = _symbolSubscriptions.GetOrAdd(connectionId,
            _ => new HashSet<string>(StringComparer.Ordinal));

        // Determine which symbols to add or remove
        var symbolsToRemove = oldSymbols.Except(symbols).ToList();
        var symbolsToAdd = symbols.Except(oldSymbols).ToList();

        if (symbolsToAdd.Count == 0 && symbolsToRemove.Count == 0)
        {
            // No changes needed
            _connectionLastActivity[connectionId] = DateTime.UtcNow;
            return;
        }

        // Process removals and additions in batches
        var tasks = new List<Task>();

        // Remove client from old groups
        foreach (var symbol in symbolsToRemove)
        {
            var groupId = $"{_groupName}_{symbol}";
            tasks.Add(_hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId));
            if (symbol != null)
            {
                oldSymbols.Remove(symbol);

                // Track active symbols for optimization
                UpdateActiveSymbols(symbol);
            }
        }

        // Add client to new groups
        foreach (var symbol in symbolsToAdd)
        {
            var groupId = $"{_groupName}_{symbol}";
            tasks.Add(_hubContext.Groups.AddToGroupAsync(connectionId, groupId));
            if (symbol != null)
            {
                oldSymbols.Add(symbol);

                // Mark symbol as active
                _activeSymbols.TryAdd(symbol, 1);
            }
        }

        // Update last activity timestamp
        _connectionLastActivity[connectionId] = DateTime.UtcNow;

        // Wait for all group operations to complete
        await Task.WhenAll(tasks);

        _logger.LogDebug("Updated subscription for client {ConnectionId}. Added: {Added}, Removed: {Removed}",
            connectionId, symbolsToAdd.Count, symbolsToRemove.Count);

        // Log current subscriptions for debugging
        _logger.LogDebug("Client {ConnectionId} now subscribed to symbols: {Symbols}",
            connectionId, string.Join(", ", oldSymbols));
    }

    public async Task RemoveSubscriptionAsync(string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));

        if (_symbolSubscriptions.TryRemove(connectionId, out var symbols))
        {
            var tasks = new List<Task>();

            // Remove client from all symbol groups
            foreach (var symbol in symbols)
            {
                var groupId = $"{_groupName}_{symbol}";
                tasks.Add(_hubContext.Groups.RemoveFromGroupAsync(connectionId, groupId));

                // Update active symbols tracking
                UpdateActiveSymbols(symbol);
            }

            await Task.WhenAll(tasks);
        }

        // Clean up connection tracking
        _connectionLastActivity.TryRemove(connectionId, out _);

        _logger.LogDebug("Removed subscription for client {ConnectionId}", connectionId);
    }

    public bool IsHealthy()
    {
        try
        {
            // Check connection
            var isRedisConnected = _redisConnection.IsConnected;
            var isExecutingTaskRunning = _executingTask?.IsCompleted == false;
            var isProcessingTaskRunning = _processingTask?.IsCompleted == false;
            var messageChannelCount = _messageChannel.Reader.Count;
            var isHealthy = isRedisConnected && isExecutingTaskRunning && isProcessingTaskRunning;

            // Log detailed health information periodically
            _logger.LogInformation(
                "Health check: Redis connected: {IsRedisConnected}, Executing task: {IsExecutingTaskRunning}, " +
                "Processing task: {IsProcessingTaskRunning}, Channel count: {MessageCount}, " +
                "Connection count: {ConnectionCount}, Active symbols: {SymbolCount}",
                isRedisConnected,
                isExecutingTaskRunning,
                isProcessingTaskRunning,
                messageChannelCount,
                _connectionLastActivity.Count,
                _activeSymbols.Count);

            return isHealthy;
        }
        catch
        {
            return false;
        }
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _isSubscribed = false;

        const int maxRetries = 2;
        const int maxMinutesSinceLastSubscription = 2;

        var retryCount = 0;
        var initialBackoff = TimeSpan.FromMilliseconds(500);
        var lastSuccessfulSubscription = DateTime.MinValue;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (ShouldForceRestart(lastSuccessfulSubscription, maxMinutesSinceLastSubscription))
                ForceRestart("Cannot subscribe for a prolonged period");

            try
            {
                var subscriber = _redisConnection.GetSubscriber();
                var connectionMultiplexer = subscriber.Multiplexer;

                await SubscribeToRedis(subscriber);

                retryCount = 0;
                _isSubscribed = true;
                lastSuccessfulSubscription = DateTime.UtcNow;
                _logger.LogInformation("Successfully subscribed to Redis channel: {Channel}", _channel);

                var monitoringTask = MonitorRedisConnectionAsync(connectionMultiplexer, stoppingToken);
                await Task.WhenAny(Task.Delay(Timeout.Infinite, stoppingToken), monitoringTask);

                if (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Redis subscription interrupted, will retry");
                    try
                    {
                        await subscriber.UnsubscribeAllAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error unsubscribing");
                    }
                }
            }
            catch (RedisTimeoutException ex)
            {
                _isSubscribed = false;
                _logger.LogError(ex, "Redis timeout. Attempt {RetryCount}", retryCount);
                await DelayWithBackoff(retryCount++, initialBackoff, stoppingToken);
            }
            catch (RedisConnectionException ex)
            {
                _isSubscribed = false;
                _logger.LogError(ex, "Redis connection error. Attempt {RetryCount}", retryCount);
                await DelayWithBackoff(retryCount++, initialBackoff, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _isSubscribed = false;
                _logger.LogError(ex, "Unexpected error. Attempt {RetryCount}", retryCount);
                await DelayWithBackoff(retryCount++, initialBackoff, stoppingToken);
            }

            if (retryCount >= maxRetries)
            {
                _logger.LogCritical("Max retries ({MaxRetries}) reached, cooling down", maxRetries);
                if (ShouldForceRestart(lastSuccessfulSubscription, maxMinutesSinceLastSubscription))
                    ForceRestart("Too many retries and prolonged downtime");

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                retryCount = 0;
            }
        }
    }

    private async Task SubscribeToRedis(ISubscriber subscriber)
    {
        var channel = new RedisChannel(_channel, RedisChannel.PatternMode.Literal);

        await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            await subscriber.SubscribeAsync(channel, async void (redisChannel, messageData) =>
            {
                try
                {
                    if (!messageData.HasValue)
                    {
                        _logger.LogDebug("Received empty Redis message");
                        return;
                    }

                    byte[]? compressedData = messageData;
                    if (compressedData == null || compressedData.Length == 0)
                    {
                        _logger.LogDebug("Empty or null compressed data");
                        return;
                    }

                    var symbol = TryExtractSymbolFromMessage(messageData);
                    if (!string.IsNullOrEmpty(symbol) && !_activeSymbols.ContainsKey(symbol))
                    {
                        _logger.LogDebug("No subscribers for symbol {Symbol}, skipping", symbol);
                        return;
                    }

                    _logger.LogDebug("Queued Redis message for symbol: {Symbol}", symbol);

                    var redisMessage = new RedisMessage
                    {
                        Channel = redisChannel.ToString(),
                        CompressedData = compressedData,
                        ReceivedTime = DateTime.UtcNow,
                        Symbol = symbol
                    };

                    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(5));
                    var canWrite = await _messageChannel.Writer.WaitToWriteAsync(cts.Token);
                    if (canWrite && _messageChannel.Writer.TryWrite(redisMessage)) return;

                    _logger.LogWarning("Message channel full. Capacity: {Capacity}, Count: {Count}",
                        MessageChannelCapacity, _messageChannel.Reader.Count);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing Redis message");
                }
            });
        });
    }

    private async Task MonitorRedisConnectionAsync(IConnectionMultiplexer connection, CancellationToken token)
    {
        while (!token.IsCancellationRequested && _isSubscribed)
        {
            try
            {
                if (!connection.IsConnected)
                {
                    _logger.LogWarning("Redis connection lost");
                    _isSubscribed = false;
                    break;
                }

                var ping = await connection.GetDatabase().PingAsync();
                if (ping > TimeSpan.FromSeconds(5))
                {
                    _logger.LogWarning("Ping latency high: {Latency}ms", ping.TotalMilliseconds);
                    _isSubscribed = false;
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis ping failed");
                _isSubscribed = false;
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(15), token);
        }
    }

    private bool ShouldForceRestart(DateTime lastSuccess, int maxMinutes)
    {
        return !_isSubscribed &&
               lastSuccess != DateTime.MinValue &&
               (DateTime.UtcNow - lastSuccess).TotalMinutes > maxMinutes;
    }

    private void ForceRestart(string reason)
    {
        _logger.LogCritical("{Reason}. Restarting container...", reason);

        // Delay to exit
        Thread.Sleep(1000);

        // Non-zero exit code
        Environment.Exit(1);
    }

    private async Task DelayWithBackoff(int retryAttempt, TimeSpan initialBackoff, CancellationToken token)
    {
        // Exponential backoff with jitter
        var backoffMs = (int)(initialBackoff.TotalMilliseconds * Math.Pow(2, retryAttempt));
        // Cap the maximum backoff to avoid extremely long delays
        backoffMs = Math.Min(backoffMs, 1000); // Max 1 second

        var jitter = new Random().Next(0, 100); // Add some randomness to prevent thundering herd

        _logger.LogDebug("Backing off for {BackoffMs}ms before retry attempt {RetryAttempt}",
            backoffMs + jitter, retryAttempt + 1);

        await Task.Delay(backoffMs + jitter, token);
    }

    private async Task StartMessageProcessingAsync(CancellationToken stoppingToken)
    {
        // Create multiple consumer tasks to process messages in parallel
        var consumerTasks = new List<Task>();
        for (var i = 0; i < MaxConcurrentProcessing; i++)
        {
            var processorId = i;
            consumerTasks.Add(Task.Run(() => ProcessMessagesAsync(processorId, stoppingToken), stoppingToken));
        }

        try
        {
            // Wait for all consumer tasks to complete
            await Task.WhenAll(consumerTasks);
        }
        catch (OperationCanceledException)
        {
            // Nothing to do
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message processing tasks");
        }
    }

    [SuppressMessage("SonarQube", "S3776")]
    private async Task ProcessMessagesAsync(int processorId, CancellationToken stoppingToken)
    {
        var pendingMessages = new List<RedisMessage>(BatchSize * 2);
        var processorName = $"Processor-{processorId}";

        _logger.LogInformation("{ProcessorName} started", processorName);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Reset or clear the pending messages list
                if (pendingMessages.Count > BatchSize * 4)
                    // If the list has grown too large, create a new one to avoid excessive memory usage
                    pendingMessages = new List<RedisMessage>(BatchSize * 2);
                else
                    pendingMessages.Clear();

                try
                {
                    // Batch read messages with timeout to ensure regular processing
                    var batchCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    batchCts.CancelAfter(_batchWindow);

                    try
                    {
                        // Fill the batch either until full or until timeout
                        while (pendingMessages.Count < BatchSize)
                            if (await _messageChannel.Reader.WaitToReadAsync(batchCts.Token))
                            {
                                if (_messageChannel.Reader.TryRead(out var message)) pendingMessages.Add(message);
                            }
                            else
                            {
                                // No more messages available
                                break;
                            }
                    }
                    catch (OperationCanceledException) when (batchCts.IsCancellationRequested &&
                                                             !stoppingToken.IsCancellationRequested)
                    {
                        // Batch timeout expired, proceed with processing whatever we have
                    }
                    finally
                    {
                        batchCts.Dispose();
                    }

                    // Process the batch if we have any messages
                    if (pendingMessages.Count > 0)
                    {
                        _logger.LogDebug("{ProcessorName}: Processing batch of {Count} messages",
                            processorName, pendingMessages.Count);
                        await ProcessMessageBatchAsync(pendingMessages, processorName);
                    }
                    else
                    {
                        // No messages to process, wait briefly to avoid CPU spinning
                        await Task.Delay(TimeSpan.FromMilliseconds(1), stoppingToken);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Normal shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{ProcessorName}: Error processing message batch", processorName);

                    // Short delay after error to prevent tight error loops
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                }
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{ProcessorName}: Fatal error in message processor", processorName);
        }
        finally
        {
            _logger.LogInformation("{ProcessorName} stopped", processorName);
        }
    }

    private async Task ProcessMessageBatchAsync(List<RedisMessage> messages, string processorName)
    {
        if (messages.Count == 0) return;

        await _processingThrottle.WaitAsync();
        try
        {
            var startTime = Stopwatch.GetTimestamp();
            var messagesBySymbol = new Dictionary<string, List<ParsedMessage>>(StringComparer.Ordinal);

            foreach (var message in messages)
            {
                if (message.CompressedData.Length == 0)
                    continue;

                var symbol = message.Symbol;

                if (string.IsNullOrEmpty(symbol))
                {
                    try
                    {
                        var jsonData = CompressionHelper.DecompressData(message.CompressedData);
                        if (string.IsNullOrEmpty(jsonData))
                            continue;

                        var marketStreamingResponse = JsonSerializer.Deserialize<MarketStreamingResponse>(
                            jsonData, _jsonOptions);

                        if (marketStreamingResponse?.Response.Data is { Count: > 0 })
                            foreach (var item in marketStreamingResponse.Response.Data)
                            {
                                symbol = item.Symbol;
                                if (string.IsNullOrEmpty(symbol) || !_activeSymbols.ContainsKey(symbol))
                                    continue;

                                if (!messagesBySymbol.TryGetValue(symbol, out var symbolMessages))
                                    messagesBySymbol[symbol] = symbolMessages = [];

                                symbolMessages.Add(new ParsedMessage
                                {
                                    JsonData = jsonData,
                                    ReceivedTime = message.ReceivedTime
                                });
                            }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to extract symbol from message");
                    }
                }
                else
                {
                    if (!_activeSymbols.ContainsKey(symbol))
                        continue;

                    try
                    {
                        var jsonData = CompressionHelper.DecompressData(message.CompressedData);
                        if (string.IsNullOrEmpty(jsonData))
                            continue;

                        if (!messagesBySymbol.TryGetValue(symbol, out var symbolMessages))
                            messagesBySymbol[symbol] = symbolMessages = [];

                        symbolMessages.Add(new ParsedMessage
                        {
                            JsonData = jsonData,
                            ReceivedTime = message.ReceivedTime
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Decompression failed for symbol {Symbol}", symbol);
                    }
                }
            }

            // Parallel sending, with thread-safe object pooling
            var sendTasks = messagesBySymbol.Select(async kvp =>
            {
                var symbol = kvp.Key;
                var groupName = $"{_groupName}_{symbol}";
                var symbolMessages = kvp.Value;
                var response = _responsePool.Get();

                try
                {
                    await SendSymbolUpdatesWithPooledResponseAsync(groupName, symbol, symbolMessages, response);
                }
                finally
                {
                    _responsePool.Return(response);
                }
            });

            await Task.WhenAll(sendTasks);

            var elapsedTime = Stopwatch.GetElapsedTime(startTime);
            if (elapsedTime > TimeSpan.FromMilliseconds(100))
                _logger.LogWarning(
                    "{ProcessorName}: Slow batch processing: {Elapsed}ms for {Count} messages ({SymbolCount} symbols)",
                    processorName, elapsedTime.TotalMilliseconds, messages.Count, messagesBySymbol.Count);
        }
        finally
        {
            _processingThrottle.Release();
        }
    }

    private async Task SendSymbolUpdatesWithPooledResponseAsync(
        string groupName,
        string symbol,
        List<ParsedMessage> messages,
        MarketStreamingResponse response)
    {
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            if (!_activeSymbols.ContainsKey(symbol))
            {
                _logger.LogDebug("No active subscribers for {Symbol}, skipping send", symbol);
                return;
            }

            var latestMessage = messages[^1];

            try
            {
                var marketStreamingResponse = JsonSerializer.Deserialize<MarketStreamingResponse>(
                    latestMessage.JsonData, _jsonOptions);

                if (marketStreamingResponse == null)
                {
                    _logger.LogWarning("Could not deserialize message for symbol {Symbol}", symbol);
                    return;
                }

                // Reset response before reuse
                response.Code = marketStreamingResponse.Code;
                response.Op = marketStreamingResponse.Op;
                response.Message = marketStreamingResponse.Message;
                response.SendingId = marketStreamingResponse.SendingId;
                response.SendingTime = marketStreamingResponse.SendingTime;
                response.ProcessingTime = marketStreamingResponse.ProcessingTime;
                response.CreationTime = marketStreamingResponse.CreationTime;
                response.ResponseTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff");
                response.SequenceNumber = marketStreamingResponse.SequenceNumber;
                response.MdEntryType = marketStreamingResponse.MdEntryType;
                response.Response.Data = [];

                if (marketStreamingResponse.Response.Data.Count > 0)
                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var item in marketStreamingResponse.Response.Data)
                        if (item.Symbol == symbol)
                            response.Response.Data.Add(item);

                var responseJson = JsonSerializer.Serialize(response, _jsonOptions);
                var compressedBytes = CompressionHelper.CompressString(responseJson);
                var messageSizeKb = compressedBytes.Length / 1024;
                _symbolMessageSizes.AddOrUpdate(symbol, messageSizeKb, (_, _) => messageSizeKb);

                if (messageSizeKb > 50)
                    _logger.LogWarning("Large message for symbol {Symbol}: {Size}KB", symbol, messageSizeKb);

                await _redisRetryPolicy.ExecuteAsync(async () =>
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                    await _hubContext.Clients.Group(groupName).SendAsync(_methodName, compressedBytes, cts.Token);
                });

                UpdateSymbolStats(symbol, Stopwatch.GetElapsedTime(startTime));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing or sending update for symbol {Symbol}", symbol);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in pooled response sender for {Symbol}", symbol);
        }
    }

    private string? TryExtractSymbolFromMessage(RedisValue message)
    {
        try
        {
            if (!message.HasValue)
                return null;

            // RedisValue can be implicitly converted to byte[] when it represents binary data
            byte[]? compressedData = message != RedisValue.Null ? message : [];
            if (compressedData == null || compressedData.Length == 0)
                return null;

            // Try to decompress and extract symbol
            string json;
            try
            {
                // Use the CompressionHelper to decompress the data
                json = CompressionHelper.DecompressData(compressedData);
                if (string.IsNullOrEmpty(json))
                    return null;
            }
            catch (Exception ex)
            {
                // If decompression fails, this might not be a valid message
                _logger.LogWarning(ex, "Decompression failed during symbol extraction");
                return null;
            }

            try
            {
                // Try full deserialization for more reliability
                var response = JsonSerializer.Deserialize<MarketStreamingResponse>(json, _jsonOptions);
                if (response is { Response.Data.Count: > 0 })
                    foreach (var item in response.Response.Data.Select(i => i.Symbol))
                        if (!string.IsNullOrEmpty(item))
                        {
                            _logger.LogDebug("Extracted symbol {Symbol} through full deserialization", item);
                            return item;
                        }
            }
            catch
            {
                // If full deserialization fails, try the partial approach as fallback
            }

            // Find symbol with minimal string parsing as fallback
            var symbolIndex = json.IndexOf("\"symbol\":", StringComparison.Ordinal);
            if (symbolIndex < 0) return null;

            symbolIndex += 9; // Length of "symbol":

            // Skip whitespace and quotes
            while (symbolIndex < json.Length && (json[symbolIndex] == ' ' || json[symbolIndex] == '"')) symbolIndex++;

            // Extract symbol value
            var symbolEndIndex = json.IndexOfAny(['"', ','], symbolIndex);
            if (symbolEndIndex < 0) return null;

            var symbol = json.Substring(symbolIndex, symbolEndIndex - symbolIndex).Trim('"');
            _logger.LogDebug("Extracted symbol through string parsing: {Symbol}", symbol);
            return symbol;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during symbol extraction");
            return null;
        }
    }

    private void UpdateSymbolStats(string symbol, TimeSpan processingTime)
    {
        _symbolStats.AddOrUpdate(
            symbol,
            _ => new SymbolStats
            {
                MessageCount = 1,
                TotalProcessingTimeMs = (long)processingTime.TotalMilliseconds,
                LastUpdated = DateTime.UtcNow
            },
            (_, stats) =>
            {
                stats.MessageCount++;
                stats.TotalProcessingTimeMs += (long)processingTime.TotalMilliseconds;
                stats.LastUpdated = DateTime.UtcNow;
                return stats;
            });
    }

    private void UpdateActiveSymbols(string symbol)
    {
        // Count how many connections have this symbol
        var symbolSubscriberCount = _symbolSubscriptions.Values
            .Count(subs => subs.Contains(symbol));

        if (symbolSubscriberCount > 0)
        {
            // Symbol has subscribers
            _activeSymbols.TryAdd(symbol, 1);
            _logger.LogDebug("Symbol {Symbol} has {Count} subscribers", symbol, symbolSubscriberCount);
        }
        else
        {
            // No more subscribers for this symbol
            _activeSymbols.TryRemove(symbol, out _);
            _logger.LogDebug("Symbol {Symbol} has no subscribers, removing from active symbols", symbol);
        }
    }

    private void CleanupOldConnections(object? state)
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.Subtract(ConnectionExpirationTime);
            var keysToRemove = _connectionLastActivity
                .Where(kvp => kvp.Value < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            if (keysToRemove.Count == 0) return;

            _logger.LogInformation("Starting connection cleanup. Found {Count} inactive connections",
                keysToRemove.Count);

            // Process in smaller batches
            const int batchSize = 100;
            for (var i = 0; i < keysToRemove.Count; i += batchSize)
            {
                var batch = keysToRemove.Skip(i).Take(batchSize).ToList();
                var tasks = new List<Task>();

                foreach (var connectionId in batch) tasks.Add(RemoveSubscriptionAsync(connectionId));

                Task.WhenAll(tasks).Wait(TimeSpan.FromSeconds(30));
            }

            _logger.LogInformation("Connection cleanup completed. Removed {Count} connections", keysToRemove.Count);

            // Refresh active symbols list
            RefreshActiveSymbols();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during connection cleanup");
        }
    }

    private void RefreshActiveSymbols()
    {
        try
        {
            // Rebuild active symbols mapping
            var allActiveSymbols = new HashSet<string>(StringComparer.Ordinal);

            foreach (var symbolSet in _symbolSubscriptions.Values)
                allActiveSymbols.UnionWith(symbolSet);

            // Update the active symbols dictionary
            _activeSymbols.Clear();
            foreach (var symbol in allActiveSymbols)
                _activeSymbols.TryAdd(symbol, 1);

            _logger.LogInformation("Active symbols refreshed. Current count: {Count}", _activeSymbols.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing active symbols");
        }
    }

    [SuppressMessage("SonarQube", "S1215")]
    private void ReportPerformanceStats(object? state)
    {
        try
        {
            var totalMessages = 0L;
            var totalTime = 0L;
            var slowSymbols = new List<(string Symbol, double AvgTime, long Count)>();

            foreach (var (symbol, stats) in _symbolStats)
            {
                var avgTime = stats.MessageCount > 0
                    ? (double)stats.TotalProcessingTimeMs / stats.MessageCount
                    : 0;

                totalMessages += stats.MessageCount;
                totalTime += stats.TotalProcessingTimeMs;

                // Track slow symbols for optimization
                if (avgTime > 5 && stats.MessageCount > 100)
                    slowSymbols.Add((symbol, avgTime, stats.MessageCount));
            }

            // Calculate overall average
            var overallAvg = totalMessages > 0 ? (double)totalTime / totalMessages : 0;

            _logger.LogInformation(
                "Performance stats: Total messages: {TotalMessages}, Avg processing time: {AvgTime:F2}ms",
                totalMessages, overallAvg);

            var currentMemory = GC.GetTotalMemory(false) / (1024 * 1024); // MB
            _logger.LogInformation("Current managed memory usage: {MemoryMB}MB", currentMemory);

            var channelCapacityUsed = (double)_messageChannel.Reader.Count / MessageChannelCapacity * 100;
            if (channelCapacityUsed > 50)
                _logger.LogWarning("High channel capacity usage: {CapacityUsed:F1}%", channelCapacityUsed);

            // Report slow symbols
            if (slowSymbols.Count > 0)
            {
                slowSymbols.Sort((a, b) => b.AvgTime.CompareTo(a.AvgTime));
                var top5Slow = slowSymbols.Take(5)
                    .Select(s => $"{s.Symbol}:{s.AvgTime:F2}ms:{s.Count}msg")
                    .ToArray();

                _logger.LogWarning(
                    "Slow symbols detected: {SlowSymbols}",
                    string.Join(", ", top5Slow));
            }

            var gcCooldown = TimeSpan.FromMinutes(15);
            var isLowTraffic = _messageChannel.Reader.Count < 1000;

            if (currentMemory > MemoryThresholdMb &&
                DateTime.UtcNow - _lastGcCollectTime > gcCooldown && isLowTraffic)
            {
                _logger.LogInformation(
                    "Triggering GC.Collect due to high memory usage ({MemoryMB}MB) during low traffic period",
                    currentMemory);

                GC.Collect(1, GCCollectionMode.Optimized, false, false);

                if (currentMemory > MemoryThresholdMb * 1.5)
                {
                    _logger.LogInformation("Memory still high, performing full collection");
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, false, false);
                }

                _lastGcCollectTime = DateTime.UtcNow;
            }

            // Clean up old stats to prevent memory growth
            var cutoffTime = DateTime.UtcNow.AddHours(-2);
            var oldStats = _symbolStats
                .Where(kvp => kvp.Value.LastUpdated < cutoffTime)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var symbol in oldStats)
                _symbolStats.TryRemove(symbol, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting performance stats");
        }
    }

    // Data container classes
    private sealed class RedisMessage
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable PropertyCanBeMadeInitOnly.Local
        public string Channel { get; set; } = string.Empty;
        public byte[] CompressedData { get; set; } = [];
        public DateTime ReceivedTime { get; set; }
        public string? Symbol { get; set; } // Store pre-extracted symbol
    }

    private sealed class ParsedMessage
    {
        public string JsonData { get; set; } = string.Empty;
        public DateTime ReceivedTime { get; set; }
    }

    private sealed class SymbolStats
    {
        public long MessageCount { get; set; }
        public long TotalProcessingTimeMs { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    private sealed class DefaultMarketStreamingResponsePooledObjectPolicy : PooledObjectPolicy<MarketStreamingResponse>
    {
        public override MarketStreamingResponse Create()
        {
            return new MarketStreamingResponse
            {
                Response = new StreamingResponse { Data = [] }
            };
        }

        public override bool Return(MarketStreamingResponse obj)
        {
            // Reset the response to prevent data leaks
            obj.Code = string.Empty;
            obj.Op = string.Empty;
            obj.Message = string.Empty;
            obj.SendingId = null;
            obj.SendingTime = null;
            obj.ProcessingTime = null;
            obj.CreationTime = null;
            obj.ResponseTime = null;
            obj.SequenceNumber = 0;
            obj.Response.Data.Clear();

            return true;
        }
    }
}