using System.Text;
using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Domain.Models.Kafka;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IStockDataOptimizedV2Publisher
{
    ValueTask EnqueueMessageAsync(ItchMessage message, string cleanMessage);
    ValueTask DisposeAsync();
    Task<bool> IsHealthy(CancellationToken cancellationToken = default);
}

public class StockDataOptimizedV2Publisher : IStockDataOptimizedV2Publisher, IAsyncDisposable
{
    private readonly string _bidOfferDataTopic;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IConfiguration _configuration;
    private readonly IKafkaProducerOptimizedV2Service _kafkaProducer;
    private readonly ILogger<StockDataOptimizedV2Publisher> _logger;
    private readonly Channel<StockDataMessage> _messageChannel;
    private readonly Task[] _processTasks;
    private readonly string _tradeDataTopic;
    private readonly string _product;

    public StockDataOptimizedV2Publisher(
        IConfiguration configuration,
        IKafkaProducerOptimizedV2Service kafkaProducer,
        ILogger<StockDataOptimizedV2Publisher> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
        _configuration = configuration;

        var channelCapacity = configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsChannelCapacity, 100000); // Increased from default
        var enableBackpressure = configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsEnableBackpressure, true);

        _messageChannel = Channel.CreateBounded<StockDataMessage>(
            new BoundedChannelOptions(channelCapacity)
            {
                SingleReader = false, // Allow multiple readers
                SingleWriter = false,
                FullMode = enableBackpressure
                    ? BoundedChannelFullMode.Wait
                    : BoundedChannelFullMode.DropOldest
            });

        // Initialize topic configuration
        _tradeDataTopic = configuration[ConfigurationKeys.KafkaTopic] ?? "set_stock_market_data";
        _bidOfferDataTopic = configuration[ConfigurationKeys.KafkaBidOfferTopic] ?? "set_stock_market_bid_offer";
        _product = configuration[ConfigurationKeys.Product] ?? "Equity";
        _cancellationTokenSource = new CancellationTokenSource();

        // Create multiple consumer tasks for parallel processing
        var processorCount = Environment.ProcessorCount;

        _processTasks = new Task[processorCount];

        for (var i = 0; i < processorCount; i++)
            _processTasks[i] = ProcessMessagesAsync(_cancellationTokenSource.Token);

        _logger.LogInformation("Initialized StockDataOptimizedV2Publisher with {ProcessorCount} processors",
            processorCount);
    }

    public async ValueTask EnqueueMessageAsync(ItchMessage message, string cleanMessage)
    {
        try
        {
            string messageKey;
            var seqNo = DateTime.UtcNow.Ticks;
            var stockMessage = new StockMessage
            {
                MessageType = message.MsgType.ToString(),
                Message = cleanMessage
            };

            if (message.Metadata != null)
            {
                if (!string.IsNullOrEmpty(message.Metadata.OrderBookId))
                {
                    messageKey = $"{message.Metadata.OrderBookId}_{_product}";
                }
                else
                {
                    var sequenceRange = message.Metadata.SequenceNumber / 10000;
                    messageKey = $"{message.MsgType}_{sequenceRange}";
                }
            }
            else
            {
                messageKey = Guid.NewGuid().ToString("N");
            }

            if (message.MsgType == ItchMessageType.b)
                // Create and enqueue snapshot for BidOffer Topic if we have entries for it
                await EnqueueSnapshotAsync(stockMessage, _bidOfferDataTopic, messageKey, message.Metadata?.OrderBookId,
                    seqNo);
            else
                // Create and enqueue snapshot for Trade Topic if we have entries for it
                await EnqueueSnapshotAsync(stockMessage, _tradeDataTopic, messageKey, message.Metadata?.OrderBookId,
                    seqNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue stock message for OrderBookId {Symbol}",
                message.Metadata?.OrderBookId ?? "unknown");
        }
    }

#pragma warning disable CA1816
    public async ValueTask DisposeAsync()
