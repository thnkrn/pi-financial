using System.Collections.Concurrent;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IStockDataPublisher
{
    Task PublishStockData<T>(string symbol, T data);
    Task PublishBatchStockData<T>(IEnumerable<Tuple<string, T>> dataBatch);
}

public class StockDataPublisher : IStockDataPublisher
{
    private static readonly ConcurrentBag<Message<string, string>> BatchBuffer = [];
    private static readonly object Lock = new();
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly ILogger<StockDataPublisher> _logger;

    /// <summary>
    /// </summary>
    /// <param name="kafkaProducer"></param>
    /// <param name="logger"></param>
    public StockDataPublisher(IKafkaProducerService kafkaProducer, ILogger<StockDataPublisher> logger)
    {
        _logger = logger;
        _kafkaProducer = kafkaProducer;
    }

    public async Task PublishStockData<T>(string symbol, T data)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = symbol,
            Value = JsonSerializer.Serialize(data)
        };

        _ = _kafkaProducer.ProduceMessageAsync(kafkaMessage);

        // Log current metrics
        var metrics = _kafkaProducer.GetMetrics();
        _logger.LogInformation(
            "Current throughput: {MessagesPerSecond:F2} messages/sec, Avg latency: {AverageLatencyMs:F2}ms",
            metrics.MessagesPerSecond,
            metrics.AverageLatencyMs);

        await Task.CompletedTask;
    }

    public async Task PublishBatchStockData<T>(IEnumerable<Tuple<string, T>> dataBatch)
    {
        var messages = dataBatch.Select(data =>
            new Message<string, string>
            {
                Key = data.Item1,
                Value = JsonSerializer.Serialize(data.Item2)
            }).ToArray();

        _ = _kafkaProducer.ProduceBatchAsync(messages);

        await Task.CompletedTask;
    }

    public async Task PublishStockDataLatest<T>(string symbol, T data)
    {
        var message = new Message<string, string>
        {
            Key = symbol,
            Value = JsonSerializer.Serialize(data)
        };

        lock (Lock)
        {
            BatchBuffer.Add(message);
            if (BatchBuffer.Count >= 20) // Send every 20 messages
            {
                var messagesToSend = BatchBuffer.ToArray();
                BatchBuffer.Clear();
                _ = _kafkaProducer.ProduceBatchAsync(messagesToSend);
            }
        }

        await Task.CompletedTask;
    }
}