using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.GlobalMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataRealTime.Domain.Models.Fix;
using Pi.GlobalMarketDataRealTime.Infrastructure.Helpers;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

public interface IStockDataOptimizedPublisher
{
    ValueTask EnqueueMessageAsync(MarketDataSnapshotFullRefresh message);
    ValueTask DisposeAsync();
}

public class StockDataOptimizedPublisher : IStockDataOptimizedPublisher, IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly IConfiguration _configuration;
    private readonly IKafkaProducerOptimizedService _kafkaProducer;
    private readonly ILogger<StockDataOptimizedPublisher> _logger;
    private readonly Channel<SequencedMessage> _messageChannel;
    private readonly Task _processTask;
    private readonly object _sequenceLock = new();
    private long _currentSequence;
    private DateTime _lastSequenceResetTime = DateTime.UtcNow;
    
    // Topic configuration
    private readonly string _tradeDataTopic;
    private readonly HashSet<char> _tradeEntryTypes;
    
    public StockDataOptimizedPublisher(
        IConfiguration configuration,
        IKafkaProducerOptimizedService kafkaProducer,
        ILogger<StockDataOptimizedPublisher> logger)
    {
        _kafkaProducer = kafkaProducer;
        _logger = logger;
        _currentSequence = 0;

        var stockMessageServiceOptionsChannelCapacity = configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsChannelCapacity, StockMessageServiceOptions.ChannelCapacity);
        var stockMessageServiceOptionsEnableBackpressure = configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsEnableBackpressure,
            StockMessageServiceOptions.EnableBackpressure);

        _configuration = configuration;
        _messageChannel = Channel.CreateBounded<SequencedMessage>(
            new BoundedChannelOptions(stockMessageServiceOptionsChannelCapacity)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = stockMessageServiceOptionsEnableBackpressure
                    ? BoundedChannelFullMode.Wait
                    : BoundedChannelFullMode.DropOldest
            });

        // Initialize topic configuration
        _tradeDataTopic = configuration[ConfigurationKeys.KafkaTradeDataTopic] ?? "ge_stock_market_trade_data";

        // Define entry types for each topic
        _tradeEntryTypes =
        [
            MDEntryType.TRADE, // '2'
            MDEntryType.OPENING_PRICE, // '4'
            MDEntryType.CLOSING_PRICE,
            MDEntryType.BID, // '0'
            MDEntryType.OFFER, // '1'
            MDEntryType.TRADE_VOLUME
        ];

        _cancellationTokenSource = new CancellationTokenSource();
        _processTask = ProcessMessagesAsync(_cancellationTokenSource.Token);
    }

    public async ValueTask EnqueueMessageAsync(MarketDataSnapshotFullRefresh message)
    {
        try
        {
            var symbol = message.GetString(Tags.Symbol);
            var mdReqId = message.GetString(Tags.MDReqID);
            var sendingTime = message.Header.IsSetField(Tags.SendingTime)
                ? message.Header.GetString(Tags.SendingTime)
                : string.Empty;

            // Process market data entries
            var group = new MarketDataSnapshotFullRefresh.NoMDEntriesGroup();
            var allEntries = await Task.Run(() => FixListenerOptimizedHelper.ExtractMarketDataEntries(message, group));
            
            if (allEntries.Count == 0) return;

            // Group entries by topic
            var entriesByTopic = GroupEntriesByTopic(allEntries);
            
            // Create and enqueue snapshot for Trade Topic if we have entries for it
            if (entriesByTopic.TryGetValue(_tradeDataTopic, out var entriesForTradeTopic) && entriesForTradeTopic.Count > 0)
            {
                await EnqueueSnapshotAsync(symbol, mdReqId, sendingTime, entriesForTradeTopic, _tradeDataTopic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to enqueue stock message for symbol {Symbol}",
                message.GetString(Tags.Symbol));
        }
    }

    private Dictionary<string, List<MarketDataEntry>> GroupEntriesByTopic(List<MarketDataEntry> allEntries)
    {
        var result = new Dictionary<string, List<MarketDataEntry>>
        {
            { _tradeDataTopic, [] },
        };
        
        foreach (var entry in allEntries)
        {
            if (string.IsNullOrEmpty(entry.MdEntryType) || entry.MdEntryType.Length != 1) 
                continue;
                
            var entryType = entry.MdEntryType[0];
            
            if (_tradeEntryTypes.Contains(entryType))
            {
                result[_tradeDataTopic].Add(entry);
            }
        }
        
        return result;
    }

    private async Task EnqueueSnapshotAsync(string symbol, string mdReqId, string sendingTime, 
        List<MarketDataEntry> entries, string topic)
    {
        var snapshot = new MarketDataSnapshot
        {
            Symbol = symbol,
            SendingTime = sendingTime,
            MdReqId = mdReqId,
            Entries = entries
        };

        // Get next sequence with overflow protection
        var (sequence, timestamp) = GetNextSequence();

        // Create balanced key using symbol + hash
        var hash = symbol.GetHashCode() & 0x7FFFFFFF;
        var balancedKey = $"{symbol}_{hash % 16}";
        var kafkaMessage = new Message<string, string>
        {
            Key = balancedKey,
            Value = JsonConvert.SerializeObject(snapshot),
            Timestamp = new Timestamp(timestamp, TimestampType.CreateTime),
            Headers = new Headers
            {
                { "sequence", BitConverter.GetBytes(sequence) },
                { "timestamp", BitConverter.GetBytes(timestamp.Ticks) },
                { "reset_generation", BitConverter.GetBytes(_lastSequenceResetTime.Ticks) }
            }
        };

        var sequencedMessage = new SequencedMessage
        {
            Sequence = sequence,
            Timestamp = timestamp,
            ResetGeneration = _lastSequenceResetTime,
            KafkaMessage = kafkaMessage,
            Topic = topic
        };

        await _messageChannel.Writer.WriteAsync(sequencedMessage);

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(
                "Enqueued message for symbol {Symbol} with sequence {Sequence} to topic {Topic}",
                symbol,
                sequence,
                topic);
    }

#pragma warning disable CA1816
    public async ValueTask DisposeAsync()
#pragma warning restore CA1816
    {
        try
        {
            _messageChannel.Writer.Complete();
            await _cancellationTokenSource.CancelAsync();
            await _processTask;
        }
        finally
        {
            _cancellationTokenSource.Dispose();
        }
    }

    private (long Sequence, DateTime Timestamp) GetNextSequence()
    {
        lock (_sequenceLock)
        {
            var timestamp = DateTime.UtcNow;
            var currentValue = _currentSequence;
            var nextValue = currentValue + 1;

            // Check for overflow or if we're close to maxValue
            if (nextValue is < 0 or long.MaxValue)
            {
                // Log the sequence reset
                _logger.LogWarning(
                    "Sequence number reset from {CurrentValue}. Time since last reset: {TimeSinceReset}",
                    currentValue,
                    timestamp - _lastSequenceResetTime);

                nextValue = 0;
                _lastSequenceResetTime = timestamp;
            }

            _currentSequence = nextValue;
            return (nextValue, timestamp);
        }
    }

    private async Task ProcessMessagesAsync(CancellationToken cancellationToken)
    {
        // Separate message batches for each topic
        var messagesByTopic = new Dictionary<string, SortedDictionary<(long Sequence, DateTime Timestamp), Message<string, string>>>
        {
            { _tradeDataTopic, new SortedDictionary<(long Sequence, DateTime Timestamp), Message<string, string>>() },
        };
        
        var stockMessageServiceOptionsBatchWindowMs = _configuration.GetValue(
            ConfigurationKeys.MessagePublisherOptionsBatchWindowMs, StockMessageServiceOptions.BatchWindowMs);
        var batchTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(stockMessageServiceOptionsBatchWindowMs));

        try
        {
            while (!cancellationToken.IsCancellationRequested)
                await BatchProcessMessagesAsync(messagesByTopic, batchTimer, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Normal shutdown of message processing loop");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in message processing loop");
        }
    }

    private async Task BatchProcessMessagesAsync(
        Dictionary<string, SortedDictionary<(long Sequence, DateTime Timestamp), Message<string, string>>> messagesByTopic,
        PeriodicTimer batchTimer,
        CancellationToken cancellationToken)
    {
        try
        {
            var stockMessageServiceOptionsBatchSize = _configuration.GetValue(
                ConfigurationKeys.MessagePublisherOptionsBatchSize, StockMessageServiceOptions.BatchSize);

            var totalMessageCount = 0;
            
            // Try to read messages until batch is full or timer expires
            while (totalMessageCount < stockMessageServiceOptionsBatchSize * 2) // * 2 because we have two topics now
            {
                if (await _messageChannel.Reader.WaitToReadAsync(cancellationToken)
                    && _messageChannel.Reader.TryRead(out var sequencedMessage)
                    && messagesByTopic.TryGetValue(sequencedMessage.Topic, out var messages))
                {
                    messages[(sequencedMessage.Sequence, sequencedMessage.Timestamp)] = sequencedMessage.KafkaMessage;
                    totalMessageCount++;
                }

                // Check if batch timer expired
                if (!await batchTimer.WaitForNextTickAsync(cancellationToken))
                    break;
            }

            // Process each topic's message batch
            foreach (var topicEntry in messagesByTopic)
            {
                var topic = topicEntry.Key;
                var messages = topicEntry.Value;
                
                if (messages.Count > 0)
                {
                    // Ensure messages are in sequence and ordered by timestamp within same sequence generation
                    var orderedMessages = messages.Values.ToArray();

                    // Send batch to Kafka for this topic
                    await SendBatchToKafkaAsync(orderedMessages, topic, cancellationToken);

                    var firstKey = messages.Keys.First();
                    var lastKey = messages.Keys.Last();

                    _logger.LogInformation(
                        "Processed batch of {Count} messages for topic {Topic}, sequence range {StartSeq}-{EndSeq}, " +
                        "timestamp range {StartTime:yyyy-MM-dd HH:mm:ss.fff}-{EndTime:yyyy-MM-dd HH:mm:ss.fff}",
                        messages.Count,
                        topic,
                        firstKey.Sequence,
                        lastKey.Sequence,
                        firstKey.Timestamp,
                        lastKey.Timestamp);
                    
                    // Clear the processed messages for this topic
                    messages.Clear();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process batch of messages");
        }
    }
    
    private async Task SendBatchToKafkaAsync(Message<string, string>[] messages, string topic, CancellationToken cancellationToken)
    {
        try
        {
            // Create a method to produce messages to a specific topic
            await _kafkaProducer.ProduceBatchToTopicAsync(messages, topic, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send batch to Kafka topic {Topic}", topic);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }
}

// ReSharper disable UnusedAutoPropertyAccessor.Global
public class SequencedMessage
{
    public required long Sequence { get; init; }
    public required DateTime Timestamp { get; init; }
    public required DateTime ResetGeneration { get; init; }
    public required Message<string, string> KafkaMessage { get; init; }
    public required string Topic { get; init; }
}

public static class StockMessageServiceOptions
{
    public const int BatchSize = 30;
    public const int BatchWindowMs = 15;
    public const int ChannelCapacity = 100000;
    public const bool EnableBackpressure = true;
}