using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks.Dataflow;
using Confluent.Kafka;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Application.Helpers;
using Pi.GlobalMarketDataWSS.Application.Interfaces.FixMapper;
using Pi.GlobalMarketDataWSS.Application.Services.Constants;
using Pi.GlobalMarketDataWSS.Application.Services.FixMapper;
using Pi.GlobalMarketDataWSS.DataSubscriber.Constants;
using Pi.GlobalMarketDataWSS.DataSubscriber.Models;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Redis;
using Timestamp = Confluent.Kafka.Timestamp;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Handlers;

public sealed class KafkaMessageHandler : IKafkaMessageHandler<Message<string, string>>, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly string _channel;

    // Dataflow components
    private readonly ActionBlock<EntryProcessingItem> _entryProcessingBlock;
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IMarketScheduleDataService _marketScheduleService;
    private readonly MessageSamplerService _messageSamplerService;
    private readonly IPriceInfoMapperService _priceInfoMapperService;
    private readonly ActionBlock<MarketStreamingResponse> _publishBlock;
    private readonly SemaphoreSlim _publishSemaphore = new(1, 1);
    private readonly IRedisV2Publisher _redisPublisher;

    private readonly ObjectPool<StreamingBody> _streamingBodyPool =
        new DefaultObjectPool<StreamingBody>(new DefaultPooledObjectPolicy<StreamingBody>());

    private readonly BackgroundTaskQueue _taskQueue;
    private readonly IMongoService<WhiteList> _whiteListService;

    /// <summary>
    ///     Initializes a new instance of the KafkaMessageHandler class
    /// </summary>
    /// <param name="logger">Logger instance</param>
    /// <param name="redisPublisher">Redis publisher service</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="marketScheduleService">Market schedule service</param>
    /// <param name="whiteListService">Whitelist service</param>
    /// <param name="priceInfoMapperService">Price info mapper service</param>
    /// <param name="taskQueue">Background task queue</param>
    /// <param name="messageSamplerService">Message sampler</param>
    [SuppressMessage("SonarQube", "S107")]
    public KafkaMessageHandler(
        ILogger<KafkaMessageHandler> logger,
        IRedisV2Publisher redisPublisher,
        IConfiguration configuration,
        IMarketScheduleDataService marketScheduleService,
        IMongoService<WhiteList> whiteListService,
        IPriceInfoMapperService priceInfoMapperService,
        BackgroundTaskQueue taskQueue,
        MessageSamplerService messageSamplerService
    )
    {
        _logger = logger;
        _redisPublisher = redisPublisher;
        _channel = configuration[ConfigurationKeys.RedisChannel] ?? string.Empty;
        _marketScheduleService = marketScheduleService;
        _whiteListService = whiteListService;
        _priceInfoMapperService = priceInfoMapperService;
        _taskQueue = taskQueue;
        _messageSamplerService = messageSamplerService;

        // Configure dataflow options with limited concurrency to avoid overwhelming resources
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount,
            BoundedCapacity = 100000,
            CancellationToken = _cancellationTokenSource.Token
        };

        // Create the entry processing block for handling individual entries
        _entryProcessingBlock = new ActionBlock<EntryProcessingItem>(
            ProcessEntryAsync,
            executionOptions);

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
                BoundedCapacity = 100000,
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
            _entryProcessingBlock.Complete();
            _publishBlock.Complete();

            // Try to wait for a short time for processing to complete
            Task.WaitAll([_entryProcessingBlock.Completion, _publishBlock.Completion],
                TimeSpan.FromSeconds(10));

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _publishSemaphore.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing KafkaMessageHandler");
        }
    }

    public async Task HandleAsync(Message<string, string> message)
    {
        if (message.Key.Equals("health_check", StringComparison.OrdinalIgnoreCase))
            return;

        if (string.IsNullOrEmpty(message.Value))
        {
            _logger.LogWarning("Received empty message");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            if (!message.Value.IsValidJsonMessage())
            {
                _logger.LogWarning("The message cannot be deserialized because it is invalid");
                return;
            }

            var cleanMessage = message.Value.SimpleCleanJsonMessage();
            _logger.LogDebug("Received message: {CleanMessage}", cleanMessage);

            var result = FixData.FromJson(cleanMessage);
            if (result == null
                || string.IsNullOrEmpty(result.Symbol)
                || string.IsNullOrEmpty(result.MdReqId)
                || result.Entries == null)
            {
                _logger.LogWarning("The message cannot be deserialized because it is in an invalid format");
                return;
            }

            var split = result.Symbol?.Split(".") ?? [];
            var symbol = split[0];
            var venue = split.Length > 1
                ? split[1]
                : string.Empty;

            // Update venue if null or empty
            venue = await GetWhiteListVenue(symbol, venue);

            // Create a processing task for each entry
            var entries = (result.Entries ?? []).OfType<Entry>();
            var entryList = entries.ToList();
            if (entryList.Any())
            {
                // Process entries in parallel using TPL Dataflow
                var entryTasks = entryList.Select(entry => new EntryProcessingItem
                    {
                        Entry = entry,
                        Symbol = symbol,
                        Venue = venue,
                        SendingTime = result.SendingTime,
                        SequenceNumber = result.SequenceNumber,
                        MdEntryType = result.MdEntryType,
                        CreationTime = message.Timestamp
                    })
                    .Select(item => _entryProcessingBlock.SendAsync(item))
                    .Cast<Task>()
                    .ToList();

                // Wait for all entries to be queued (not processed)
                await Task.WhenAll(entryTasks);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing message");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("HandleAsync completed in {ElapsedMilliseconds} ms",
                stopwatch.ElapsedMilliseconds);
        }
    }

    [SuppressMessage("SonarQube", "S3776")]
    private async Task ProcessEntryAsync(EntryProcessingItem item)
    {
        try
        {
            var entry = item.Entry;
            var marketSession = string.Empty;

            if (entry.MdEntryType == FixMessageType.Trade || entry.MdEntryType == FixMessageType.ClosingPrice)
            {
                var marketSchedule = await GetMarketScheduleAsync(item, entry);
                if (marketSchedule is { MarketSession: not null })
                    marketSession = marketSchedule.MarketSession;
            }

            switch (entry.MdEntryType)
            {
                case FixMessageType.Bid:
                case FixMessageType.Offer:
                    var bidOfferWrapper = new VariableWrapper
                    {
                        Symbol = item.Symbol,
                        Venue = item.Venue,
                        MarketSession = marketSession ?? string.Empty,
                        SendingTime = item.SendingTime,
                        SequenceNumber = item.SequenceNumber,
                        MdEntryType = item.MdEntryType,
                        CreationTime = item.CreationTime
                    };
                    await HandleOrderBookAsync(entry, bidOfferWrapper);
                    break;
                case FixMessageType.Trade:
                    var tradeWrapper = new VariableWrapper
                    {
                        Symbol = item.Symbol,
                        Venue = item.Venue,
                        MarketSession = marketSession ?? string.Empty,
                        SendingTime = item.SendingTime,
                        SequenceNumber = item.SequenceNumber,
                        MdEntryType = item.MdEntryType,
                        CreationTime = item.CreationTime
                    };
                    await HandlePublicTradeAsync(entry, tradeWrapper);
                    await HandlePriceInfoAsync(entry, tradeWrapper);
                    break;
                case FixMessageType.OpeningPrice:
                    await HandleOpen(item.Symbol, entry);
                    break;
                case FixMessageType.ClosingPrice:
                    var closePriceWrapper = new VariableWrapper
                    {
                        Symbol = item.Symbol,
                        Venue = item.Venue,
                        MarketSession = marketSession ?? string.Empty,
                        SendingTime = item.SendingTime,
                        SequenceNumber = item.SequenceNumber,
                        MdEntryType = item.MdEntryType,
                        CreationTime = item.CreationTime
                    };
                    await HandlePriorClose(item.Symbol, entry);
                    await SetPriceAsClosingPriceIfMarketClosed(closePriceWrapper, entry);
                    break;
                case FixMessageType.B:
                    var priceWrapper = new VariableWrapper
                    {
                        Symbol = item.Symbol,
                        Venue = item.Venue,
                        MarketSession = marketSession ?? string.Empty,
                        SendingTime = item.SendingTime,
                        SequenceNumber = item.SequenceNumber,
                        MdEntryType = item.MdEntryType,
                        CreationTime = item.CreationTime
                    };
                    await HandlePriceInfoAsync(entry, priceWrapper);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing entry for symbol {Symbol}", item.Symbol);
        }
    }

    private async Task<MarketSchedule?> GetMarketScheduleAsync(EntryProcessingItem item, Entry entry)
    {
        var marketSchedules = await _marketScheduleService.GetMarketSchedulesAsync(item.Symbol, item.Venue);
        var marketSchedule = marketSchedules
            .FirstOrDefault(target => !string.IsNullOrEmpty(target.Symbol)
                                      && target.Symbol.Equals(item.Symbol)
                                      && entry.MdEntryTime >= target.UTCStartTime
                                      && entry.MdEntryTime <= target.UTCEndTime);
        return marketSchedule;
    }


    private async Task<string> GetWhiteListVenue(string symbol, string venue)
    {
        if (!string.IsNullOrEmpty(venue))
            return venue;

        var cacheKey = $"{CacheKey.Whitelist}-exchange-{symbol}";

        try
        {
            var cachedData = await _redisPublisher.GetAsync<string>(cacheKey, true);
            if (!string.IsNullOrEmpty(cachedData))
                return cachedData;

            var whiteList = await _whiteListService.GetByFilterAsync(target =>
                !string.IsNullOrEmpty(target.Symbol)
                && target.Symbol.Equals(symbol)
            );

            if (whiteList is { Exchange: not null })
            {
                var exchange = whiteList.Exchange;

                await _redisPublisher.SetAsync(cacheKey, exchange, true, TimeSpan.FromDays(30));

                return exchange;
            }

            return string.Empty;
        }
        catch
        {
            return venue;
        }
    }

    private async Task HandleOrderBookAsync(Entry entry,
        VariableWrapper variableWrapper)
    {
        var symbol = variableWrapper.Symbol ?? string.Empty;
        var venue = variableWrapper.Venue ?? string.Empty;
        var sendingTime = variableWrapper.SendingTime;
        var sequenceNumber = variableWrapper.SequenceNumber;
        var mdEntryType = variableWrapper.MdEntryType;
        var creationTime = variableWrapper.CreationTime;
        var orderBook = OrderBookMapperService.Map(entry);

        if (orderBook != null)
        {
            var streamingBody = await GetOrCreateStreamingBodyAsync(symbol, venue);

            var streamOrderBook = streamingBody.OrderBook;

            switch (entry.MdEntryType)
            {
                case FixMessageType.Bid:
                    var bidOrder = OrderBookMapperService.ConvertBidToList(orderBook);
                    if (bidOrder != null)
                    {
                        var bidOrders = new List<List<string>> { bidOrder };
                        streamOrderBook.Bid = bidOrders;
                    }

                    break;
                case FixMessageType.Offer:
                    var offerOrder = OrderBookMapperService.ConvertOfferToList(orderBook);
                    if (offerOrder != null)
                    {
                        var offerOrders = new List<List<string>> { offerOrder };
                        streamOrderBook.Offer = offerOrders;
                    }

                    break;
            }

            streamingBody.OrderBook = streamOrderBook;
            await PublishResultAsync(streamingBody, sendingTime, sequenceNumber, mdEntryType, creationTime, true);
        }
    }

    private async Task HandlePriorClose(string symbol, Entry entry)
    {
        var cacheKey = $"{CacheKey.PriorClose}{symbol}";
        await _redisPublisher.SetAsync(cacheKey, entry.MdEntryPx.ToString(CultureInfo.InvariantCulture));
    }

    private async Task HandleOpen(string symbol, Entry entry)
    {
        var cacheKey = $"{CacheKey.Open}{symbol}";
        await _redisPublisher.SetAsync(cacheKey, entry.MdEntryPx.ToString(CultureInfo.InvariantCulture));
    }
    
    private async Task SetPriceAsClosingPriceIfMarketClosed(VariableWrapper closePriceWrapper, Entry entry)
    {
        if (closePriceWrapper.MarketSession == null) return;
        if (closePriceWrapper.MarketSession.Equals(MarketSession.MainSession, StringComparison.InvariantCultureIgnoreCase) ||
             closePriceWrapper.MarketSession.Equals(MarketSession.ScheduleBreak, StringComparison.InvariantCultureIgnoreCase))
            return;
            
        //NOTE: update only when market is not in main session
        var streamingBodyCacheKey = $"{CacheKey.StreamingBody}{closePriceWrapper.Symbol}";
        var streamingBody = await _redisPublisher.GetAsync<StreamingBody>(streamingBodyCacheKey) ??
                            _streamingBodyPool.Get();
        
        var streamingBodyUpdated = _priceInfoMapperService.Map(streamingBody, closePriceWrapper.MarketSession, entry);

        await PublishResultAsync(streamingBodyUpdated, closePriceWrapper.SendingTime, closePriceWrapper.SequenceNumber,
            closePriceWrapper.MdEntryType, closePriceWrapper.CreationTime, true);
    }

    private async Task HandlePriceInfoAsync(Entry entry, VariableWrapper variableWrapper)
    {
        var symbol = variableWrapper.Symbol ?? string.Empty;
        var venue = variableWrapper.Venue ?? string.Empty;
        var marketSession = variableWrapper.MarketSession ?? string.Empty;
        var sendingTime = variableWrapper.SendingTime;
        var sequenceNumber = variableWrapper.SequenceNumber;
        var mdEntryType = variableWrapper.MdEntryType;
        var creationTime = variableWrapper.CreationTime;
        var streamingBody = await GetOrCreateStreamingBodyAsync(symbol, venue);
        var streamingBodyUpdated = _priceInfoMapperService.Map(streamingBody, marketSession, entry);

        await PublishResultAsync(streamingBodyUpdated, sendingTime, sequenceNumber, mdEntryType, creationTime,
            true);
    }

    private async Task HandlePublicTradeAsync(Entry entry, VariableWrapper variableWrapper)
    {
        var symbol = variableWrapper.Symbol ?? string.Empty;
        var venue = variableWrapper.Venue ?? string.Empty;
        var marketSession = variableWrapper.MarketSession;
        var sendingTime = variableWrapper.SendingTime;
        var sequenceNumber = variableWrapper.SequenceNumber;
        var mdEntryType = variableWrapper.MdEntryType;
        var creationTime = variableWrapper.CreationTime;

        if (string.IsNullOrEmpty(marketSession)
            || !MarketSession.MainSession.ToUpper().Equals(marketSession.ToUpper()))
            return;

        var publicTrade = PublicTradeMapperService.Map(entry);
        var streamingBody = await GetOrCreateStreamingBodyAsync(symbol, venue);

        // Build PublicTrade
        var publicTrades = streamingBody.PublicTrades;
        List<List<object>> updatedTrades =
        [
            PublicTradeMapperService.ConvertToList(publicTrade, streamingBody.PreClose)
        ];

        if (publicTrades is { Count: > 0 })
            updatedTrades.AddRange(publicTrades);

        streamingBody.PublicTrades = updatedTrades.Take(DefaultValue.MaxPublicTradeCount).ToList();

        await PublishResultAsync(streamingBody, sendingTime, sequenceNumber, mdEntryType, creationTime, false);
    }

    private async Task<StreamingBody> GetOrCreateStreamingBodyAsync(string symbol, string venue)
    {
        var streamingBodyCacheKey = $"{CacheKey.StreamingBody}{symbol}";
        var priorCloseCacheKey = $"{CacheKey.PriorClose}{symbol}";
        var openCacheKey = $"{CacheKey.Open}{symbol}";
        var batchCacheKeys = new Dictionary<string, bool>
        {
            { priorCloseCacheKey, false },
            { openCacheKey, false }
        };

        var batchCached = await _redisPublisher.GetManyAsync<string>(batchCacheKeys);
        var priorClose = batchCached[priorCloseCacheKey] ?? "0.00";
        var open = batchCached[openCacheKey] ?? "0.00";
        var streamingBody = await _redisPublisher.GetAsync<StreamingBody>(streamingBodyCacheKey) ??
                            _streamingBodyPool.Get();

        streamingBody.PriorClose = priorClose;
        streamingBody.Open = open;
        streamingBody.Symbol = symbol;
        streamingBody.Venue = venue;

        return streamingBody;
    }

    private async Task PublishResultAsync(StreamingBody streamingBody,
        string? sendingTime,
        long? sequenceNumber,
        string? mdEntryType,
        Timestamp creationTime,
        bool isPublish)
    {
        // Start the cache task right away to allow parallel execution
        var cacheTask = SetStreamingCache(streamingBody);

        if (isPublish)
        {
            var compositeKey = $"{streamingBody.Symbol}_{mdEntryType}";

            // Check if this message should be sampled or published
            if (!_messageSamplerService.ShouldPublishMessage(compositeKey))
            {
                _logger.LogDebug("Message for composite key {CompositeKey} sampled out", compositeKey);

                // Make sure cache is set before returning
                await cacheTask;
                return;
            }

            // Only create the response object if we're going to publish
            var streamingResponse = new StreamingResponse { Data = [streamingBody] };
            var marketStreamingResponse = new MarketStreamingResponse
            {
                Code = "200",
                Op = "Streaming",
                SendingTime = sendingTime ?? string.Empty,
                SequenceNumber = sequenceNumber ?? 0,
                ProcessingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
                SendingId = Guid.NewGuid().ToString("N"),
                CreationTime = creationTime.UtcDateTime.ToString("yyyyMMdd-HH:mm:ss.fff"),
                MdEntryType = mdEntryType,
                Response = streamingResponse
            };

            // Store the response in the sampler for future reference
            _messageSamplerService.StoreLatestResponse(compositeKey, marketStreamingResponse);

            // Try to post to the publishing block
            var success = _publishBlock.Post(marketStreamingResponse);
            if (!success)
                try
                {
                    // Fallback to direct publishing when the block is full
                    await _redisPublisher.PublishAsync(_channel, marketStreamingResponse, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing response directly");
                }
        }

        // Ensure cache is set before returning
        await cacheTask;
    }

    private async Task SetStreamingCache(StreamingBody streamingBody)
    {
        var symbol = streamingBody.Symbol;

        await _taskQueue.QueueTaskAsync(async _ =>
        {
            try
            {
                var priceBody = new PriceResponse
                {
                    Price = !string.IsNullOrEmpty(streamingBody.Price)
                        ? streamingBody.Price
                        : "0.00",
                    PriceChanged = streamingBody.PriceChanged,
                    PriceChangedRate = streamingBody.PriceChangedRate,
                    TotalVolume = streamingBody.TotalVolume,
                    TotalAmount = streamingBody.TotalAmount
                };

                var cacheKey = $"{CacheKey.StreamingBody}{symbol}";
                await _redisPublisher.SetAsync(cacheKey, streamingBody);

                var priceCacheKey = $"{CacheKey.PriceStreamingBody}{symbol}";
                await _redisPublisher.SetStringAsync(priceCacheKey, priceBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting price cache for {Symbol}", symbol);
            }
        });
    }
}