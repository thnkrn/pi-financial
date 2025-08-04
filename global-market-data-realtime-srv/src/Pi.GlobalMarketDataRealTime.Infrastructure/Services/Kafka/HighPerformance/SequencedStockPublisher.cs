using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface ISequencedStockPublisher
{
    Task PublishStockData<T>(string symbol, T data);
    Task PublishBatchStockData<T>(IEnumerable<Tuple<string, T>> dataBatch);
}

public sealed class SequencedStockPublisher : ISequencedStockPublisher, IDisposable
{
    private readonly CancellationTokenSource _cancellationToken;
    private readonly Channel<(string Symbol, object Data, long Sequence)> _channel;
    private readonly ILogger<SequencedStockPublisher> _logger;
    private readonly IStockDataPublisher _publisher;
    private readonly ConcurrentDictionary<string, long> _sequenceBySymbol;
    private bool _disposed;

    public SequencedStockPublisher(
        IStockDataPublisher publisher,
        ILogger<SequencedStockPublisher> logger)
    {
        _publisher = publisher;
        _logger = logger;
        _sequenceBySymbol = new ConcurrentDictionary<string, long>();
        _cancellationToken = new CancellationTokenSource();

        var options = new BoundedChannelOptions(10000) // Limit queue size to 10,000 messages
        {
            FullMode = BoundedChannelFullMode.Wait, // Backpressure handling
            SingleReader = false, // Allow multiple readers
            SingleWriter = false
        };
        _channel = Channel.CreateBounded<(string Symbol, object Data, long Sequence)>(options);

        // Start background processor
        _ = ProcessMessagesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task PublishStockData<T>(string symbol, T data)
    {
        ThrowIfDisposed();

        if (string.IsNullOrEmpty(symbol))
        {
            _logger.LogWarning("Attempted to publish with null or empty symbol");
            return;
        }

        try
        {
            // Get next sequence number for this symbol
            var sequence = _sequenceBySymbol.AddOrUpdate(
                symbol,
                1,
                (_, current) => current + 1
            );

            // ReSharper disable once NullableWarningSuppressionIsUsed
            // Fire and forget - write to channel
            await _channel.Writer.WriteAsync((symbol, data!, sequence));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error queueing message for symbol {Symbol}", symbol);
            throw new InvalidOperationException(ex.Message);
        }
    }

    public async Task PublishBatchStockData<T>(IEnumerable<Tuple<string, T>> dataBatch)
    {
        ThrowIfDisposed();

        foreach (var (symbol, data) in dataBatch)
            await PublishStockData(symbol, data);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
            try
            {
                _cancellationToken.Cancel();
                _cancellationToken.Dispose();

                // Complete the channel
                _channel.Writer.Complete();

                // Allow tasks to finish
                Task.Delay(500).Wait();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during producer disposal: {Message}", ex.Message);
            }

        _disposed = true;
    }

    ~SequencedStockPublisher()
    {
        Dispose(false);
    }

    private async Task ProcessMessagesAsyncOriginal()
    {
        try
        {
            var lastPublishedSequence = new ConcurrentDictionary<string, long>();

            await foreach (var (symbol, data, sequence) in _channel.Reader.ReadAllAsync(_cancellationToken.Token))
                _ = Task.Run(async () => // **Process messages in parallel**
                {
                    try
                    {
                        var lastSequence = lastPublishedSequence.GetOrAdd(symbol, 0);
                        if (sequence != lastSequence + 1)
                        {
                            _logger.LogWarning(
                                "Out of sequence message detected for {Symbol}. Expected {Expected}, got {Actual}",
                                symbol, lastSequence + 1, sequence);
                            return; // Skip out-of-sequence messages
                        }

                        await _publisher.PublishStockData(symbol, data);
                        lastPublishedSequence[symbol] = sequence;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error publishing message for symbol {Symbol} with sequence {Sequence}",
                            symbol, sequence);
                    }
                });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogInformation(ex, "Message processing cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in message processing loop");
        }
    }

    private async Task ProcessMessagesAsync()
    {
        var workers = Enumerable.Range(0, Environment.ProcessorCount) // Parallel workers
            .Select(_ => Task.Run(async () =>
            {
                try
                {
                    var lastPublishedSequence = new ConcurrentDictionary<string, long>();

                    await foreach (var (symbol, data, sequence) in _channel.Reader.ReadAllAsync(
                                       _cancellationToken.Token))
                        try
                        {
                            var lastSequence = lastPublishedSequence.GetOrAdd(symbol, 0);
                            if (sequence != lastSequence + 1)
                            {
                                _logger.LogWarning(
                                    "Out of sequence message for {Symbol}. Expected {Expected}, got {Actual}",
                                    symbol, lastSequence + 1, sequence);
                                continue;
                            }

                            await _publisher.PublishStockData(symbol, data);
                            lastPublishedSequence[symbol] = sequence;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error publishing message for {Symbol} with sequence {Sequence}",
                                symbol, sequence);
                        }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogWarning(ex, "Message processing cancelled");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Fatal error in message processing loop");
                }
            }))
            .ToArray();

        await Task.WhenAll(workers); // Run multiple consumers
    }


    private void ThrowIfDisposed()
    {
        if (!_disposed) return;
        throw new ObjectDisposedException(nameof(KafkaProducerService));
    }
}