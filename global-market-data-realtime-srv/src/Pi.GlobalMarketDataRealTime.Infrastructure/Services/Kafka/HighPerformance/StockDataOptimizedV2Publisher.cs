using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using Pi.GlobalMarketDataRealTime.Infrastructure.Helpers;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IStockDataOptimizedV2Publisher
{
    ValueTask EnqueueMessageAsync(MarketDataSnapshotFullRefresh message);
    ValueTask DisposeAsync();
    Task<bool> IsHealthy(CancellationToken cancellationToken = default);
}

public class StockDataOptimizedV2Publisher : IStockDataOptimizedV2Publisher, IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;

    private readonly ObjectPool<MarketDataSnapshotFullRefresh.NoMDEntriesGroup> _groupPool =
        new DefaultObjectPool<MarketDataSnapshotFullRefresh.NoMDEntriesGroup>(
            new DefaultPooledObjectPolicy<MarketDataSnapshotFullRefresh.NoMDEntriesGroup>());

    private readonly IKafkaProducerOptimizedV2Service _kafkaProducer;
    private readonly ILogger<StockDataOptimizedV2Publisher> _logger;
    private readonly Channel<StockDataMessage> _messageChannel;
    private readonly Task[] _processTasks;
    private readonly string _tradeDataTopic;
    private readonly HashSet<char> _tradeEntryTypes;

    public StockDataOptimizedV2Publisher(
        IConfiguration configuration,
        IKafkaProducerOptimizedV2Service kafkaProducer,
        ILogger<StockDataOptimizedV2Publisher> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;

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
        _tradeDataTopic = configuration[ConfigurationKeys.KafkaTradeDataTopic] ?? "ge_stock_market_data";

        // Define entry types for each topic
        _tradeEntryTypes =
        [
            MDEntryType.TRADE, // '2'
            MDEntryType.OPENING_PRICE, // '4'
            MDEntryType.CLOSING_PRICE, // '5'
            MDEntryType.BID, // '0'
            MDEntryType.OFFER, // '1'
            MDEntryType.TRADE_VOLUME // 'B'
        ];

        _cancellationTokenSource = new CancellationTokenSource();

        // Create multiple consumer tasks for parallel processing
        const int processorCount = 12;
        _processTasks = new Task[processorCount];

        for (var i = 0; i < processorCount; i++)
            _processTasks[i] = ProcessMessagesAsync(_cancellationTokenSource.Token);

        _logger.LogInformation("Initialized StockDataOptimizedV2Publisher with {ProcessorCount} processors",
            processorCount);
    }

    [SuppressMessage("SonarQube", "S3776")]
    public async ValueTask EnqueueMessageAsync(MarketDataSnapshotFullRefresh message)
    {
        try
        {
            var symbol = message.GetString(Tags.Symbol);
            var mdReqId = message.GetString(Tags.MDReqID);

            // Get the sequence number of the message
            var seqNo = DateTime.UtcNow.Ticks;

            // Try to get the sending time
            var sendingTime = message.Header.IsSetField(Tags.SendingTime)
                ? message.Header.GetString(Tags.SendingTime)
                : string.Empty;

            var group = _groupPool.Get();
            List<MarketDataEntry> entries;

            try
            {
                entries = FixListenerOptimizedHelper.ExtractMarketDataEntries(message, group);
            }
            finally
            {
                _groupPool.Return(group);
            }

            if (entries.Count == 0)
                return;

            // Group entries by topic
            var tradeEntries = new List<MarketDataEntry>();
            var typesBuilder = new StringBuilder(entries.Count);

            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.MdEntryType) || entry.MdEntryType.Length != 1)
                    continue;

                var entryType = entry.MdEntryType[0];

                if (_tradeEntryTypes.Contains(entryType))
                {
                    tradeEntries.Add(entry);
                }

                typesBuilder.Append(entryType.ToString());
            }

            var types = typesBuilder.ToString();
            if (tradeEntries.Count > 0)
            {
                // Create and enqueue snapshot for Trade Topic if we have entries for it
                var enqueueSnapshot = new EnqueueSnapshot
                {
                    Symbol = symbol,
                    MdReqId = mdReqId,
                    MdEntryType = $"Trade_{types}",
                    SendingTime = sendingTime,
                    Topic = _tradeDataTopic,
                    MessageKey = symbol,
                    SequenceNumber = seqNo
                };

                await EnqueueSnapshotAsync(enqueueSnapshot, tradeEntries);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue stock message for symbol {Symbol}",
                message.IsSetField(Tags.Symbol) ? message.GetString(Tags.Symbol) : "unknown");
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

    private async Task EnqueueSnapshotAsync(EnqueueSnapshot enqueueSnapshot, List<MarketDataEntry> entries)
    {
        var snapshot = new MarketDataSnapshot
        {
            Symbol = enqueueSnapshot.Symbol,
            SendingTime = enqueueSnapshot.SendingTime,
            SequenceNumber = enqueueSnapshot.SequenceNumber,
            MdReqId = enqueueSnapshot.MdReqId,
            MdEntryType = enqueueSnapshot.MdEntryType,
            Entries = entries
        };

        // Serialize message once to avoid multiple serializations
        var compositeKey = $"{enqueueSnapshot.MessageKey}";
        if (string.IsNullOrEmpty(compositeKey))
            compositeKey = Guid.NewGuid().ToString("N");

        var serializedValue = JsonConvert.SerializeObject(snapshot);
        var kafkaMessage = new Message<string, string>
        {
            Key = compositeKey,
            Value = serializedValue,
            Timestamp = new Timestamp(DateTime.UtcNow, TimestampType.CreateTime),
            // Add headers for additional metadata
            Headers = new Headers
            {
                { "symbol", Encoding.UTF8.GetBytes(enqueueSnapshot.Symbol ?? string.Empty) },
                { "sendingTime", Encoding.UTF8.GetBytes(enqueueSnapshot.SendingTime ?? string.Empty) },
                { "sequenceNumber", BitConverter.GetBytes(enqueueSnapshot.SequenceNumber) }
            }
        };

        var stockMessage = new StockDataMessage
        {
            KafkaMessage = kafkaMessage,
            Topic = enqueueSnapshot.Topic ?? string.Empty
        };

        await _messageChannel.Writer.WriteAsync(stockMessage);
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
                var messageCount = await CollectMessagesForBatchAsync(topicBatches, batchConfig, cancellationToken);

                // Send all non-empty batches
                await SendBatchesAsync(topicBatches, cancellationToken);

                // Add small delay if no messages were processed to avoid CPU spinning
                if (messageCount == 0)
                    await DelayWithCancellationHandlingAsync(3, cancellationToken);
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

    private static (int batchSize, int batchWindowMs, int maxWaitTimeMs) GetBatchConfiguration()
    {
        const int batchSize = 200;
        const int batchWindowMs = 25;
        const int maxWaitTimeMs = 100;

        return (batchSize, batchWindowMs, maxWaitTimeMs);
    }

    private Dictionary<string, List<Message<string, string>>> InitializeTopicBatches(int batchSize)
    {
        return new Dictionary<string, List<Message<string, string>>>
        {
            [_tradeDataTopic] = new(batchSize),
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

        while (ShouldContinueCollectingMessages(messageCount,
                   batchConfig.batchSize,
                   startTime,
                   firstMessageTime,
                   batchConfig.batchWindowMs,
                   batchConfig.maxWaitTimeMs))
        {
            // Calculate timeout for the next read operation
            var timeout = CalculateTimeout(firstMessageTime, batchConfig.maxWaitTimeMs, batchConfig.batchWindowMs);
            if (timeout <= 0)
                break;

            // Try to read the next message with timeout
            var message = await TryReadMessageWithTimeoutAsync(timeout, cancellationToken);
            if (message == null)
                break;

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

            // ReSharper disable once InlineTemporaryVariable
            var messagesToSend = messages;
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
            const int maxRetries = 3;
            const int retryDelay = 1000;
            var retryCount = 0;

            while (retryCount < maxRetries)
            {
                try
                {
                    await _kafkaProducer.ProduceBatchAsync(topic, messagesToSend, cancellationToken);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to send batch to topic {Topic}. Retry {RetryCount}/{MaxRetries} in {Delay}ms",
                        topic, retryCount, maxRetries, retryDelay);
                    await Task.Delay(retryDelay, cancellationToken);
                }

                retryCount++;
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

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class EnqueueSnapshot
{
    public string? Symbol { get; set; }
    public string? MdReqId { get; set; }
    public string? MdEntryType { get; set; }
    public string? SendingTime { get; set; }
    public string? Topic { get; set; }
    public string? MessageKey { get; set; }
    public long SequenceNumber { get; set; }
}