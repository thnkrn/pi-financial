using System.Diagnostics;
using Confluent.Kafka;
using Serilog;
using Serilog.Events;

namespace Pi.SetMarketDataWSS.Infrastructure.Extensions;

public static class KafkaBatchingV2Extensions
{
    private static readonly ILogger Logger = Log.ForContext(typeof(KafkaBatchingV2Extensions));

    /// <summary>
    ///     Asynchronously consumes a batch of messages from Kafka.
    /// </summary>
    /// <typeparam name="TKey">The type of the message key.</typeparam>
    /// <typeparam name="TVal">The type of the message value.</typeparam>
    /// <param name="consumer">The Kafka consumer.</param>
    /// <param name="maxWaitTime">Maximum time to wait for completing the batch.</param>
    /// <param name="maxBatchSize">Maximum number of messages in the batch.</param>
    /// <param name="partitionFillTimeMs">Maximum time in milliseconds to spend filling from the same partition.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A task that resolves to a list of consumed messages.</returns>
    public static async Task<IEnumerable<ConsumeResult<TKey, TVal>>> ConsumeBatchAsync<TKey, TVal>(
        this IConsumer<TKey, TVal> consumer,
        TimeSpan maxWaitTime,
        int maxBatchSize,
        int partitionFillTimeMs = 50,
        CancellationToken ct = default)
    {
        // This is still using Task.Run because Confluent.Kafka's consumer is inherently synchronous
        // In a future version, consider using Kafka's native async API if/when available
        return await Task.Run(() =>
                ConsumeBatchV2(consumer, maxWaitTime, maxBatchSize, partitionFillTimeMs, ct),
            ct);
    }

    /// <summary>
    ///     Synchronously consumes a batch of messages from Kafka.
    /// </summary>
    /// <typeparam name="TKey">The type of the message key.</typeparam>
    /// <typeparam name="TVal">The type of the message value.</typeparam>
    /// <param name="consumer">The Kafka consumer.</param>
    /// <param name="maxWaitTime">Maximum time to wait for completing the batch.</param>
    /// <param name="maxBatchSize">Maximum number of messages in the batch.</param>
    /// <param name="partitionFillTimeMs">Maximum time in milliseconds to spend filling from the same partition.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A list of consumed messages.</returns>
    public static IEnumerable<ConsumeResult<TKey, TVal>> ConsumeBatchV2<TKey, TVal>(
        this IConsumer<TKey, TVal> consumer,
        TimeSpan maxWaitTime,
        int maxBatchSize,
        int partitionFillTimeMs = 50,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(consumer);

        if (maxBatchSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxBatchSize), "Batch size must be positive");

        var startTime = DateTime.UtcNow;
        var deadline = startTime.Add(maxWaitTime);
        var batch = new List<ConsumeResult<TKey, TVal>>(maxBatchSize);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            while (batch.Count < maxBatchSize && DateTime.UtcNow < deadline)
            {
                ct.ThrowIfCancellationRequested();

                if (!TryFillBatch(consumer, maxBatchSize, ct, deadline, batch, partitionFillTimeMs))
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            // Let cancellation propagate up
            throw;
        }
        catch (KafkaException ex)
        {
            // Preserve Kafka exceptions for specific handling
            Logger.Error(ex, "Kafka error in ConsumeBatch");
            throw new InvalidOperationException("Error while consuming batch from Kafka", ex);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unexpected error in ConsumeBatch");
            throw new InvalidOperationException("Error while consuming batch from Kafka", ex);
        }
        finally
        {
            stopwatch.Stop();
        }

        // Only build diagnostic information if debug logging is enabled
        if (Logger.IsEnabled(LogEventLevel.Debug) && batch.Count > 0)
        {
            var partitionCounts = batch
                .GroupBy(m => m.Partition.Value)
                .Select(g => $"P({g.Key}):C({g.Count()})");

            Logger.Debug(
                "Returning {Count} message(s) from ConsumeBatch in {ElapsedMs}ms. Partitions: {Partitions}",
                batch.Count,
                stopwatch.ElapsedMilliseconds,
                string.Join(", ", partitionCounts));
        }

        return batch;
    }

    /// <summary>
    ///     Attempts to fill the batch with messages from Kafka.
    /// </summary>
    /// <returns>True if more attempts should be made to fill the batch, false otherwise.</returns>
#pragma warning disable CA1068
    private static bool TryFillBatch<TKey, TVal>(
#pragma warning restore CA1068
        IConsumer<TKey, TVal> consumer,
        int maxBatchSize,
        CancellationToken ct,
        DateTime deadline,
        List<ConsumeResult<TKey, TVal>> batch,
        int partitionFillTimeMs)
    {
        ct.ThrowIfCancellationRequested();

        // Calculate timeout for initial consumption
        var now = DateTime.UtcNow;
        var remainingTime = deadline - now;
        if (remainingTime <= TimeSpan.Zero) return false;

        // Use a timeout that's adjusted based on remaining time, but capped
        // Divide by 2 to allow for multiple attempts within the remaining time
        var timeoutMs = Math.Min(Math.Max(1, remainingTime.TotalMilliseconds / 2), 10);

        // Try to get the first message
        var msg = consumer.Consume(TimeSpan.FromMilliseconds(timeoutMs));
        if (msg == null || msg.IsPartitionEOF) return false;

        batch.Add(msg);

        // Try to quickly fill the batch with messages from the same partition
        if (batch.Count < maxBatchSize)
            TryFillFromSamePartition(consumer, msg.TopicPartition, batch, maxBatchSize, partitionFillTimeMs, ct);

        // Return true if we should continue trying to fill the batch
        return batch.Count < maxBatchSize;
    }

    /// <summary>
    ///     Attempts to quickly fill the batch with additional messages from the same partition.
    /// </summary>
    private static void TryFillFromSamePartition<TKey, TVal>(
        IConsumer<TKey, TVal> consumer,
        TopicPartition partition,
        List<ConsumeResult<TKey, TVal>> batch,
        int maxBatchSize,
        int partitionFillTimeMs,
        CancellationToken ct)
    {
        var partitionFillDeadline = DateTime.UtcNow.AddMilliseconds(partitionFillTimeMs);
        var shortTimeout = TimeSpan.Zero;
        var maxAttempts = Math.Min(maxBatchSize - batch.Count, 100);
        var attempts = 0;

        while (batch.Count < maxBatchSize && DateTime.UtcNow < partitionFillDeadline && attempts < maxAttempts)
        {
            attempts++;
            ct.ThrowIfCancellationRequested();

            var additionalMsg = consumer.Consume(shortTimeout);
            if (additionalMsg == null || additionalMsg.IsPartitionEOF)
                break;

            batch.Add(additionalMsg);

            if (!additionalMsg.TopicPartition.Equals(partition))
                break;
        }
    }
}