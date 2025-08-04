using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Pi.GlobalMarketData.Application.Constants;
using Pi.GlobalMarketData.Application.Helpers;
using Pi.GlobalMarketData.Application.Services.FixMapper;
using Pi.GlobalMarketData.DataProcessingService.Services;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Fix;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Redis;
using Pi.GlobalMarketData.Infrastructure.Interfaces.TimescaleEf;

namespace Pi.GlobalMarketData.DataProcessingService.Handlers;

/// <summary>
///     Handles messages from Kafka and processes realtime market data
/// </summary>
public sealed class KafkaMessageHandler : IKafkaMessageV2Handler<Message<string, string>>, IDisposable
{
    private const int MaxBoundedCapacity = 100000;
    private const int MaxPostAttempts = 50;
    private const int PostRetryDelayMs = 100;
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IMarketScheduleDataService _marketScheduleService;

    // TPL Dataflow components
    private readonly
        ActionBlock<(Entry entry, string symbol, string venue, TaskCompletionSource<bool> completionSource)>
        _realtimeMarketDataBlock;

    private readonly IRedisV2Publisher _redisPublisher;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMongoService<WhiteList> _whiteListService;
    private long _totalPostAttempts;
    private long _totalPostFailures;

    // Metrics
    private long _totalProcessingFailures;
    private long _totalProcessingSuccesses;