#pragma warning restore CA1816
    {
        try
        {
            _messageChannel.Writer.Complete();
            await _cancellationTokenSource.CancelAsync();

            // Wait with timeout to avoid blocking indefinitely
            if (await Task.WhenAny(Task.WhenAll(_processTasks), Task.Delay(5000)) != Task.WhenAll(_processTasks))
                _logger.LogWarning("Some process tasks did not complete within the timeout period");
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }

    public async Task<bool> IsHealthy(CancellationToken cancellationToken = default)
    {
        return await _kafkaProducer.IsHealthy(cancellationToken);
    }

    private async Task EnqueueSnapshotAsync(StockMessage stockMessage,
        string topic,
        string messageKey,
        string? orderBookId,
        long sequenceNumber)
    {
        // Serialize message once to avoid multiple serializations
        var serializedValue = JsonConvert.SerializeObject(stockMessage);
        var kafkaMessage = new Message<string, string>
        {
            Key = messageKey,
            Value = serializedValue,
            Timestamp = new Timestamp(DateTime.UtcNow, TimestampType.CreateTime),
            // Add headers for additional metadata
            Headers = new Headers
            {
                { "orderBookId", Encoding.UTF8.GetBytes(orderBookId ?? string.Empty) },
                { "sequenceNumber", BitConverter.GetBytes(sequenceNumber) }
            }
        };

        var stockDataMessage = new StockDataMessage
        {
            KafkaMessage = kafkaMessage,
            Topic = topic
        };

        await _messageChannel.Writer.WriteAsync(stockDataMessage);
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        var batchConfig = GetBatchConfiguration();
        var topicBatches = InitializeTopicBatches(batchConfig.batchSize);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ClearBatches(topicBatches);

                // Collect messages and fill batches
                var messageCount = await CollectMessagesForBatchAsync(
                    topicBatches, batchConfig, cancellationToken);

                // Send all non-empty batches
                await SendBatchesAsync(topicBatches, cancellationToken);

                // Add small delay if no messages were processed to avoid CPU spinning
                if (messageCount == 0)
                    await DelayWithCancellationHandlingAsync(10, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Normal during shutdown, no need to log
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in message processing loop");
        }
    }

    private (int batchSize, int batchWindowMs, int maxWaitTimeMs) GetBatchConfiguration()
    {
        var batchSize = _configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsBatchSize, 30);
        var batchWindowMs = _configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsBatchWindowMs, 15);
        var maxWaitTimeMs = _configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsMaxWaitTimeMs, 100);

        return (batchSize, batchWindowMs, maxWaitTimeMs);
    }

    private Dictionary<string, List<Message<string, string>>> InitializeTopicBatches(int batchSize)
    {
        return new Dictionary<string, List<Message<string, string>>>
        {
            [_tradeDataTopic] = new(batchSize),
            [_bidOfferDataTopic] = new(batchSize)
        };
    }

    private static void ClearBatches(Dictionary<string, List<Message<string, string>>> topicBatches)
    {
        foreach (var batch in topicBatches.Values)
            batch.Clear();
    }

    private async Task<int> CollectMessagesForBatchAsync(
        Dictionary<string, List<Message<string, string>>> topicBatches,
        (int batchSize, int batchWindowMs, int maxWaitTimeMs) batchConfig,
        CancellationToken cancellationToken)
    {
        var messageCount = 0;
        var startTime = DateTime.UtcNow;
        var firstMessageTime = DateTime.MinValue;

        while (ShouldContinueCollectingMessages(
                   messageCount, batchConfig.batchSize, startTime,
                   firstMessageTime, batchConfig.batchWindowMs, batchConfig.maxWaitTimeMs))
        {
            // Calculate timeout for the next read operation
            var timeout = CalculateTimeout(firstMessageTime, batchConfig.maxWaitTimeMs, batchConfig.batchWindowMs);
            if (timeout <= 0) break;

            // Try to read the next message with timeout
            var message = await TryReadMessageWithTimeoutAsync(timeout, cancellationToken);
            if (message == null) break;

            // Update first message time if this is the first message
            if (firstMessageTime == DateTime.MinValue)
                firstMessageTime = DateTime.UtcNow;

            // Add message to the appropriate batch
            if (topicBatches.TryGetValue(message.Topic, out var batch))
            {
                batch.Add(message.KafkaMessage);
                messageCount++;
            }
        }

        return messageCount;
    }

    private static bool ShouldContinueCollectingMessages(
        int messageCount, int batchSize, DateTime startTime,
        DateTime firstMessageTime, int batchWindowMs, int maxWaitTimeMs)
    {
        return messageCount < batchSize &&
               (DateTime.UtcNow - startTime).TotalMilliseconds < maxWaitTimeMs &&
               (firstMessageTime == DateTime.MinValue ||
                (DateTime.UtcNow - firstMessageTime).TotalMilliseconds < batchWindowMs);
    }

    private static int CalculateTimeout(DateTime firstMessageTime, int maxWaitTimeMs, int batchWindowMs)
    {
        return firstMessageTime == DateTime.MinValue
            ? maxWaitTimeMs
            : batchWindowMs - (int)(DateTime.UtcNow - firstMessageTime).TotalMilliseconds;
    }

    private async Task<StockDataMessage?> TryReadMessageWithTimeoutAsync(int timeoutMs,
        CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(timeoutMs);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            timeoutCts.Token, cancellationToken);

        try
        {
            if (!await _messageChannel.Reader.WaitToReadAsync(linkedCts.Token))
                return null;
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            // This is normal when the timeout is reached
            return null;
        }

        return _messageChannel.Reader.TryRead(out var message) ? message : null;
    }

    private async Task SendBatchesAsync(
        Dictionary<string, List<Message<string, string>>> topicBatches,
        CancellationToken cancellationToken)
    {
        var sendTasks = new List<Task>();

        foreach (var (topic, messages) in topicBatches)
        {
            if (messages.Count == 0)
                continue;

            var messagesToSend = messages.ToList();
            sendTasks.Add(SendSingleBatchAsync(topic, messagesToSend, cancellationToken));
        }

        if (sendTasks.Count > 0)
            await Task.WhenAny(
                Task.WhenAll(sendTasks),
                Task.Delay(5000, cancellationToken));
    }

    private Task SendSingleBatchAsync(
        string topic,
        List<Message<string, string>> messagesToSend,
        CancellationToken cancellationToken)
    {
        return Task.Run(async () =>
        {
            try
            {
                await _kafkaProducer.ProduceBatchAsync(topic, messagesToSend, cancellationToken);

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Sent batch of {Count} messages to topic {Topic}",
                        messagesToSend.Count, topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send batch of {Count} messages to topic {Topic}",
                    messagesToSend.Count, topic);
            }
        }, cancellationToken);
    }

    private static async Task DelayWithCancellationHandlingAsync(int delayMs, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delayMs, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Propagate cancellation
            throw;
        }
    }
}

// Simple message wrapper
public class StockDataMessage
{
    public required Message<string, string> KafkaMessage { get; init; }
    public required string Topic { get; init; }
}