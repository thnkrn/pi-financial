using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Newtonsoft.Json;
using Pi.SetMarketDataWSS.Application.Helpers;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchHousekeeper;
using Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;
using Pi.SetMarketDataWSS.Application.Interfaces.StreamingResponseBuilder;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.DataSubscriber.BidOfferMapper;
using Pi.SetMarketDataWSS.DataSubscriber.Models;
using Pi.SetMarketDataWSS.DataSubscriber.Services;
using Pi.SetMarketDataWSS.Domain.ConstantConfigurations;
using Pi.SetMarketDataWSS.Domain.Entities;
using Pi.SetMarketDataWSS.Domain.Models.Kafka;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using Pi.SetMarketDataWSS.Infrastructure.Exceptions;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketDataWSS.Infrastructure.Interfaces.Redis;
using StackExchange.Redis;
using JsonException = Newtonsoft.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;
using RedisChannel = Pi.SetMarketDataWSS.Domain.Models.Redis.RedisChannel;
using RedisValue = Pi.SetMarketDataWSS.Domain.Models.Redis.RedisValue;

namespace Pi.SetMarketDataWSS.DataSubscriber.Handlers;

public sealed class KafkaMessageHandler : IKafkaMessageHandler<Message<string, string>>, IDisposable
{
    private const string ErrorDeserializingMessage = "Error deserializing message";
    private const string UnexpectedErrorMessage = "Unexpected error processing message";
    private const string ErrorMessageTemplate = "{ErrorMessage} {ExceptionMessage}";
    private const string DecimalsInPrice = "decimals-in-price-";
    private const int BoundedCapacity = 200000;
    private readonly IBidOfferService _bidOfferService;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly string _channel;
    private readonly IItchHousekeeperService _itchHousekeeperService;
    private readonly IItchMapperService _itchMapperService;
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IMarketStreamingResponseBuilder _marketStreamingResponseBuilder;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly Dictionary<string, Func<string, Timestamp, Task<bool>>> _messageHandlers;

    // Sequential ActionBlock for processing messages in order
    private readonly
        ActionBlock<(string? MessageType, string? Message, Timestamp CreationTime, TaskCompletionSource<(bool, bool)>
            ResultCompletion)> _processingBlock;

    private readonly ActionBlock<MarketStreamingResponse> _publishBlock;
    private readonly IRedisV2Publisher _redisPublisher;
    private readonly BackgroundTaskQueue _taskQueue;
    private readonly string _handleProduct;