    /// <summary>
    ///     Initializes a new instance of the <see cref="KafkaMessageHandler" /> class
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="whiteListService">White list service</param>
    /// <param name="redisPublisher">Redis publisher</param>
    /// <param name="marketScheduleService">Market schedule service</param>
    /// <param name="scopeFactory">Service scope factory</param>
    public KafkaMessageHandler(
        ILogger<KafkaMessageHandler> logger,
        IMongoService<WhiteList> whiteListService,
        IRedisV2Publisher redisPublisher,
        IMarketScheduleDataService marketScheduleService,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _whiteListService = whiteListService ?? throw new ArgumentNullException(nameof(whiteListService));
        _redisPublisher = redisPublisher ?? throw new ArgumentNullException(nameof(redisPublisher));
        _marketScheduleService =
            marketScheduleService ?? throw new ArgumentNullException(nameof(marketScheduleService));
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

        // Configure the ActionBlock with high capacity and parallelism
        var options = new ExecutionDataflowBlockOptions
        {
            BoundedCapacity = MaxBoundedCapacity,
            MaxDegreeOfParallelism = Environment.ProcessorCount * 4,
            EnsureOrdered = false,
            SingleProducerConstrained = true // Optimization if single producer
        };

        // Create processing block with proper error handling
        _realtimeMarketDataBlock =
            new ActionBlock<(Entry entry, string symbol, string venue, TaskCompletionSource<bool> completionSource)>(
                async data =>
                {
                    try
                    {
                        var success = await ProcessRealtimeMarketDataWithRetryAsync(data.entry, data.symbol, data.venue)
                            .ConfigureAwait(false);
                        if (success)
                        {
                            Interlocked.Increment(ref _totalProcessingSuccesses);
                            data.completionSource.TrySetResult(true);
                        }
                        else
                        {
                            Interlocked.Increment(ref _totalProcessingFailures);
                            _logger.LogWarning(
                                "Failed to process realtime market data for {Symbol} {Venue} after retries",
                                data.symbol,
                                data.venue);
                            data.completionSource.TrySetResult(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ensure all exceptions are caught to prevent ActionBlock from failing
                        Interlocked.Increment(ref _totalProcessingFailures);
                        _logger.LogError(ex,
                            "Unhandled exception in ProcessRealtimeMarketDataAsync for {Symbol} {Venue}",
                            data.symbol,
                            data.venue);
                        data.completionSource.TrySetException(ex);
                    }
                },
                options);

        // Set up periodic logging of processing metrics
        StartPeriodicMetricsLogging();
    }

    /// <summary>
    ///     Disposes resources
    /// </summary>
    public void Dispose()
    {
        _realtimeMarketDataBlock.Complete();
        try
        {
            // Log final metrics before disposal
            LogProcessingMetrics();

            // Wait for processing to complete with a timeout
            _logger.LogInformation("Waiting for data processing to complete...");
            if (!_realtimeMarketDataBlock.Completion.Wait(TimeSpan.FromMinutes(20)))
                _logger.LogWarning("Timeout waiting for processing to complete. Some data may not be processed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during dataflow block completion");
        }
    }

    /// <summary>
    ///     Handles a Kafka message
    /// </summary>
    /// <param name="message">The message to process</param>
    /// <returns>True if processing was successful, false otherwise</returns>
    [SuppressMessage("SonarQube", "S3776")]
    public async Task<bool> HandleAsync(Message<string, string> message)
    {
        if (message.Key.Equals("health_check", StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.IsNullOrEmpty(message.Value) || !message.Value.IsValidJsonMessage())
        {
            _logger.LogWarning("The message cannot be deserialized because it is invalid");
            return true;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var cleanMessage = message.Value.SimpleCleanJsonMessage();
            var result = FixData.FromJson(cleanMessage);

            if (result == null
                || string.IsNullOrEmpty(result.Symbol)
                || string.IsNullOrEmpty(result.MDReqID)
                || result.Entries == null)
            {
                _logger.LogWarning("The message cannot be deserialized because it is in an invalid format");
                return true;
            }

            var split = result.Symbol?.Split(".") ?? [];
            var symbol = string.Empty;
            var venue = string.Empty;

            switch (split.Length)
            {
                case 0:
                    _logger.LogWarning("Received empty symbol and venue");
                    return true;
                case 1:
                    symbol = split[0];
                    break;
                case 2:
                    symbol = split[0];
                    venue = split[1];
                    break;
            }

            venue = await GetWhiteListVenue(symbol, venue).ConfigureAwait(false);

            var tradeEntries = result.Entries?.Where(e => e.MDEntryType == FixMessageType.Trade) ?? [];
            var tradeEnumerable = tradeEntries as Entry[] ?? tradeEntries.ToArray();

            if (!tradeEnumerable.Any())
                return true;

            // Create a list to hold all processing tasks
            var processEntryTasks = new List<Task<bool>>();
            var eligibleEntries = new List<Entry>();

            // First, identify all eligible entries (market schedule validation)
            foreach (var entry in tradeEnumerable)
            {
                var marketSchedules = await _marketScheduleService.GetMarketSchedulesAsync(symbol, venue)
                    .ConfigureAwait(false);
                var marketSchedule = marketSchedules
                    .FirstOrDefault(target => !string.IsNullOrEmpty(target.Symbol)
                                              && target.Symbol.Equals(symbol)
                                              && entry.MDEntryTime >= target.UTCStartTime
                                              && entry.MDEntryTime <= target.UTCEndTime);

                if (marketSchedule is { MarketSession: not null }
                    && marketSchedule.MarketSession.Equals("MainSession", StringComparison.OrdinalIgnoreCase))
                    eligibleEntries.Add(entry);
            }

            // If no eligible entries, return success
            if (eligibleEntries.Count == 0)
                return true;

            // Process all eligible entries
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var entry in eligibleEntries)
            {
                var processingTask = PostToProcessingBlockAsync(entry, symbol, venue);
                processEntryTasks.Add(processingTask);
            }

            // Wait for all processing to complete
            var allResults = await Task.WhenAll(processEntryTasks).ConfigureAwait(false);

            // Only return true if TrueForAll entries were processed successfully
            var allSuccessful = allResults.ToList().TrueForAll(done => done);
            if (!allSuccessful)
                _logger.LogWarning(
                    "Some entries failed processing: {SuccessCount}/{TotalCount} for {Symbol} {Venue}",
                    allResults.Count(r => r), allResults.Length, symbol, venue);

            return allSuccessful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message: {Message}", ex.Message);
            return false;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "HandleAsync completed in {ElapsedMilliseconds} ms with InputCount={InputCount}",
                stopwatch.ElapsedMilliseconds,
                _realtimeMarketDataBlock.InputCount);
        }
    }

    /// <summary>
    ///     Processes a single realtime market data entry with retry
    /// </summary>
    [SuppressMessage("SonarQube", "S2589")]
    private async Task<bool> ProcessRealtimeMarketDataWithRetryAsync(Entry entry, string symbol, string venue)
    {
        const int maxRetries = 3;
        var retryCount = 0;
        var success = false;

        while (retryCount < maxRetries && !success)
            try
            {
                success = await ProcessRealtimeMarketDataAsync(entry, symbol, venue).ConfigureAwait(false);

                if (!success && retryCount < maxRetries - 1)
                {
                    retryCount++;
                    var delayMs = 100 * (1 << retryCount); // Exponential backoff
                    _logger.LogDebug(
                        "Retrying processing for {Symbol} {Venue}, attempt {Attempt}/{MaxRetries} after {Delay}ms",
                        symbol, venue, retryCount + 1, maxRetries, delayMs);

                    await Task.Delay(delayMs).ConfigureAwait(false);
                }
                else
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                retryCount++;
                _logger.LogError(ex,
                    "Error during processing attempt {Attempt}/{MaxRetries} for {Symbol} {Venue}",
                    retryCount, maxRetries, symbol, venue);

                if (retryCount >= maxRetries)
                    break;

                var delayMs = 200 * (1 << retryCount); // Exponential backoff with higher base for errors
                await Task.Delay(delayMs).ConfigureAwait(false);
            }

        return success;
    }

    /// <summary>
    ///     Processes a single realtime market data entry
    /// </summary>
    /// <param name="entry">The entry to process</param>
    /// <param name="symbol">The symbol</param>
    /// <param name="venue">The venue</param>
    /// <returns>True if processing was successful, false otherwise</returns>
    private async Task<bool> ProcessRealtimeMarketDataAsync(Entry entry, string symbol, string venue)
    {
        if (string.IsNullOrEmpty(symbol) || string.IsNullOrEmpty(venue))
        {
            _logger.LogWarning("Cannot process market data with empty symbol or venue");
            return false;
        }

        try
        {
            var realtimeMarketData = FixMapperService.MapToRealtimeMarketData(entry, symbol, venue);
            if (realtimeMarketData == null)
            {
                _logger.LogWarning("Failed to map entry to realtime market data for {Symbol} {Venue}", symbol, venue);
                return false;
            }

            using var scope = _scopeFactory.CreateScope();
            var realtimeMarketDataService =
                scope.ServiceProvider.GetRequiredService<ITimescaleService<RealtimeMarketData>>();

            await realtimeMarketDataService.UpsertAsync(
                realtimeMarketData,
                nameof(RealtimeMarketData.DateTime),
                nameof(RealtimeMarketData.Symbol),
                nameof(RealtimeMarketData.Venue)
            ).ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error to upsert realtimeMarketData {Symbol} {Venue}", symbol, venue);
            return false;
        }
    }

    /// <summary>
    ///     Gets the venue from whitelist or cache
    /// </summary>
    /// <param name="symbol">The symbol</param>
    /// <param name="venue">The current venue (maybe empty)</param>
    /// <returns>The venue from whitelist, or the original venue if not found or error</returns>
    private async Task<string> GetWhiteListVenue(string symbol, string venue)
    {
        if (!string.IsNullOrEmpty(venue))
            return venue;

        var cacheKey = $"{CacheKey.WhiteListProcessor}-exchange-{symbol}";

        try
        {
            var cachedData = await _redisPublisher.GetAsync<string>(cacheKey, true).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(cachedData))
                return cachedData;

            var whiteList = await _whiteListService.GetByFilterAsync(target =>
                !string.IsNullOrEmpty(target.Symbol) && target.Symbol.Equals(symbol)
            ).ConfigureAwait(false);

            if (whiteList is { Exchange: not null })
            {
                var exchange = whiteList.Exchange;

                await _redisPublisher.SetAsync(cacheKey, exchange, true, TimeSpan.FromDays(30))
                    .ConfigureAwait(false);

                return exchange;
            }

            _logger.LogDebug("No venue found in whitelist for {Symbol}", symbol);
            return string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving venue from whitelist for {Symbol}", symbol);
            return venue;
        }
    }

