using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Services;

/// <summary>
///     High-performance service that handles message sampling for Kafka messages by compositeKey.
///     Implements IHostedService for proper lifecycle management and memory cleanup.
/// </summary>
public class MessageSamplerService : IHostedService, IDisposable
{
    private readonly TimeSpan _cleanupInterval;
    private readonly ConcurrentDictionary<string, SamplingState> _compositeKeySamplingStates = new();
    private readonly TimeSpan _inactiveThreshold;
    private readonly bool _isEnabled;
    private readonly ILogger<MessageSamplerService> _logger;
    private readonly int _samplingCount;
    private readonly long _samplingTimeWindowTicks; // Store as ticks for better performance
    private Timer? _cleanupTimer;
    private bool _disposed;

    /// <summary>
    ///     Initializes a new instance of the MessageSamplerService class
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="configuration">Application configuration</param>
    public MessageSamplerService(
        ILogger<MessageSamplerService> logger,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Core sampling settings
        _isEnabled = configuration.GetValue("MESSAGE_SAMPLING:ENABLED", false);
        _samplingCount = configuration.GetValue("MESSAGE_SAMPLING:SAMPLING_COUNT", 8);
        var timeWindowMs = configuration.GetValue("MESSAGE_SAMPLING:TIME_WINDOW_MS", 20);
        _samplingTimeWindowTicks = timeWindowMs * Stopwatch.Frequency / 1000;

        // Memory management settings
        _cleanupInterval = TimeSpan.FromMinutes(
            configuration.GetValue("MESSAGE_SAMPLING:CLEANUP_INTERVAL_MINUTES", 30));
        _inactiveThreshold = TimeSpan.FromMinutes(
            configuration.GetValue("MESSAGE_SAMPLING:INACTIVE_THRESHOLD_MINUTES", 10));

        _logger.LogInformation(
            "Message sampling service initialized. Enabled: {Enabled}, SamplingCount: {SamplingCount}, TimeWindowMs: {TimeWindowMs}",
            _isEnabled, _samplingCount, timeWindowMs);
    }

    /// <summary>
    ///     Dispose resources
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Starts the background cleanup task
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("MessageSamplerService starting");

        // Start timer for cleanup of inactive compositeKeys to prevent memory leaks
        _cleanupTimer = new Timer(CleanupInactiveCompositeKeys, null, _cleanupInterval, _cleanupInterval);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Stops the background cleanup task
    /// </summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("MessageSamplerService stopping");

        _cleanupTimer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Dispose pattern implementation
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing) _cleanupTimer?.Dispose();

        _disposed = true;
    }

    /// <summary>
    ///     Checks if a message should be published based on sampling rules.
    ///     This is optimized for performance - avoid creating the full response object
    ///     before checking if it will be published.
    /// </summary>
    /// <param name="compositeKey">The compositeKey for which the message is being sampled</param>
    /// <param name="response">Optional response object, will be stored if provided</param>
    /// <returns>True if the message should be published, false otherwise</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ShouldPublishMessage(string compositeKey, MarketStreamingResponse? response = null)
    {
        // Fast path - if disabled, always publish
        if (!_isEnabled) return true;

        // Safety check - always publish for empty compositeKeys
        if (string.IsNullOrEmpty(compositeKey)) return true;

        // Store response if provided
        if (response != null) StoreLatestResponse(compositeKey, response);

        // Reference caching for lock-free path when possible
        var state = GetOrCreateState(compositeKey);

        // Fast check, lock if needed
        var currentTime = Stopwatch.GetTimestamp();
        var elapsedTicks = currentTime - state.LastPublishTime;
        if (elapsedTicks >= _samplingTimeWindowTicks)
            // Reset counter and update timestamp using lock
            lock (state.Lock)
            {
                state.MessageCount = 0;
                state.LastPublishTime = currentTime;
                return true;
            }

        // Fast path: avoid lock acquisition when we're not near the threshold
        var currentCount = Interlocked.Increment(ref state.MessageCount);
        if (currentCount < _samplingCount * 0.7) // Only use lock when we're close to threshold
            return false;

        // Slow path: use lock when we're close to a decision point
        lock (state.Lock)
        {
            // Re-check both conditions inside lock
            currentTime = Stopwatch.GetTimestamp();
            elapsedTicks = currentTime - state.LastPublishTime;

            // Check if we've reached the sampling count or time window
            if (currentCount >= _samplingCount || elapsedTicks >= _samplingTimeWindowTicks)
            {
                // Reset counter and update timestamp
                state.MessageCount = 0;
                state.LastPublishTime = currentTime;
                return true;
            }

            return false;
        }
    }

    /// <summary>
    ///     Store latest response for a compositeKey without checking publication decision
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void StoreLatestResponse(string compositeKey, MarketStreamingResponse response)
    {
        if (!_isEnabled || string.IsNullOrEmpty(compositeKey)) return;

        var state = GetOrCreateState(compositeKey);
        state.LatestResponse = response;
    }

    /// <summary>
    ///     Gets the latest response for a compositeKey
    /// </summary>
    /// <param name="compositeKey">The compositeKey</param>
    /// <returns>The latest market streaming response for the compositeKey, or null if none exists</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MarketStreamingResponse? GetLatestResponse(string compositeKey)
    {
        if (string.IsNullOrEmpty(compositeKey) || !_compositeKeySamplingStates.TryGetValue(compositeKey, out var state))
            return null;

        lock (state.Lock)
        {
            return state.LatestResponse;
        }
    }

    /// <summary>
    ///     Gets or creates sampling state for a compositeKey
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SamplingState GetOrCreateState(string compositeKey)
    {
        // Try to get existing state first - fast path or
        // Fallback to creation - slow path
        return _compositeKeySamplingStates.TryGetValue(compositeKey, out var state)
            ? state
            : _compositeKeySamplingStates.GetOrAdd(compositeKey, _ => new SamplingState());
    }

    /// <summary>
    ///     Timer callback to clean up inactive compositeKeys and prevent memory leaks
    /// </summary>
    private void CleanupInactiveCompositeKeys(object? state)
    {
        try
        {
            var currentTime = Stopwatch.GetTimestamp();
            var inactiveThresholdTicks = (long)(_inactiveThreshold.TotalSeconds * Stopwatch.Frequency);
            var compositeKeysToRemove = new List<string>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            // First pass: identify inactive compositeKeys
            foreach (var item in _compositeKeySamplingStates)
                if (currentTime - item.Value.LastActivityTime > inactiveThresholdTicks)
                    compositeKeysToRemove.Add(item.Key);

            // Second pass: remove inactive compositeKeys
            var removedCount = 0;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var compositeKey in compositeKeysToRemove)
                if (_compositeKeySamplingStates.TryRemove(compositeKey, out _))
                    removedCount++;

            if (removedCount > 0)
                _logger.LogInformation("Cleaned up {Count} inactive compositeKeys from sampler", removedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during inactive compositeKeys cleanup");
        }
    }

    /// <summary>
    ///     Represents the sampling state for a compositeKey
    /// </summary>
    private sealed class SamplingState
    {
        public readonly long LastActivityTime = Stopwatch.GetTimestamp();
        public long LastPublishTime = Stopwatch.GetTimestamp();
        public MarketStreamingResponse? LatestResponse;
        public int MessageCount;
        public object Lock { get; } = new();
    }
}