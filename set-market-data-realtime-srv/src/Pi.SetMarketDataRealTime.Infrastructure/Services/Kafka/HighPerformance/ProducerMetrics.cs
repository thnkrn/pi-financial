using System.Collections.Concurrent;
using System.Diagnostics;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public class ProducerMetrics
{
    private readonly ConcurrentDictionary<string, long> _bytesProducedByTopic = new();
    private readonly ConcurrentQueue<double> _latencies = new();
    private readonly ConcurrentDictionary<string, long> _messageCountByTopic = new();
    private readonly Stopwatch _uptime = Stopwatch.StartNew();
    private DateTime _lastResetTime = DateTime.UtcNow;
    private long _totalBytesProduced;
    private long _totalMessageCount;

    public void RecordLatency(double latencyMs)
    {
        _latencies.Enqueue(latencyMs);
        // Keep only last 1000 latency measurements
        while (_latencies.Count > 1000)
            _latencies.TryDequeue(out _);
    }

    public void RecordMessage(string topic, int messageSize)
    {
        Interlocked.Increment(ref _totalMessageCount);
        Interlocked.Add(ref _totalBytesProduced, messageSize);

        _messageCountByTopic.AddOrUpdate(topic, 1, (_, count) => count + 1);
        _bytesProducedByTopic.AddOrUpdate(topic, messageSize, (_, bytes) => bytes + messageSize);
    }

    public ProducerMetricsSnapshot GetSnapshot()
    {
        var currentTime = DateTime.UtcNow;
        var timespan = (currentTime - _lastResetTime).TotalSeconds;

        var latencies = _latencies.ToArray();
        var p99Latency = latencies.Length > 0 ? Percentile(latencies, 0.99) : 0;
        var p95Latency = latencies.Length > 0 ? Percentile(latencies, 0.95) : 0;
        var avgLatency = latencies.Length > 0 ? latencies.Average() : 0;

        return new ProducerMetricsSnapshot
        {
            MessageCount = _totalMessageCount,
            BytesProduced = _totalBytesProduced,
            MessagesPerSecond = _totalMessageCount / timespan,
            BytesPerSecond = _totalBytesProduced / timespan,
            AverageLatencyMs = avgLatency,
            P95LatencyMs = p95Latency,
            P99LatencyMs = p99Latency,
            UptimeMs = _uptime.ElapsedMilliseconds,
            MessageCountByTopic = new Dictionary<string, long>(_messageCountByTopic),
            BytesByTopic = new Dictionary<string, long>(_bytesProducedByTopic)
        };
    }

    private static double Percentile(double[] sequence, double percentile)
    {
        Array.Sort(sequence);
        var length = sequence.Length;
        var n = (length - 1) * percentile + 1;

        if (n.Equals(1)) return sequence[0];
        if (n.Equals(length)) return sequence[length - 1];

        var k = (int)n;
        var d = n - k;
        return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
    }

    public void Reset()
    {
        _totalMessageCount = 0;
        _totalBytesProduced = 0;
        _lastResetTime = DateTime.UtcNow;
        _messageCountByTopic.Clear();
        _bytesProducedByTopic.Clear();
        while (!_latencies.IsEmpty)
            _latencies.TryDequeue(out _);
    }
}