    /// <summary>
    ///     Posts an entry to the processing block and returns a task that completes when processing is done
    /// </summary>
    private async Task<bool> PostToProcessingBlockAsync(Entry entry, string symbol, string venue)
    {
        var completionSource = new TaskCompletionSource<bool>();
        var attempts = 0;

        while (attempts < MaxPostAttempts)
        {
            attempts++;
            Interlocked.Increment(ref _totalPostAttempts);

            if (_realtimeMarketDataBlock.Post((entry, symbol, venue, completionSource)))
                try
                {
                    // Wait for the processing to complete with a timeout
                    using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
                    return await completionSource.Task.WaitAsync(cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Nothing to do
                    Interlocked.Increment(ref _totalProcessingFailures);
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error waiting for processing completion for {Symbol} {Venue}", symbol, venue);
                    return false;
                }

            // Log when block is near capacity and we're having trouble posting
            if (attempts % 5 == 0 || _realtimeMarketDataBlock.InputCount >= MaxBoundedCapacity * 0.8)
                _logger.LogWarning(
                    "Dataflow block near capacity: {Count}/{Capacity}, attempt {Attempt}/{MaxAttempts}",
                    _realtimeMarketDataBlock.InputCount,
                    MaxBoundedCapacity,
                    attempts,
                    MaxPostAttempts);

            // Apply backoff delay to allow processing to catch up
            await Task.Delay(PostRetryDelayMs * (1 << Math.Min(8, attempts))).ConfigureAwait(false);
        }

        Interlocked.Increment(ref _totalPostFailures);
        _logger.LogError(
            "Failed to post message to processing block after {Attempts} attempts for {Symbol} {Venue}",
            attempts, symbol, venue);

        return false;
    }

    /// <summary>
    ///     Starts periodic logging of processing metrics
    /// </summary>
    private void StartPeriodicMetricsLogging()
    {
        Task.Run(async () =>
        {
            while (!_realtimeMarketDataBlock.Completion.IsCompleted)
            {
                await Task.Delay(TimeSpan.FromMinutes(5)).ConfigureAwait(false);
                LogProcessingMetrics();
            }
        });
    }

    /// <summary>
    ///     Logs the current processing metrics
    /// </summary>
    private void LogProcessingMetrics()
    {
        var successes = Interlocked.Read(ref _totalProcessingSuccesses);
        var failures = Interlocked.Read(ref _totalProcessingFailures);
        var postAttempts = Interlocked.Read(ref _totalPostAttempts);
        var postFailures = Interlocked.Read(ref _totalPostFailures);

        _logger.LogInformation(
            "Processing metrics: Successes={Successes}, Failures={Failures}, " +
            "Post attempts={PostAttempts}, Post failures={PostFailures}, " +
            "Current queue size={QueueSize}/{MaxCapacity}",
            successes, failures, postAttempts, postFailures,
            _realtimeMarketDataBlock.InputCount, MaxBoundedCapacity);

        // Reset counters after logging
        Interlocked.Exchange(ref _totalProcessingSuccesses, 0);
        Interlocked.Exchange(ref _totalProcessingFailures, 0);
        Interlocked.Exchange(ref _totalPostAttempts, 0);
        Interlocked.Exchange(ref _totalPostFailures, 0);
    }
}