using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.ObjectPool;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Domain.Models.Request;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using Pi.SetMarketDataWSS.Infrastructure.Helpers;
using Pi.SetMarketDataWSS.SignalRHub.Hubs;
using Pi.SetMarketDataWSS.SignalRHub.Interfaces;
using Polly;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Pi.SetMarketDataWSS.SignalRHub.Services;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable PropertyCanBeMadeInitOnly.Local
public sealed class StreamingMarketDataSubscriberGroupFilter : IStreamingMarketDataSubscriberGroupFilter,
    IAsyncDisposable
{
    private const int MaxConcurrentProcessing = 8;
    private const int BatchSize = 10;
    private static readonly TimeSpan ConnectionCleanupInterval = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan ConnectionExpirationTime = TimeSpan.FromHours(1);
    private readonly TimeSpan _batchWindow = TimeSpan.FromMilliseconds(15);
    private readonly string _channel;
    private readonly IAsyncPolicy _circuitBreakerPolicy;
    private readonly Timer _cleanupTimer;
    private readonly ConcurrentDictionary<string, DateTime> _connectionLastActivity;
    private readonly ConcurrentDictionary<string, MarketStreamingRequest> _connectionRequests;
    private readonly string _groupName;
    private readonly IHubContext<StreamingHubGroupFilter> _hubContext;
    private readonly ILogger<StreamingMarketDataSubscriberGroupFilter> _logger;
    private readonly Channel<RedisMessageWrapper> _messageChannel;
    private readonly string _methodName;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly SemaphoreSlim _processingThrottle;
    private readonly IConnectionMultiplexer _redisConnection;
    private readonly ObjectPool<MarketStreamingResponse> _responsePool;
    private readonly Dictionary<string, SymbolStats> _symbolPerformanceStats = new();
    private readonly ConcurrentDictionary<string, HashSet<string>> _symbolSubscriptions;
    private Task? _executingTask;
    private Task? _processingTask;
    private CancellationTokenSource? _stoppingCts;

    public StreamingMarketDataSubscriberGroupFilter(
        IConnectionMultiplexer redisConnection,
        IHubContext<StreamingHubGroupFilter> hubContext,
        IConfiguration configuration,
        ILogger<StreamingMarketDataSubscriberGroupFilter> logger,
        IAsyncPolicy circuitBreakerPolicy)
    {
        _redisConnection = redisConnection ?? throw new ArgumentNullException(nameof(redisConnection));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _circuitBreakerPolicy = circuitBreakerPolicy ?? throw new ArgumentNullException(nameof(circuitBreakerPolicy));
        _connectionRequests = new ConcurrentDictionary<string, MarketStreamingRequest>();
        _symbolSubscriptions = new ConcurrentDictionary<string, HashSet<string>>();
        _connectionLastActivity = new ConcurrentDictionary<string, DateTime>();
        _responsePool =
            new DefaultObjectPool<MarketStreamingResponse>(new DefaultMarketStreamingResponsePooledObjectPolicy());
        _processingThrottle = new SemaphoreSlim(MaxConcurrentProcessing, MaxConcurrentProcessing);
        _messageChannel = Channel.CreateBounded<RedisMessageWrapper>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = false,
            SingleWriter = false
        });

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

        // Setup cleanup timer
        _cleanupTimer = new Timer(CleanupOldConnections, null, ConnectionCleanupInterval, ConnectionCleanupInterval);
    }

    public async ValueTask DisposeAsync()
    {
        if (_stoppingCts != null)
            try
            {
                await StopAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during StreamingMarketDataSubscriberGroupFilter disposal");
            }
            finally
            {
                _stoppingCts.Dispose();
                _stoppingCts = null;
                await _cleanupTimer.DisposeAsync();
                _processingThrottle.Dispose();
            }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("StreamingMarketDataSubscriberGroupFilter is starting.");
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _executingTask = ExecuteAsync(_stoppingCts.Token);
        _processingTask = StartMessageProcessingAsync(_stoppingCts.Token);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null && _processingTask == null) return;

        _logger.LogDebug("StreamingMarketDataSubscriberGroupFilter is stopping.");

        try
        {
            if (_stoppingCts is { IsCancellationRequested: false }) await _stoppingCts.CancelAsync();
        }
        finally
        {
            var completedTasks = new List<Task>();

            if (_executingTask != null)
                completedTasks.Add(_executingTask);

            if (_processingTask != null)
                completedTasks.Add(_processingTask);

            await Task.WhenAny(Task.WhenAll(completedTasks), Task.Delay(Timeout.Infinite, cancellationToken))
                .ConfigureAwait(false);
        }
    }

    public async Task UpdateSubscriptionAsync(string connectionId, MarketStreamingRequest request)
    {
        if (string.IsNullOrEmpty(connectionId))
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));

        ArgumentNullException.ThrowIfNull(request);

        var symbols = request.Data?.Param?.Select(p => p.Symbol).Where(s => !string.IsNullOrEmpty(s)).ToHashSet() ?? [];
        var oldSymbols = _symbolSubscriptions.GetOrAdd(connectionId, []);
        var symbolsToRemove = oldSymbols.Except(symbols).ToList();
        var symbolsToAdd = symbols.Except(oldSymbols).ToList();

        var tasks = new List<Task>();

        foreach (var symbol in symbolsToRemove)
        {
            tasks.Add(_hubContext.Groups.RemoveFromGroupAsync(connectionId, $"{_groupName}_{symbol}"));
            if (symbol != null) oldSymbols.Remove(symbol);
        }

        foreach (var symbol in symbolsToAdd)
        {
            tasks.Add(_hubContext.Groups.AddToGroupAsync(connectionId, $"{_groupName}_{symbol}"));
            if (symbol != null) oldSymbols.Add(symbol);
        }

        await Task.WhenAll(tasks);

        _connectionRequests[connectionId] = request;
        _connectionLastActivity[connectionId] = DateTime.UtcNow;

        _logger.LogDebug("Updated subscription for client {ConnectionId}. Symbols: {Symbols}", connectionId,
            string.Join(", ", symbols));
    }

    public async Task RemoveSubscriptionAsync(string connectionId)
    {
        if (string.IsNullOrEmpty(connectionId))
            throw new ArgumentException("Connection ID cannot be null or empty", nameof(connectionId));

        if (_symbolSubscriptions.TryRemove(connectionId, out var symbols))
        {
            // Create a copy of the symbols to avoid modifying the collection during iteration
            var symbolsCopy = symbols.ToArray();
            var tasks = new List<Task>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var symbol in symbolsCopy)
                tasks.Add(_hubContext.Groups.RemoveFromGroupAsync(connectionId, $"{_groupName}_{symbol}"));

            await Task.WhenAll(tasks);
        }

        _connectionRequests.TryRemove(connectionId, out _);
        _connectionLastActivity.TryRemove(connectionId, out _);

        _logger.LogDebug("Removed subscription for client {ConnectionId}", connectionId);
    }

    public bool IsHealthy()
    {
        var isRedisConnected = _redisConnection.IsConnected;
        var isExecutingTaskRunning = _executingTask?.IsCompleted == false;
        var isProcessingTaskRunning = _processingTask?.IsCompleted == false;

        _logger.LogInformation(
            "Health check: Redis connected: {IsRedisConnected}, Executing task running: {IsExecutingTaskRunning}, Processing task running: {IsProcessingTaskRunning}",
            isRedisConnected,
            isExecutingTaskRunning,
            isProcessingTaskRunning);

        return isRedisConnected && isExecutingTaskRunning && isProcessingTaskRunning;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redisConnection.GetSubscriber();
        var subPatternChannel = new RedisChannel(_channel, RedisChannel.PatternMode.Literal);

        _logger.LogDebug("Subscribing to Redis channel: {Channel}", _channel);

        await _circuitBreakerPolicy.ExecuteAsync(async () =>
        {
            await subscriber.SubscribeAsync(subPatternChannel, (channel, message) =>
            {
                // Immediately enqueue the message for processing instead of processing it here
                var wrapper = new RedisMessageWrapper
                {
                    Channel = channel,
                    Message = message,
                    ReceivedTime = DateTime.UtcNow
                };

                // Try to add to the channel, if full, log and continue
                if (!_messageChannel.Writer.TryWrite(wrapper))
                    _logger.LogWarning("Message channel is full. Dropping message.");
            });
        });

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "StreamingMarketDataSubscriberGroupFilter is stopping due to cancellation");
        }
    }

    private async Task StartMessageProcessingAsync(CancellationToken stoppingToken)
    {
        // Create multiple consumer tasks to process messages in parallel
        var consumerTasks = new List<Task>();
        for (var i = 0; i < MaxConcurrentProcessing; i++) consumerTasks.Add(ProcessMessagesAsync(stoppingToken));

        try
        {
            // Wait for all consumer tasks to complete
            await Task.WhenAll(consumerTasks);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Message processing is stopping due to cancellation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message processing task");
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken stoppingToken)
    {
        var pendingMessages = new List<RedisMessageWrapper>();
        var lastBatchTime = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                // Read a message from the channel
                var message = await _messageChannel.Reader.ReadAsync(stoppingToken);
                pendingMessages.Add(message);

                // Check if we should process this batch (enough messages or time window elapsed)
                var shouldProcessBatch = pendingMessages.Count >= BatchSize ||
                                         DateTime.UtcNow - lastBatchTime > _batchWindow;

                if (shouldProcessBatch)
                {
                    // Process the batch
                    await ProcessMessageBatchAsync(pendingMessages);
                    pendingMessages.Clear();
                    lastBatchTime = DateTime.UtcNow;
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message batch");
                // Process any pending messages before continuing
                if (pendingMessages.Count > 0)
                    try
                    {
                        await ProcessMessageBatchAsync(pendingMessages);
                    }
                    catch (Exception batchEx)
                    {
                        _logger.LogError(batchEx, "Error processing pending messages after exception");
                    }
                    finally
                    {
                        pendingMessages.Clear();
                        lastBatchTime = DateTime.UtcNow;
                    }
            }
    }

    private async Task ProcessMessageBatchAsync(List<RedisMessageWrapper> messages)
    {
        await _processingThrottle.WaitAsync();
        try
        {
            // Group messages by symbol for more efficient processing
            var deserializedMessages = new List<(MarketStreamingResponse response, DateTime receivedTime)>();

            foreach (var messageWrapper in messages)
                try
                {
                    var compressedBytes = messageWrapper.Message;
                    var decompressData = CompressionHelper.DecompressData(compressedBytes);
                    var marketStreamingResponse =
                        JsonSerializer.Deserialize<MarketStreamingResponse>(decompressData, _options);

                    if (marketStreamingResponse?.Response?.Data != null)
                        deserializedMessages.Add((marketStreamingResponse, messageWrapper.ReceivedTime));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing message");
                }

            if (deserializedMessages.Count == 0)
                return;

            // Group by symbol for efficient processing
            var dataBySymbol =
                new Dictionary<string,
                    List<(object data, DateTime receivedTime, MarketStreamingResponse originalResponse)>>();

            foreach (var (response, receivedTime) in deserializedMessages)
            {
                if (response.Response?.Data == null)
                    continue;

                foreach (var data in response.Response.Data)
                {
                    var symbol = data.Symbol;
                    if (string.IsNullOrEmpty(symbol))
                        continue;

                    if (!dataBySymbol.TryGetValue(symbol, out var list))
                    {
                        list = new List<(object, DateTime, MarketStreamingResponse)>();
                        dataBySymbol[symbol] = list;
                    }

                    list.Add((data, receivedTime, response));
                }
            }

            // Process each symbol's data
            var allTasks = new List<Task>();

            foreach (var (symbol, dataList) in dataBySymbol)
            {
                var groupName = $"{_groupName}_{symbol}";
                allTasks.Add(SendSymbolUpdatesAsync(groupName, dataList));
            }

            if (allTasks.Count > 0) await Task.WhenAll(allTasks);
        }
        finally
        {
            _processingThrottle.Release();
        }
    }

    private async Task SendSymbolUpdatesAsync(string groupName,
        List<(object data, DateTime receivedTime, MarketStreamingResponse originalResponse)> updates)
    {
        // Measure performance
        var startTime = Stopwatch.GetTimestamp();
        var symbol = groupName[(_groupName.Length + 1)..]; // Extract symbol from group name

        try
        {
            // Prepare the response object from the pool
            var response = _responsePool.Get();
            try
            {
                // Use the most recent update as the base for common properties
                var (_, receivedTime, originalResponse) = updates[^1];

                // Set common properties
                response.Code = originalResponse.Code;
                response.Op = originalResponse.Op;
                response.Message = originalResponse.Message;
                response.SendingId = originalResponse.SendingId;
                response.SendingTime = originalResponse.SendingTime;
                response.ProcessingTime = originalResponse.ProcessingTime;
                response.CreationTime = originalResponse.CreationTime;
                response.ResponseTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff");
                response.SequenceNumber = originalResponse.SequenceNumber;

                // Collect all data items (take only the most recent one if multiple exist)
                // For stock market data, we typically only need the latest update per symbol
                var lastData = updates[^1].data;
                if (lastData is StreamingBody streamingBody)
                {
                    response.Response = new StreamingResponse
                    {
                        Data = [streamingBody]
                    };
                }
                else
                {
                    _logger.LogWarning("Invalid data type for symbol {Symbol}. Expected StreamingBody but got {Type}.",
                        symbol, lastData?.GetType().Name ?? "null");

                    response.Response = new StreamingResponse
                    {
                        Data = []
                    };
                }

                // Send the update to the clients
                var responseStr = JsonSerializer.Serialize(response, _options);
                var responseBytes = CompressionHelper.CompressString(responseStr);

                await _hubContext.Clients.Group(groupName).SendAsync(_methodName, responseBytes);

                // Calculate latency for monitoring
                var latency = DateTime.UtcNow - receivedTime;

                // Log latency metrics for high-latency messages
                if (latency > TimeSpan.FromMilliseconds(100))
                    _logger.LogWarning("High latency ({Latency}ms) for symbol {Symbol}",
                        latency.TotalMilliseconds, symbol);
            }
            finally
            {
                _responsePool.Return(response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending updates for symbol {Symbol}", symbol);
        }

        // Record performance statistics
        var elapsed = Stopwatch.GetElapsedTime(startTime);
        lock (_symbolPerformanceStats)
        {
            if (!_symbolPerformanceStats.TryGetValue(symbol, out var stats))
            {
                stats = new SymbolStats();
                _symbolPerformanceStats[symbol] = stats;
            }

            stats.MessageCount++;
            stats.TotalProcessingTimeMs += (long)elapsed.TotalMilliseconds;
            stats.LastUpdated = DateTime.UtcNow;

            // Log slow-processing symbols for optimization
            if (stats.MessageCount % 1000 == 0)
            {
                var avgProcessingTime = stats.TotalProcessingTimeMs / stats.MessageCount;
                if (avgProcessingTime > 5) // More than 5ms average
                    _logger.LogWarning("Symbol {Symbol} has high average processing time: {AvgTime}ms",
                        symbol, avgProcessingTime);
            }
        }
    }

    private void CleanupOldConnections(object? state)
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.Subtract(ConnectionExpirationTime);
            var keysToRemove = new List<string>();

            // Identify old connections
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var kvp in _connectionLastActivity)
                if (kvp.Value < cutoffTime)
                    keysToRemove.Add(kvp.Key);

            // Batch removal to minimize impact
            const int batchSize = 50;
            for (var i = 0; i < keysToRemove.Count; i += batchSize)
            {
                var batch = keysToRemove.Skip(i).Take(batchSize).ToList();
                foreach (var key in batch)
                {
                    _connectionLastActivity.TryRemove(key, out _);
                    _connectionRequests.TryRemove(key, out _);

                    if (_symbolSubscriptions.TryRemove(key, out _))
                        _logger.LogInformation("Cleaned up inactive connection: {ConnectionId}", key);
                }

                // Small delay between batches to reduce impact
                if (i + batchSize < keysToRemove.Count)
                    Thread.Sleep(1);
            }

            // Log cleanup summary
            if (keysToRemove.Count > 0)
                _logger.LogInformation("Connection cleanup completed. Removed {Count} inactive connections",
                    keysToRemove.Count);

            // Also clean up symbol stats that haven't been updated recently
            CleanupSymbolStats();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during connection cleanup");
        }
    }

    private void CleanupSymbolStats()
    {
        try
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-1);
            var symbolsToRemove = new List<string>();

            lock (_symbolPerformanceStats)
            {
                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var kvp in _symbolPerformanceStats)
                    if (kvp.Value.LastUpdated < cutoffTime)
                        symbolsToRemove.Add(kvp.Key);

                foreach (var symbol in symbolsToRemove) _symbolPerformanceStats.Remove(symbol);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up symbol stats");
        }
    }

    private sealed class RedisMessageWrapper
    {
        public RedisChannel Channel { get; set; }
        public RedisValue Message { get; set; }
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
                Response = new StreamingResponse()
            };
        }

        public override bool Return(MarketStreamingResponse obj)
        {
            // Reset all properties to default values
            obj.Code = string.Empty;
            obj.Op = string.Empty;
            obj.Message = string.Empty;
            obj.SendingId = null;
            obj.SendingTime = null;
            obj.ProcessingTime = null;
            obj.CreationTime = null;
            obj.ResponseTime = null;
            obj.SequenceNumber = 0;
            obj.Response.Data = [];

            return true;
        }
    }
}