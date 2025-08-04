namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class ProducerMetricsSnapshot
{
    public long MessageCount { get; set; }
    public long BytesProduced { get; set; }
    public double MessagesPerSecond { get; set; }
    public double BytesPerSecond { get; set; }
    public double AverageLatencyMs { get; set; }
    public double P95LatencyMs { get; set; }
    public double P99LatencyMs { get; set; }
    public long UptimeMs { get; set; }
    public Dictionary<string, long>? MessageCountByTopic { get; set; }
    public Dictionary<string, long>? BytesByTopic { get; set; }
}