    public KafkaMessageHandler(IRedisV2Publisher redisPublisher,
        KafkaMessageHandlerDependencies dependencies,
        IConfiguration configuration,
        ILogger<KafkaMessageHandler> logger)
    {
        _redisPublisher = redisPublisher ?? throw new ArgumentNullException(nameof(redisPublisher));
        _itchMapperService = dependencies.ItchMapperService;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _marketStreamingResponseBuilder = dependencies.MarketStreamingResponseBuilder;
        _itchHousekeeperService = dependencies.ItchHousekeeperService;
        _bidOfferService = dependencies.BidOfferService;
        _channel = configuration[ConfigurationKeys.RedisChannel] ?? string.Empty;
        _handleProduct = configuration[ConfigurationKeys.ConsumeProduct] ?? string.Empty;
        _taskQueue = dependencies.TaskQueue;
        _messageHandlers = new Dictionary<string, Func<string, Timestamp, Task<bool>>>
        {
            { nameof(ItchMessageType.i), HandleTradeTickerMessageAsync },
            { nameof(ItchMessageType.I), HandleTradeStatisticsMessageAsync },
            { nameof(ItchMessageType.R), HandleOrderBookDirectoryMessage },
            { nameof(ItchMessageType.O), HandleOrderBookStateMessageAsync },
            { nameof(ItchMessageType.b), HandleMarketByPriceMessageAsync },
            { nameof(ItchMessageType.Q), HandleReferencePriceMessageAsync },
            { nameof(ItchMessageType.J), HandleIndexPriceMessageAsync },
            { nameof(ItchMessageType.k), HandlePriceLimitMessageAsync },
            { nameof(ItchMessageType.m), HandleMarketDirectoryMessageAsync },
            { nameof(ItchMessageType.L), HandleTickSizeTableEntry },
            { nameof(ItchMessageType.h), HandleOpenInterest },
            { nameof(ItchMessageType.Z), HandleEquilibriumPrice },
            { nameof(ItchMessageType.T), HandleSecondsMessage }
        };

        // Create a sequential ActionBlock with EnsureOrdered=true to guarantee message ordering
        _processingBlock =
            new ActionBlock<(string? MessageType, string? Message, Timestamp CreationTime,
                TaskCompletionSource<(bool, bool)>
                ResultCompletion)>(
                async item =>
                {
                    try
                    {
                        var result = await HandleAsyncService(item.MessageType, item.Message, item.CreationTime);
                        item.ResultCompletion.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message type {MessageType}", item.MessageType);
                        item.ResultCompletion.SetResult((false, false));
                    }
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    BoundedCapacity = BoundedCapacity,
                    EnsureOrdered = true,
                    SingleProducerConstrained = true,
                    CancellationToken = _cancellationTokenSource.Token
                });

        _publishBlock = new ActionBlock<MarketStreamingResponse>(
            async response =>
            {
                try
                {
                    await _redisPublisher.PublishAsync(_channel, response, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing response");
                }
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = BoundedCapacity,
                SingleProducerConstrained = true,
                EnsureOrdered = false,
                CancellationToken = _cancellationTokenSource.Token
            });
    }

    public void Dispose()
    {
        try
        {
            // Signal completion to blocks
            _processingBlock.Complete();
            _publishBlock.Complete();

            // Try to wait for a short time for processing to complete
            Task.WaitAll([_processingBlock.Completion, _publishBlock.Completion],
                TimeSpan.FromMinutes(20));

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing KafkaMessageHandler");
        }
    }

    public async Task<(bool processed, bool storedInRedis)> HandleAsync(Message<string, string> message)
    {
        if (string.IsNullOrEmpty(message.Value))
        {
            _logger.LogWarning("Received empty message");
            return (false, false);
        }

        if (_handleProduct.Equals("Derivative", StringComparison.CurrentCultureIgnoreCase) &&
            message.Key.Contains("Derivative", StringComparison.CurrentCultureIgnoreCase))
        {
            try
            {
                var orderBookId = message.Key.Split("_").FirstOrDefault();
                {
                    var cacheKey = $"{CacheKey.StreamingBody}{orderBookId}";
                    var cachedStreamingBody = await GetCacheAsync(cacheKey);
                    if (cachedStreamingBody != null)
                    {
                        try
                        {
                            var streamingResponse =
                                JsonSerializer.Deserialize<MarketStreamingResponse>(cachedStreamingBody, JsonOptions);

                            var venue = streamingResponse?.Response.Data?.FirstOrDefault()?.Venue;

                            if (venue?.Equals("Equity", StringComparison.CurrentCultureIgnoreCase) ?? false)
                            {
                                _logger.LogInformation(
                                    "Skipping message {Key} because it is from equity venue. OrderBookId: {OrderBookId}, Venue: {Venue}",
                                    message.Key, orderBookId, venue);
                                return (true, true); // Skip when venue is equity
                            }
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogError(ex,
                                "Error deserializing cached streaming body for orderBookId: {OrderBookId}",
                                orderBookId);
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No cached streaming body found for orderBookId: {OrderBookId}", orderBookId);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error to exclude {Key}", message.Key);
            }
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            if (!message.Value.IsValidJsonMessage())
            {
                _logger.LogWarning("The message cannot be deserialized because it is invalid");
                return (false, false);
            }

            var cleanMessage = message.Value.SimpleCleanJsonMessage();
            _logger.LogDebug("Received the message: {CleanMessage}", cleanMessage);

            var stockMessage = JsonConvert.DeserializeObject<StockMessage>(cleanMessage);
            if (stockMessage == null || string.IsNullOrEmpty(stockMessage.Message))
            {
                _logger.LogWarning("Unable to deserialize message or message is null");
                return (false, false);
            }

            // Create completion source to track the result with both processing and storage status
            var resultCompletion = new TaskCompletionSource<(bool Processed, bool StoredInRedis)>();

            // Post to processing block with result tracking
            var success = _processingBlock.Post((stockMessage.MessageType, stockMessage.Message, message.Timestamp,
                resultCompletion));

            if (success)
                // Wait for the processing to complete and return its result
                return await resultCompletion.Task;
            else
                try
                {
                    // Fallback to direct processing when the block is full
                    return await HandleAsyncService(
                        stockMessage.MessageType,
                        stockMessage.Message,
                        message.Timestamp
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message directly");
                    return (false, false);
                }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return (false, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            return (false, false);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("HandleAsync completed in {ElapsedMilliseconds} ms",
                stopwatch.ElapsedMilliseconds);
        }
    }

    private async Task<(bool Processed, bool StoredInRedis)> HandleAsyncService(string? messageType, string? message,
        Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(messageType) || string.IsNullOrEmpty(message))
        {
            _logger.LogWarning("Received empty message type or message");
            return (false, false);
        }

        if (_messageHandlers.TryGetValue(messageType, out var handler))
            try
            {
                var processed = await handler(message, creationTime);

                // Assume if processing succeeded, storage also succeeded
                // In a more rigorous implementation, we could track Redis operations separately
                return (processed, processed);
            }
            catch (RedisException ex)
            {
                // Processing succeeded but Redis storage failed
                _logger.LogError(ex, "Redis error while processing message type {MessageType}: {Message}",
                    messageType, ex.Message);
                return (true, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message type {MessageType}: {Message}",
                    messageType, ex.Message);
                return (false, false);
            }

        _logger.LogWarning("Received an unhandled message: {MessageType}", messageType);
        return (false, false);
    }

    [SuppressMessage("SonarQube", "S1172")]
    private static async Task<bool> HandleSecondsMessage(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        await Task.CompletedTask;
        return true;
    }

    private async Task<bool> HandleMarketDirectoryMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var marketDirectoryMessage =
                JsonConvert.DeserializeObject<MarketDirectoryMessageWrapper>(message);

            if (marketDirectoryMessage == null)
            {
                _logger.LogWarning("Received null market directory message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (marketDirectoryMessage.Metadata != null)
            {
                timestamp = marketDirectoryMessage.Metadata.Timestamp;
                sequenceNumber = marketDirectoryMessage.Metadata.SequenceNumber;
            }

            var messageType = marketDirectoryMessage.MsgType.ToString();
            var marketDirectoryKey =
                $"{CacheKey.MarketDirectory}{marketDirectoryMessage.MarketCode?.Value.ToString()}";

            var marketDirectoryCache = await GetCacheAsync(marketDirectoryKey);
            var marketDirectoryMessageResult = _itchMapperService.MapToDataCache(
                marketDirectoryMessage,
                new Dictionary<string, string>
                {
                    { CacheKey.MarketDirectory, marketDirectoryCache ?? string.Empty }
                }
            );

            if (marketDirectoryMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = marketDirectoryMessageResult.RedisValue,
                    RedisChannel = marketDirectoryMessageResult.RedisChannel,
                    CachingKey = marketDirectoryMessage.MarketCode?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleTradeTickerMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var tradeTickerMessage = JsonConvert.DeserializeObject<TradeTickerMessageWrapper>(
                message
            );

            if (tradeTickerMessage == null)
            {
                _logger.LogWarning("Received null trade ticker message: {Message}", message);
                return false;
            }

            if (tradeTickerMessage.DealSource?.Value == 3)
            {
                return true;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (tradeTickerMessage.Metadata != null)
            {
                timestamp = tradeTickerMessage.Metadata.Timestamp;
                sequenceNumber = tradeTickerMessage.Metadata.SequenceNumber;
            }

            var messageType = tradeTickerMessage.MsgType.ToString();
            var messageTypeQUpper = ItchMessageType.Q.ToString();
            var priceInfoILowerKey =
                $"{CacheKey.PriceInfo}{messageType}-{tradeTickerMessage.OrderbookId?.Value.ToString()}";
            var priceInfoQUpperKey =
                $"{CacheKey.PriceInfo}{messageTypeQUpper}-{tradeTickerMessage.OrderbookId?.Value.ToString()}";
            var publicTradeKey =
                $"{CacheKey.PublicTrade}{tradeTickerMessage.OrderbookId?.Value.ToString()}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { priceInfoILowerKey, true },
                { priceInfoQUpperKey, true },
                { publicTradeKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var priceInfoILowerCache = batchCache[priceInfoILowerKey];
            var priceInfoQUpperCache = batchCache[priceInfoQUpperKey];
            var publicTradeCache = batchCache[publicTradeKey];
            var tradeTickerMessageResult = _itchMapperService.MapToDataCache(
                tradeTickerMessage,
                new Dictionary<string, string>
                {
                    { $"{CacheKey.PriceInfo}{messageType}-", priceInfoILowerCache ?? string.Empty },
                    { $"{CacheKey.PriceInfo}{messageTypeQUpper}-", priceInfoQUpperCache ?? string.Empty },
                    { CacheKey.PublicTrade, publicTradeCache ?? string.Empty }
                }
            );

            if (tradeTickerMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = tradeTickerMessageResult.RedisValue,
                    RedisChannel = tradeTickerMessageResult.RedisChannel,
                    CachingKey = tradeTickerMessage.OrderbookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleTradeStatisticsMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var tradeStatisticsMessage =
                JsonConvert.DeserializeObject<TradeStatisticsMessageWrapper>(message);

            if (tradeStatisticsMessage == null)
            {
                _logger.LogWarning("Received null trade statistics message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (tradeStatisticsMessage.Metadata != null)
            {
                timestamp = tradeStatisticsMessage.Metadata.Timestamp;
                sequenceNumber = tradeStatisticsMessage.Metadata.SequenceNumber;
            }

            var messageType = tradeStatisticsMessage.MsgType.ToString();
            var priceInfoKey =
                $"{CacheKey.PriceInfo}{messageType}-{tradeStatisticsMessage.OrderBookId?.Value.ToString()}";
            var marketStatusKey =
                $"{CacheKey.MarketStatus}{tradeStatisticsMessage.OrderBookId?.Value.ToString()}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { priceInfoKey, true },
                { marketStatusKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var tradeStatisticsMessageCache = batchCache[priceInfoKey];
            var orderBookStateMessageCache = batchCache[marketStatusKey];
            var tradeStatisticsMessageResult = _itchMapperService.MapToDataCache(
                tradeStatisticsMessage,
                new Dictionary<string, string>
                {
                    { $"{CacheKey.PriceInfo}{messageType}-", tradeStatisticsMessageCache ?? string.Empty },
                    { CacheKey.MarketStatus, orderBookStateMessageCache ?? string.Empty }
                }
            );

            if (tradeStatisticsMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = tradeStatisticsMessageResult.RedisValue,
                    RedisChannel = tradeStatisticsMessageResult.RedisChannel,
                    CachingKey = tradeStatisticsMessage.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleOrderBookDirectoryMessage(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var orderBookDirectoryMessage =
                JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(message);

            if (orderBookDirectoryMessage == null)
            {
                _logger.LogWarning("Received null order book directory message: {Message}", message);
                return false;
            }

            try
            {
                var decimalsInPriceKey = $"{DecimalsInPrice}{orderBookDirectoryMessage.OrderBookID?.Value.ToString()}";

                // Fire and forget pattern
                await _taskQueue.QueueTaskAsync(async _ =>
                {
                    // Set decimals in price to cache
                    var decimalsInPriceValue = orderBookDirectoryMessage.DecimalsInPrice is { Value: > 0 }
                        ? orderBookDirectoryMessage.DecimalsInPrice.Value
                        : 0;
                    await _redisPublisher.SetAsync(decimalsInPriceKey, decimalsInPriceValue);
                });
            }
            catch
            {
                // Nothing to do
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (orderBookDirectoryMessage.Metadata != null)
            {
                timestamp = orderBookDirectoryMessage.Metadata.Timestamp;
                sequenceNumber = orderBookDirectoryMessage.Metadata.SequenceNumber;
            }

            var messageType = orderBookDirectoryMessage.MsgType.ToString();
            var priceInfoKey =
                $"{CacheKey.PriceInfo}{orderBookDirectoryMessage.OrderBookID?.Value.ToString()}";
            var orderBookKey =
                $"{CacheKey.OrderBook}{orderBookDirectoryMessage.OrderBookID?.Value.ToString()}";
            var instrumentDetailKey =
                $"{CacheKey.InstrumentDetail}{orderBookDirectoryMessage.OrderBookID?.Value.ToString()}";
            var underlyingPriceInfoKey =
                $"{CacheKey.PriceInfo}{orderBookDirectoryMessage.UnderlyingOrderBookID?.Value.ToString()}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { priceInfoKey, true },
                { underlyingPriceInfoKey, true },
                { orderBookKey, false },
                { instrumentDetailKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var orderBookDirectoryForPriceInfoMessageCache = batchCache[priceInfoKey];
            var orderBookDirectoryForOrderBookMessageCache = batchCache[orderBookKey];
            var orderBookDirectoryForUnderlyingPriceInfoCache = batchCache[underlyingPriceInfoKey];
            var orderBookDirectoryForInstrumentDetailMessageCache = batchCache[instrumentDetailKey];
            var orderBookDirectoryMessageResult = _itchMapperService.MapToDataCache(
                orderBookDirectoryMessage,
                new Dictionary<string, string>
                {
                    {
                        CacheKey.PriceInfo,
                        orderBookDirectoryForPriceInfoMessageCache ?? string.Empty
                    },
                    {
                        CacheKey.OrderBook,
                        orderBookDirectoryForOrderBookMessageCache ?? string.Empty
                    },
                    {
                        CacheKey.InstrumentDetail,
                        orderBookDirectoryForInstrumentDetailMessageCache ?? string.Empty
                    },
                    {
                        $"{CacheKey.PriceInfo}{orderBookDirectoryMessage.UnderlyingOrderBookID}",
                        orderBookDirectoryForUnderlyingPriceInfoCache ?? string.Empty
                    }
                }
            );

            if (orderBookDirectoryMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = orderBookDirectoryMessageResult.RedisValue,
                    RedisChannel = orderBookDirectoryMessageResult.RedisChannel,
                    CachingKey = orderBookDirectoryMessage.OrderBookID?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleOrderBookStateMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var orderBookStateMessage = JsonConvert.DeserializeObject<OrderBookStateMessageWrapper>(
                message
            );

            if (orderBookStateMessage == null)
            {
                _logger.LogWarning("Received null order book state message: {Message}", message);
                return false;
            }

            // Fire and forget pattern
            await _taskQueue.QueueTaskAsync(async _ =>
            {
                // If the app gets a reset stat message from orderBookStateMessage
                // and needs to reset the stat, call Itch Housekeeper Service.
                await HandleOrderBookStateMessageResetStatAsync(orderBookStateMessage, creationTime);
                await _bidOfferService.ResetBidOfferAsync(orderBookStateMessage);
            });

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (orderBookStateMessage.Metadata != null)
            {
                timestamp = orderBookStateMessage.Metadata.Timestamp;
                sequenceNumber = orderBookStateMessage.Metadata.SequenceNumber;
            }

            var messageType = orderBookStateMessage.MsgType.ToString();
            var marketStatusKey =
                $"{CacheKey.MarketStatus}{orderBookStateMessage.OrderBookId?.Value.ToString()}";
            var instrumentDetailKey =
                $"{CacheKey.InstrumentDetail}{orderBookStateMessage.OrderBookId?.Value.ToString()}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { marketStatusKey, false },
                { instrumentDetailKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var orderBookStateMessageCache = batchCache[marketStatusKey];
            var orderBookDirectoryForInstrumentDetailMessageCache = batchCache[instrumentDetailKey];
            var orderBookStateMessageResult = _itchMapperService.MapToDataCache(
                orderBookStateMessage,
                new Dictionary<string, string>
                {
                    { CacheKey.MarketStatus, orderBookStateMessageCache ?? string.Empty },
                    {
                        CacheKey.InstrumentDetail,
                        orderBookDirectoryForInstrumentDetailMessageCache ?? string.Empty
                    }
                }
            );

            if (orderBookStateMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = orderBookStateMessageResult.RedisValue,
                    RedisChannel = orderBookStateMessageResult.RedisChannel,
                    CachingKey = orderBookStateMessage.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    // MarketByPriceMessage - (b)
    private async Task<bool> HandleMarketByPriceMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var marketByPriceMessage = JsonConvert.DeserializeObject<MarketByPriceLevelWrapper>(
                message
            );

            if (marketByPriceMessage == null)
            {
                _logger.LogWarning("Received null market by price message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (marketByPriceMessage.Metadata != null)
            {
                timestamp = marketByPriceMessage.Metadata.Timestamp;
                sequenceNumber = marketByPriceMessage.Metadata.SequenceNumber;
            }

            var decimalsInPriceValue = 5;
            try
            {
                // Get decimals in price from cache
                var decimalsInPriceKey = $"{DecimalsInPrice}{marketByPriceMessage.OrderBookID.Value.ToString()}";
                var decimalsInPriceCache = await _redisPublisher.GetAsync<int>(decimalsInPriceKey);
                if (decimalsInPriceCache > 0)
                    decimalsInPriceValue = decimalsInPriceCache;
            }
            catch
            {
                // Nothing to do
            }

            var messageType = marketByPriceMessage.MsgType.ToString();
            var symbol = marketByPriceMessage.OrderBookID.Value.ToString();
            var sequence = long.Parse(marketByPriceMessage.Metadata?.SequenceNumber.ToString() ?? "0");
            var timestampNanoseconds = marketByPriceMessage.Metadata?.Timestamp ?? 0;

            try
            {
                // Fire and forget pattern
                await _taskQueue.QueueTaskAsync(async _ =>
                {
                    await _bidOfferService.AddBidOfferItemsFromPriceLevelUpdates(
                        symbol,
                        sequence,
                        timestampNanoseconds,
                        marketByPriceMessage.PriceLevelUpdates,
                        decimalsInPriceValue
                    );
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            }

            var orderBookKey = $"{CacheKey.OrderBook}{symbol}";
            var marketByPriceMessageCache = await GetCacheAsync(orderBookKey);
            var marketByPriceMessageResult = _itchMapperService.MapToDataCache(
                marketByPriceMessage,
                new Dictionary<string, string>
                {
                    { CacheKey.OrderBook, marketByPriceMessageCache ?? string.Empty }
                }
            );

            if (marketByPriceMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = marketByPriceMessageResult.RedisValue,
                    RedisChannel = marketByPriceMessageResult.RedisChannel,
                    CachingKey = symbol,
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper, true);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleReferencePriceMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            // Parse to implemented referencePriceMessage
            var referencePriceMessage = JsonConvert.DeserializeObject<ReferencePriceMessageWrapper>(
                message
            );

            if (referencePriceMessage == null)
            {
                _logger.LogWarning("Received null reference price message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (referencePriceMessage.Metadata != null)
            {
                timestamp = referencePriceMessage.Metadata.Timestamp;
                sequenceNumber = referencePriceMessage.Metadata.SequenceNumber;
            }

            var messageType = referencePriceMessage.MsgType.ToString();
            var priceInfoKey =
                $"{CacheKey.PriceInfo}{messageType}-{referencePriceMessage.OrderBookId?.Value.ToString()}";

            // Get redis cached msg
            var priceInfoCache = await GetCacheAsync(priceInfoKey);

            // Do mapping
            var referencePriceMessageResult = _itchMapperService.MapToDataCache(
                referencePriceMessage,
                new Dictionary<string, string>
                {
                    { $"{CacheKey.PriceInfo}{messageType}-", priceInfoCache ?? string.Empty }
                }
            );

            // Publish to redis
            if (referencePriceMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = referencePriceMessageResult.RedisValue,
                    RedisChannel = referencePriceMessageResult.RedisChannel,
                    CachingKey = referencePriceMessage.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleIndexPriceMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            // Parse to implemented referencePriceMessage
            var indexPriceMessage = JsonConvert.DeserializeObject<IndexPriceMessageWrapper>(
                message
            );

            if (indexPriceMessage == null)
            {
                _logger.LogWarning("Received null index price message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (indexPriceMessage.Metadata != null)
            {
                timestamp = indexPriceMessage.Metadata.Timestamp;
                sequenceNumber = indexPriceMessage.Metadata.SequenceNumber;
            }

            var messageType = indexPriceMessage.MsgType.ToString();
            var priceInfoKey =
                $"{CacheKey.PriceInfo}{messageType}-{indexPriceMessage.OrderBookId?.Value.ToString()}";

            // Get redis cached msg
            var priceInfoCache = await GetCacheAsync(priceInfoKey);

            // Do mapping
            var indexPriceMessageResult = _itchMapperService.MapToDataCache(
                indexPriceMessage,
                new Dictionary<string, string>
                {
                    { $"{CacheKey.PriceInfo}{messageType}-", priceInfoCache ?? string.Empty }
                }
            );

            // Publish to redis
            if (indexPriceMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = indexPriceMessageResult.RedisValue,
                    RedisChannel = indexPriceMessageResult.RedisChannel,
                    CachingKey = indexPriceMessage.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandlePriceLimitMessageAsync(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            // Parse to implemented priceLimitMessage
            var priceLimitMessage = JsonConvert.DeserializeObject<PriceLimitMessageWrapper>(
                message
            );

            if (priceLimitMessage == null)
            {
                _logger.LogWarning("Received null price limit message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (priceLimitMessage.Metadata != null)
            {
                timestamp = priceLimitMessage.Metadata.Timestamp;
                sequenceNumber = priceLimitMessage.Metadata.SequenceNumber;
            }

            var messageType = priceLimitMessage.MsgType.ToString();
            var priceInfoKey =
                $"{CacheKey.PriceInfo}{messageType}-{priceLimitMessage.OrderbookId?.Value.ToString()}";

            // Get redis cached msg
            var priceInfoCache = await GetCacheAsync(priceInfoKey);

            // Do mapping
            var priceLimitMessageResult = _itchMapperService.MapToDataCache(
                priceLimitMessage,
                new Dictionary<string, string>
                {
                    { $"{CacheKey.PriceInfo}{messageType}-", priceInfoCache ?? string.Empty }
                }
            );

            // Publish to redis
            if (priceLimitMessageResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = priceLimitMessageResult.RedisValue,
                    RedisChannel = priceLimitMessageResult.RedisChannel,
                    CachingKey = priceLimitMessage.OrderbookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleTickSizeTableEntry(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var tickSizeTableEntry = JsonConvert.DeserializeObject<TickSizeTableMessageWrapper>(message);
            if (tickSizeTableEntry == null)
            {
                _logger.LogWarning("Received null tick size table entry message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (tickSizeTableEntry.Metadata != null)
            {
                timestamp = tickSizeTableEntry.Metadata.Timestamp;
                sequenceNumber = tickSizeTableEntry.Metadata.SequenceNumber;
            }

            var tickSizeTableEntryKey =
                $"{CacheKey.TickSizeTableEntry}{tickSizeTableEntry.OrderBookId?.Value.ToString()}";
            var instrumentDetailKey = $"{CacheKey.InstrumentDetail}{tickSizeTableEntry.OrderBookId?.Value.ToString()}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { tickSizeTableEntryKey, false },
                { instrumentDetailKey, false }
            };

            var messageType = tickSizeTableEntry.MsgType.ToString();
            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var tickSizeTableEntryCache = batchCache[tickSizeTableEntryKey];
            var instrumentDetailCache = batchCache[instrumentDetailKey];
            var tickSizeTableEntryResult = _itchMapperService.MapToDataCache(
                tickSizeTableEntry,
                new Dictionary<string, string>
                {
                    { CacheKey.TickSizeTableEntry, tickSizeTableEntryCache ?? string.Empty },
                    { CacheKey.InstrumentDetail, instrumentDetailCache ?? string.Empty }
                }
            );

            // Publish to redis
            if (tickSizeTableEntryResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = tickSizeTableEntryResult.RedisValue,
                    RedisChannel = tickSizeTableEntryResult.RedisChannel,
                    CachingKey = tickSizeTableEntry.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleOpenInterest(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var openInterest = JsonConvert.DeserializeObject<OpenInterestMessageWrapper>(message);
            if (openInterest == null)
            {
                _logger.LogWarning("Received null open interest message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (openInterest.Metadata != null)
            {
                timestamp = openInterest.Metadata.Timestamp;
                sequenceNumber = openInterest.Metadata.SequenceNumber;
            }

            var messageType = openInterest.MsgType.ToString();
            var streamingBodyKey = $"{CacheKey.StreamingBody}{openInterest.OrderBookId?.Value.ToString()}";
            var cachedStreamingBody = await GetCacheAsync(streamingBodyKey);
            var openInterestMappingResult = _itchMapperService.MapToDataCache(
                openInterest,
                new Dictionary<string, string>
                {
                    { CacheKey.StreamingBody, cachedStreamingBody ?? string.Empty }
                });

            // Cache to redis
            if (openInterestMappingResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = openInterestMappingResult.RedisValue,
                    RedisChannel = openInterestMappingResult.RedisChannel,
                    CachingKey = openInterest.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task<bool> HandleEquilibriumPrice(string message, Timestamp creationTime)
    {
        if (string.IsNullOrEmpty(message))
            return false;

        try
        {
            var equilibriumPriceMessage = JsonConvert.DeserializeObject<EquilibriumPriceMessageWrapper>(message);
            if (equilibriumPriceMessage == null)
            {
                _logger.LogWarning("Received null open interest message: {Message}", message);
                return false;
            }

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (equilibriumPriceMessage.Metadata != null)
            {
                timestamp = equilibriumPriceMessage.Metadata.Timestamp;
                sequenceNumber = equilibriumPriceMessage.Metadata.SequenceNumber;
            }

            var messageType = equilibriumPriceMessage.MsgType.ToString();
            var priceInfoKey = $"{CacheKey.PriceInfo}{messageType}-{equilibriumPriceMessage.OrderBookId?.Value}";
            var marketStatusKey = $"{CacheKey.MarketStatus}{equilibriumPriceMessage.OrderBookId?.Value}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { priceInfoKey, true },
                { marketStatusKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var cachePriceInfo = batchCache[priceInfoKey];
            var cacheMarketStatus = batchCache[marketStatusKey];
            var result = _itchMapperService.MapToDataCache(
                equilibriumPriceMessage,
                new Dictionary<string, string>
                {
                    { $"{CacheKey.PriceInfo}{messageType}-", cachePriceInfo ?? string.Empty },
                    { CacheKey.MarketStatus, cacheMarketStatus ?? string.Empty }
                });

            if (result != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = result.RedisValue,
                    RedisChannel = result.RedisChannel,
                    CachingKey = equilibriumPriceMessage.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                return await PublishResultAsync(variableWrapper);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, ErrorDeserializingMessage, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
            return false;
        }

        return false;
    }

    private async Task HandleOrderBookStateMessageResetStatAsync(OrderBookStateMessageWrapper messageWrapper,
        Timestamp creationTime)
    {
        try
        {
            var messageType = messageWrapper.MsgType.ToString();
            var priceInfoKey = $"{CacheKey.PriceInfo}{messageWrapper.OrderBookId?.Value.ToString()}";
            var streamingBodyKey = $"{CacheKey.StreamingBody}{messageWrapper.OrderBookId?.Value.ToString()}";
            var publicTradeKey = $"{CacheKey.PublicTrade}{messageWrapper.OrderBookId?.Value.ToString()}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { priceInfoKey, true },
                { streamingBodyKey, false },
                { publicTradeKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var priceInfoKeyCache = batchCache[priceInfoKey];
            var streamingBodyCache = batchCache[streamingBodyKey];
            var publicTradeCache = batchCache[publicTradeKey];
            var resetStatResult = _itchHousekeeperService.ResetStat(messageWrapper,
                new Dictionary<string, string>
                {
                    { CacheKey.PriceInfo, priceInfoKeyCache ?? string.Empty },
                    { CacheKey.StreamingBody, streamingBodyCache ?? string.Empty },
                    { CacheKey.PublicTrade, publicTradeCache ?? string.Empty }
                }
            );

            long timestamp = 0;
            ulong sequenceNumber = 0;

            if (messageWrapper.Metadata != null)
            {
                timestamp = messageWrapper.Metadata.Timestamp;
                sequenceNumber = messageWrapper.Metadata.SequenceNumber;
            }

            if (resetStatResult != null)
            {
                var variableWrapper = new VariableWrapper
                {
                    RedisValue = resetStatResult.RedisValue,
                    RedisChannel = resetStatResult.RedisChannel,
                    CachingKey = messageWrapper.OrderBookId?.Value.ToString(),
                    Timestamp = timestamp,
                    SequenceNumber = sequenceNumber,
                    CreationTime = creationTime,
                    MessageType = messageType
                };

                await PublishResultAsync(variableWrapper);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessageTemplate, UnexpectedErrorMessage, ex.Message);
        }
    }

    private async Task<bool> PublishResultAsync(
        VariableWrapper variableWrapper,
        bool isBidOfferMessage = false)
    {
        if (string.IsNullOrEmpty(variableWrapper.CachingKey))
            return false;

        if (variableWrapper.RedisValue == null || variableWrapper.RedisValue.Length < 1)
        {
            _logger.LogWarning(
                "No Redis values to publish or store for CachingKey: {CachingKey}",
                variableWrapper.CachingKey
            );
            return false;
        }

        try
        {
            // Track Redis operations success
            var redisSuccess = await SetCacheAsync(variableWrapper.RedisValue, variableWrapper.CachingKey);

            if (variableWrapper.RedisChannel.Equals(RedisChannel.PubSubCache) &&
                !string.IsNullOrEmpty(variableWrapper.CachingKey))
            {
                var streaming = await BuildMarketStreamingResponse(variableWrapper.CachingKey,
                    variableWrapper.MessageType ?? string.Empty);

                if (streaming is { Response.Data.Count: > 0 }
                    && !string.IsNullOrEmpty(streaming.Response.Data[0].Symbol))
                {
                    await BidOfferMessageHandlerAsync(variableWrapper.CachingKey, streaming, isBidOfferMessage);

                    // Add streaming properties
                    streaming.SequenceNumber = variableWrapper.SequenceNumber;
                    streaming.SendingTime = variableWrapper.Timestamp.ToString();
                    streaming.ProcessingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff");
                    streaming.CreationTime = variableWrapper.CreationTime.UtcDateTime.ToString("yyyyMMdd-HH:mm:ss.fff");
                    streaming.SendingId = Guid.NewGuid().ToString("N");

                    var publishSuccess = await PublishAsync(variableWrapper.CachingKey, streaming);

                    return redisSuccess && publishSuccess;
                }
            }

            // Message was processed but no streaming data was published
            return redisSuccess;
        }
        catch (RedisException ex)
        {
            _logger.LogError(
                ex,
                "Redis error while publishing or storing message for CachingKey: {CachingKey}",
                variableWrapper.CachingKey
            );
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error publishing or storing message for CachingKey: {CachingKey}",
                variableWrapper.CachingKey
            );
            return false;
        }
    }

    private async Task<bool> PublishAsync(string cachingKey, MarketStreamingResponse streaming)
    {
        try
        {
            var success = _publishBlock.Post(streaming);
            if (!success)
                try
                {
                    // Fallback to direct publishing when the block is full
                    await _redisPublisher.PublishAsync(_channel, streaming, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing response directly");
                    return false;
                }

            // Fire and forget pattern
            await _taskQueue.QueueTaskAsync(async _ => { await SetStreamingCache(cachingKey, streaming); });
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in PublishAsync for key {CachingKey}", cachingKey);
            return false;
        }
    }

    private async Task<bool> SetCacheAsync(RedisValue[] redisValue, string cachingKey)
    {
        if (redisValue is not { Length: > 0 })
            return true;

        var allSuccess = true;

        foreach (var item in redisValue)
            try
            {
                var key = $"{item.Key}{cachingKey}";
                var compress = key.StartsWith(CacheKey.PriceInfo);
                var value = JsonConvert.SerializeObject(item.Value);
                await _redisPublisher.SetAsync(key, value, compress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set Redis key {Key} for cachingKey {CachingKey}",
                    item.Key, cachingKey);
                allSuccess = false;
            }

        return allSuccess;
    }

    private async Task<string?> GetCacheAsync(string key)
    {
        try
        {
            var compress = key.StartsWith(CacheKey.PriceInfo);
            return await _redisPublisher.GetAsync<string>(key, compress);
        }
        catch (CompressionException)
        {
            await _redisPublisher.RemoveAsync(key);
        }

        return string.Empty;
    }

    private async Task BidOfferMessageHandlerAsync(string cachingKey, MarketStreamingResponse? streaming,
        bool isBidOfferMessage)
    {
        try
        {
            var bidOffer = isBidOfferMessage
                ? await _bidOfferService.GetLatestBidOfferArray(cachingKey)
                : await _bidOfferService.GetLatestBidOfferArrayCache(cachingKey);

            if (streaming?.Response.Data != null)
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < streaming.Response.Data.Count; i++)
                {
                    var streamingOrderBook = streaming.Response.Data[i].OrderBook;
                    if (streamingOrderBook != null)
                    {
                        if (bidOffer.Bids.Count > 0)
                            // ReSharper disable once UseCollectionExpression
                            streamingOrderBook.Bid = new List<List<string>>(bidOffer.Bids);

                        if (bidOffer.Offers.Count > 0)
                            // ReSharper disable once UseCollectionExpression
                            streamingOrderBook.Offer = new List<List<string>>(bidOffer.Offers);
                    }
                }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting bid/offer for {OrderBookId}", cachingKey);
        }
    }

    private async Task SetStreamingCache(string cachingKey, MarketStreamingResponse streaming)
    {
        var streamingBodyKey = $"{CacheKey.StreamingBody}{cachingKey}";
        var priceCacheKey = $"{CacheKey.PriceStreamingBody}{cachingKey}";

        try
        {
            var streamingBodyValue = JsonConvert.SerializeObject(streaming);
            await _redisPublisher.SetAsync(streamingBodyKey, streamingBodyValue);

            if (streaming.Response.Data is { Count: > 0 })
            {
                var data = streaming.Response.Data[0];
                var priceBody = new PriceResponse
                {
                    Price = data.Price ?? "0.00",
                    PriceChanged = data.PriceChanged,
                    PriceChangedRate = data.PriceChangedRate,
                    TotalVolume = data.TotalVolume,
                    TotalAmount = data.TotalAmount
                };

                await _redisPublisher.SetStringAsync(priceCacheKey, priceBody);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while setting price cache for {OrderBookId}", cachingKey);
        }
    }

    private async Task<MarketStreamingResponse?> BuildMarketStreamingResponse(string orderBookId, string messageType)
    {
        if (string.IsNullOrEmpty(orderBookId))
            throw new ArgumentNullException(nameof(orderBookId));

        try
        {
            var keys = new Dictionary<string, bool>
            {
                { $"{CacheKey.PriceInfo}{orderBookId}", true },
                { $"{CacheKey.PriceInfo}{messageType}-{orderBookId}", true },
                { $"{CacheKey.OrderBook}{orderBookId}", false },
                { $"{CacheKey.PublicTrade}{orderBookId}", false },
                { $"{CacheKey.MarketStatus}{orderBookId}", false },
                { $"{CacheKey.InstrumentDetail}{orderBookId}", false },
                { $"{CacheKey.OpenInterest}{orderBookId}", false },
                { $"{CacheKey.StreamingBody}{orderBookId}", false }
            };

            var batchCached = await _redisPublisher.GetManyAsync<string>(keys);
            var originalPriceInfoCached = batchCached[$"{CacheKey.PriceInfo}{orderBookId}"];
            var priceInfoCached = batchCached[$"{CacheKey.PriceInfo}{messageType}-{orderBookId}"];
            var marketByPriceCached = batchCached[$"{CacheKey.OrderBook}{orderBookId}"];
            var publicTradeCached = batchCached[$"{CacheKey.PublicTrade}{orderBookId}"];
            var orderBookStateCached = batchCached[$"{CacheKey.MarketStatus}{orderBookId}"];
            var instrumentDetailCached = batchCached[$"{CacheKey.InstrumentDetail}{orderBookId}"];
            var openInterestCache = batchCached[$"{CacheKey.OpenInterest}{orderBookId}"];
            var streamingBodyCache = batchCached[$"{CacheKey.StreamingBody}{orderBookId}"];

            // Deserialize caching data to InstrumentDetail for getting MarketDirectory caching data
            var instrumentDetail =
                JsonConvert.DeserializeObject<InstrumentDetail>(instrumentDetailCached ?? string.Empty) ??
                new InstrumentDetail();

            var marketDirectoryKey = $"{CacheKey.MarketDirectory}{instrumentDetail.MarketCode}";
            var streamingBodyKey = $"{CacheKey.StreamingBody}{instrumentDetail.UnderlyingOrderBookID}";
            var batchCacheKeys = new Dictionary<string, bool>
            {
                { marketDirectoryKey, false },
                { streamingBodyKey, false }
            };

            var batchCache = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
            var marketDirectoryCached = batchCache[marketDirectoryKey];
            var decimalsInPriceKey = $"{DecimalsInPrice}{orderBookId}";
            var decimalsInPriceCached = await _redisPublisher.GetAsync<int>(decimalsInPriceKey);
            var underlyingStreamingBody = batchCache[streamingBodyKey];
            var fetchDataParams = new FetchDataParams
            {
                OriginalPriceInfoCached = originalPriceInfoCached ?? string.Empty,
                PriceInfoCached = priceInfoCached ?? string.Empty,
                MarketByPriceCached = marketByPriceCached ?? string.Empty,
                PublicTradeCached = publicTradeCached ?? string.Empty,
                OrderBookStateCached = orderBookStateCached ?? string.Empty,
                InstrumentDetailCached = instrumentDetailCached ?? string.Empty,
                MarketDirectoryCached = marketDirectoryCached ?? string.Empty,
                OpenInterestCached = openInterestCache ?? string.Empty,
                StreamingBody = streamingBodyCache ?? string.Empty,
                UnderlyingStreamingBody = underlyingStreamingBody ?? string.Empty
            };

            var builder = await _marketStreamingResponseBuilder
                .WithOrderBookId(orderBookId, messageType)
                .FetchDataAsync(fetchDataParams);

            var response = builder.Build(decimalsInPriceCached);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error building market streaming response");
        }

        return null;
    }
}