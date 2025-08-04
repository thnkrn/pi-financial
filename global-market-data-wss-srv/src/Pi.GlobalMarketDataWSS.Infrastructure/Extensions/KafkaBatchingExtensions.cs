using System.Diagnostics;
using Confluent.Kafka;
using Serilog;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Extensions;

public static class KafkaBatchingExtensions
{
    private static readonly ILogger Logger = Log.ForContext(typeof(KafkaBatchingExtensions));

    public static IEnumerable<ConsumeResult<TKey, TVal>> ConsumeBatch<TKey, TVal>(this IConsumer<TKey, TVal> consumer,
        TimeSpan maxWaitTime,
        int maxBatchSize,
        CancellationToken? ct = null)
    {
        var deadline = DateTime.UtcNow.Add(maxWaitTime);
        var batch = new List<ConsumeResult<TKey, TVal>>();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            while (batch.Count < maxBatchSize && DateTime.UtcNow < deadline)
                if (!GetValue(consumer, maxBatchSize, ct, deadline, batch))
                    break;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error in ConsumeBatch");
            // Rethrow to allow caller to handle
            throw new InvalidOperationException(ex.Message, ex);
        }
        finally
        {
            stopwatch.Stop();
        }

        // Log batch details including partitions
        if (batch.Count > 0)
        {
            var partitionCounts = batch
                .GroupBy(m => m.Partition.Value)
                .Select(g => $"P({g.Key}):C({g.Count()})")
                .ToList();

            Logger.Debug(
                "Returning {Count} message(s) from ConsumeBatch in {ElapsedMs}ms. Partitions: {Partitions}",
                batch.Count,
                stopwatch.ElapsedMilliseconds,
                string.Join(", ", partitionCounts));
        }

        return batch;
    }

    // In KafkaBatchingExtensions, adjust batch collection logic
    private static bool GetValue<TKey, TVal>(IConsumer<TKey, TVal> consumer,
        int maxBatchSize,
        CancellationToken? ct,
        DateTime deadline,
        List<ConsumeResult<TKey, TVal>> batch)
    {
        ct?.ThrowIfCancellationRequested();

        // For market data, prefer batch size over waiting
        var timeoutMs = Math.Min(10, (deadline - DateTime.UtcNow).TotalMilliseconds);
        if (timeoutMs <= 0) return false;

        var msg = consumer.Consume(TimeSpan.FromMilliseconds(timeoutMs));
        if (msg == null || msg.IsPartitionEOF) return true;

        batch.Add(msg);

        // Try to quickly fill the batch (non-blocking consume)
        // Keeping the timeout extremely short (1ms) to avoid delays
        while (batch.Count < maxBatchSize)
        {
            var additionalMsg = consumer.Consume(TimeSpan.FromMilliseconds(1));
            if (additionalMsg == null || additionalMsg.IsPartitionEOF) break;
            batch.Add(additionalMsg);
        }

        return batch.Count < maxBatchSize;
    }
}