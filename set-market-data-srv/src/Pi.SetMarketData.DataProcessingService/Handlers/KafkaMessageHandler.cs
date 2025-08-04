using System.Diagnostics;
using Confluent.Kafka;
using MongoDB.Bson;
using Newtonsoft.Json;
using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.DataProcessingService.Interface;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.DataProcessing;
using Pi.SetMarketData.Infrastructure.Converters;
using Pi.SetMarketData.Infrastructure.Helpers;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketData.Infrastructure.Interfaces.Mongo;
using Pi.SetMarketData.Infrastructure.Interfaces.Redis;
using Pi.SetMarketData.Infrastructure.Interfaces.TimescaleEf;
using Pi.SetMarketData.Infrastructure.Models.Kafka;

namespace Pi.SetMarketData.DataProcessingService.Handlers;

public class KafkaMessageHandler : IKafkaMessageV2Handler<Message<string, string>>
{
    private const string RealtimeTableName = "realtime_market_data";
    private readonly TimeSpan _cacheMinute = new(0, 0, 1, 0);
    private readonly IRedisV2Publisher _cacheService;
    private readonly ICacheServiceHelper _cacheServiceHelper;
    private readonly IMongoService<CorporateAction> _corporateActionService;
    private readonly string _exchangeServer;
    private readonly IMongoService<InstrumentDetail> _instrumentDetailService;
    private readonly IMongoService<Instrument> _instrumentService;
    private readonly IItchMapperService _itchMapperService;
    private readonly JsonSerializerSettings _jsonSettings = new() { Converters = [new ObjectIdConverter()] };
    private readonly ILogger<KafkaMessageHandler> _logger;
    private readonly IMongoService<MorningStarFlag> _morningStarFlagService;
    private readonly IMongoService<OrderBook> _orderBookService;
    private readonly IMongoService<PriceInfo> _priceInfoService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMongoService<TradingSign> _tradingSignService;
    private readonly IMongoService<WhiteList> _whiteListService;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="itchMapperService"></param>
    /// <param name="dependencies"></param>
    /// <param name="moreDependencies"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public KafkaMessageHandler(
        IConfiguration configuration,
        IItchMapperService itchMapperService,
        KafkaMessageHandlerDependencies dependencies,
        KafkaMessageHandlerMoreDependencies moreDependencies,
        ILogger<KafkaMessageHandler> logger
    )
    {
        _priceInfoService = dependencies.PriceInfoService;
        _orderBookService = dependencies.OrderBookService;
        _instrumentService = dependencies.InstrumentService;
        _instrumentDetailService = dependencies.InstrumentDetailService;
        _whiteListService = dependencies.WhiteListService;

        _corporateActionService = dependencies.CorporateActionService;
        _tradingSignService = dependencies.TradingSignService;
        _cacheService = moreDependencies.CacheService;
        _morningStarFlagService = moreDependencies.MorningStarFlagService;
        _cacheServiceHelper = moreDependencies.CacheServiceHelper;
        _scopeFactory = moreDependencies.ScopeFactory;

        _itchMapperService = itchMapperService ?? throw new ArgumentNullException(nameof(itchMapperService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _exchangeServer =
            ConfigurationHelper
                .GetTopicList(configuration, ConfigurationKeys.KafkaTopic)
                .FirstOrDefault()
            ?? throw new InvalidOperationException("KafkaTopic is not configured");
    }

    public async Task<bool> HandleAsync(Message<string, string> consumeMessage)
    {
        var message = consumeMessage.Value;
        if (string.IsNullOrEmpty(message))
        {
            _logger.LogWarning("Received empty message");
            return false;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            if (!message.IsValidJsonMessage())
            {
                _logger.LogWarning("The message cannot be deserialized because it is invalid");
                return false;
            }

            var cleanMessage = message.SimpleCleanJsonMessage();
            var stockMessage = JsonConvert.DeserializeObject<StockMessage>(cleanMessage);

            if (stockMessage?.Message == null)
                return false;

            _logger.LogDebug("Received message: {CleanMessage}", cleanMessage);
            var success = stockMessage.MessageType switch
            {
                nameof(ItchMessageType.i) => await HandleTradeTickerMessageAsync(stockMessage.Message),
                nameof(ItchMessageType.I) => await HandleTradeStatisticsMessageAsync(stockMessage.Message),
                nameof(ItchMessageType.b) => await HandleMarketByPriceMessageAsync(stockMessage.Message),
                nameof(ItchMessageType.R) => await HandleOrderBookDirectoryMessageAsync(stockMessage.Message,
                    _exchangeServer.StartsWith(InstrumentConstants.SET, StringComparison.OrdinalIgnoreCase),
                    JsonConvert.DeserializeObject<MarketDirectory>(
                        await _cacheServiceHelper.TryGetAsync(
                            $"{CacheKey.MarketDirectory}{JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(stockMessage.Message)?.MarketCode.Value}"
                        ) ?? string.Empty)
                ),
                nameof(ItchMessageType.L) => await HandleTickSizeTable(stockMessage.Message),
                nameof(ItchMessageType.J) => await HandleIndexMessageAsync(stockMessage.Message),
                _ => true
            };

            return success;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Json exception during processing message: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing message: {Message}", ex.Message);
            return false;
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation("HandleAsync completed in {ElapsedMilliseconds} ms",
                stopwatch.ElapsedMilliseconds);
        }
    }


    private async Task<bool> HandleTradeTickerMessageAsync(string message)
    {
        var tradeTickerMessage = JsonConvert.DeserializeObject<TradeTickerMessageWrapper>(message);
        if (tradeTickerMessage == null)
            return false;

        var result = await MapToDatabase(tradeTickerMessage);

        if (result.Values is { Length: > 0 })
        {
            foreach (var item in result.Values)
                await HandleMessageResultAsync(result.Channel, item.TableName ?? RealtimeTableName, item.Value);
            return true;
        }

        return false;
    }

    private async Task<bool> HandleIndexMessageAsync(string message)
    {
        var indexMessage = JsonConvert.DeserializeObject<IndexPriceMessageWrapper>(message);
        if (indexMessage == null)
            return false;

        var result = await MapToDatabase(indexMessage);

        if (result.Values is { Length: > 0 })
        {
            foreach (var item in result.Values)
                await HandleMessageResultAsync(result.Channel, item.TableName ?? RealtimeTableName, item.Value);
            return true;
        }

        return false;
    }


    private async Task<bool> HandleTradeStatisticsMessageAsync(string message)
    {
        var tradeStatisticsMessage = JsonConvert.DeserializeObject<TradeStatisticsMessageWrapper>(message);
        if (tradeStatisticsMessage == null)
            return false;

        var results = _itchMapperService.MapToDatabase(tradeStatisticsMessage, null, null, null, null).ToList();

        if (results.Count == 0)
            return false;

        foreach (var result in results)
            if (result?.Values is { Length: > 0 })
                foreach (var item in result.Values)
                    await HandleMessageResultAsync(result.Channel, item.TableName ?? "price_info", item.Value);

        return true;
    }


    private async Task<bool> HandleMarketByPriceMessageAsync(string message)
    {
        var orderBookMessage = JsonConvert.DeserializeObject<MarketByPriceLevelWrapper>(message);
        if (orderBookMessage == null)
            return false;

        var storedData = await _orderBookService.GetByIdAsync(orderBookMessage.OrderBookID.ToString());
        var results = _itchMapperService.MapToDatabase(orderBookMessage, storedData, null, null, null).ToList();

        if (results.Count == 0)
            return false;

        foreach (var result in results)
            if (result?.Values is { Length: > 0 })
                foreach (var item in result.Values)
                    await HandleMessageResultAsync(result.Channel, item.TableName ?? "order_book", item.Value);

        return true;
    }


    private async Task<bool> HandleOrderBookDirectoryMessageAsync(string message, bool exchangeServer,
        MarketDirectory? marketDirectory)
    {
        var orderBookDirectoryMessage = JsonConvert.DeserializeObject<OrderBookDirectoryMessageWrapper>(message);
        if (orderBookDirectoryMessage == null)
            return false;

        var venue = await _cacheServiceHelper.GetVenueByOrderBookId(
            orderBookDirectoryMessage.OrderBookID.Value,
            orderBookDirectoryMessage.Symbol.ToString() ?? string.Empty
        );

        var results = _itchMapperService.MapToDatabase(
            orderBookDirectoryMessage,
            null,
            exchangeServer,
            GetVenue(venue ?? string.Empty),
            marketDirectory
        ).ToList();

        if (results.Count == 0)
            return false;

        foreach (var result in results)
            if (result?.Values is { Length: > 0 })
                foreach (var item in result.Values)
                    await HandleMessageResultAsync(result.Channel, item.TableName ?? "order_book_directory",
                        item.Value);

        return true;
    }


    private async Task<bool> HandleTickSizeTable(string message)
    {
        var tickSizeTableEntry = JsonConvert.DeserializeObject<TickSizeTableMessageWrapper>(message);
        if (tickSizeTableEntry == null)
            return false;

        var results = _itchMapperService.MapToDatabase(tickSizeTableEntry, null, null, null, null).ToList();

        if (results.Count == 0)
            return false;

        foreach (var result in results)
            if (result?.Values is { Length: > 0 })
                foreach (var item in result.Values)
                    await HandleMessageResultAsync(result.Channel, item.TableName ?? "tick_size_table_entry",
                        item.Value);

        return true;
    }


    private async Task HandleMessageResultAsync(
        DataProcessingChannel? channel,
        string tableName,
        object? value
    )
    {
        switch (channel)
        {
            case DataProcessingChannel.MongoDb:
                await SaveDataToMongoDb(tableName, value);
                break;
            case DataProcessingChannel.TimescaleDb:
                await SaveDataToTimescaleDb(tableName, value);
                break;
            case DataProcessingChannel.Redis:
                await SaveDataToCache(tableName, value);
                break;
            case DataProcessingChannel.Both:
                break;
            default:
                _logger.LogError("The channel. Out of the channel's range");
                break;
        }
    }

    private async Task SaveDataToTimescaleDb(string tableName, object? value)
    {
        if (value == null)
            return;

        try
        {
            switch (tableName.ToLowerInvariant())
            {
                case "realtime_market_data" when value is RealtimeMarketData realtimeMarketData:
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var realtimeMarketDataService = scope.ServiceProvider
                            .GetRequiredService<ITimescaleService<RealtimeMarketData>>();
                        await realtimeMarketDataService.UpsertAsync(
                            realtimeMarketData,
                            RealtimeTableName,
                            nameof(RealtimeMarketData.DateTime),
                            nameof(RealtimeMarketData.Symbol),
                            nameof(RealtimeMarketData.Venue)
                        );

                        _logger.LogInformation("Save: {Symbol}, Venue: {Venue}, Date: {Date}",
                            realtimeMarketData.Symbol, realtimeMarketData.Venue, realtimeMarketData.DateTime);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tableName));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    private async Task SaveDataToMongoDb(string tableName, object? value)
    {
        if (value == null)
            return;
        switch (tableName.ToLowerInvariant())
        {
            case "price_info" when value is PriceInfo priceInfo:
                var priceData = await _priceInfoService.GetByOrderBookId(
                    priceInfo.OrderBookId ?? string.Empty
                );
                await AddOrUpdatePriceInfo(priceData, priceInfo);
                break;

            case "order_book" when value is OrderBook orderBook:
                var orderData = await _orderBookService.GetByOrderBookId(
                    orderBook.OrderBookId.ToString()
                );
                await AddOrUpdateOrderBook(orderData, orderBook);
                break;

            case "instrument" when value is Instrument instrument:
                var instrumentData = await _instrumentService.GetByOrderBookId(
                    instrument.OrderBookId.ToString()
                );
                await AddOrUpdateInstrument(instrumentData, instrument);
                await AddOrUpdateMorningStarFlag(instrument);
                break;

            case "instrument_detail" when value is InstrumentDetail detail:
                var detailData = await _instrumentDetailService.GetByOrderBookId(
                    detail.OrderBookId.ToString()
                );
                await AddOrUpdateInstrumentDetail(detailData, detail);
                break;

            case "white_list" when value is WhiteList whiteList:
                var whiteListData = await _whiteListService.GetByFilterAsync(target =>
                    target.Symbol == whiteList.Symbol
                );
                await AddOrUpdateWhiteList(whiteListData, whiteList);
                break;
            case "corporate_action" when value is CorporateAction corporateAction:
                var corporateActionData = await _corporateActionService.GetByFilterAsync(target =>
                    target.OrderBookId == corporateAction.OrderBookId
                );
                await AddOrUpdateCorporateAction(corporateActionData, corporateAction);
                break;
            case "trading_sign" when value is TradingSign tradingSign:
                var tradingSignData = await _tradingSignService.GetByFilterAsync(target =>
                    target.OrderBookId == tradingSign.OrderBookId
                );
                await AddOrUpdateTradingSign(tradingSignData, tradingSign);
                break;
            default:
                _logger.LogError(
                    "The {TableName} collection. Out of the collection's range",
                    tableName
                );
                break;
        }
    }

    private async Task SaveDataToCache(string key, object? value)
    {
        if (value == null) return;

        switch (value)
        {
            case TradingSign tradingSign:
                await _cacheService.SetAsync
                (
                    $"{key}{tradingSign.OrderBookId}",
                    JsonConvert.SerializeObject(tradingSign, _jsonSettings),
                    false,
                    _cacheMinute
                );
                break;
        }
    }

    private async Task AddOrUpdateWhiteList(WhiteList? whiteListData, WhiteList whiteList)
    {
        if (whiteListData != null && !string.IsNullOrEmpty(whiteListData.Id.ToString()))
        {
            await _whiteListService.UpdateAsync(whiteListData.Id.ToString(), whiteList);
        }
        else
        {
            whiteList.Id = ObjectId.GenerateNewId();
            await _whiteListService.CreateAsync(whiteList);
        }
    }

    private async Task AddOrUpdateInstrumentDetail(
        InstrumentDetail? detailData,
        InstrumentDetail detail
    )
    {
        if (detailData != null && !string.IsNullOrEmpty(detailData.Id.ToString()))
        {
            await _instrumentDetailService.UpdateAsync(detailData.Id.ToString(), detail);
        }
        else
        {
            detail.Id = ObjectId.GenerateNewId();
            await _instrumentDetailService.CreateAsync(detail);
        }
    }

    private async Task AddOrUpdateInstrument(Instrument? instrumentData, Instrument instrument)
    {
        if (instrumentData != null && !string.IsNullOrEmpty(instrumentData.Id.ToString()))
        {
            await _instrumentService.UpdateAsync(instrumentData.Id.ToString(), instrument);
        }
        else
        {
            instrument.Id = ObjectId.GenerateNewId();
            await _instrumentService.CreateAsync(instrument);
        }
    }

    private async Task AddOrUpdateMorningStarFlag(Instrument instrument)
    {
        var morningStarFlagData = await _morningStarFlagService.GetByFilterAsync(target =>
            target.StandardTicker == instrument.Symbol
        );

        var morningStarFlag =
            morningStarFlagData
            ?? new MorningStarFlag { ExchangeId = "BKK", StandardTicker = instrument.Symbol };

        morningStarFlag.StartDate = DateTime.Now.AddYears(-2).ToString(DataFormat.DateTimeFormat);
        morningStarFlag.EndDate = DateTime.Now.ToString(DataFormat.DateTimeFormat);
        morningStarFlag.ExcludingFrom = DateTime
            .Now.AddYears(-2)
            .ToString(DataFormat.DateTimeFormat);
        morningStarFlag.ExcludingTo = DateTime.Now.ToString(DataFormat.DateTimeFormat);

        await _morningStarFlagService.UpsertAsyncByFilter(
            target => target.StandardTicker == instrument.Symbol,
            morningStarFlag
        );
    }

    private async Task AddOrUpdateOrderBook(OrderBook? orderData, OrderBook orderBook)
    {
        if (orderData != null && !string.IsNullOrEmpty(orderData.Id.ToString()))
        {
            await _orderBookService.UpdateAsync(orderData.Id.ToString(), orderBook);
        }
        else
        {
            orderBook.Id = ObjectId.GenerateNewId();
            await _orderBookService.CreateAsync(orderBook);
        }
    }

    private async Task AddOrUpdatePriceInfo(PriceInfo? priceData, PriceInfo priceInfo)
    {
        if (priceData != null && !string.IsNullOrEmpty(priceData.Id.ToString()))
        {
            await _priceInfoService.UpdateAsync(priceData.Id.ToString(), priceInfo);
        }
        else
        {
            priceInfo.Id = ObjectId.GenerateNewId();
            await _priceInfoService.CreateAsync(priceInfo);
        }
    }

    private async Task AddOrUpdateCorporateAction(
        CorporateAction? corporateActionData,
        CorporateAction corporateAction
    )
    {
        if (corporateActionData != null && !string.IsNullOrEmpty(corporateActionData.Id.ToString()))
        {
            await _corporateActionService.UpdateAsync(
                corporateActionData.Id.ToString(),
                corporateAction
            );
        }
        else
        {
            corporateAction.Id = ObjectId.GenerateNewId();
            await _corporateActionService.CreateAsync(corporateAction);
        }
    }

    private async Task AddOrUpdateTradingSign(TradingSign? tradingSignData, TradingSign tradingSign)
    {
        if (tradingSignData != null && !string.IsNullOrEmpty(tradingSignData.Id.ToString()))
        {
            await _tradingSignService.UpdateAsync(tradingSignData.Id.ToString(), tradingSign);
        }
        else
        {
            tradingSign.Id = ObjectId.GenerateNewId();
            await _tradingSignService.CreateAsync(tradingSign);
        }
    }

    private async Task<DataProcessingResult> MapToDatabase(
        TradeTickerMessageWrapper tradeTickerMessage
    )
    {
        try
        {
            if (tradeTickerMessage.Action.Value == 1) // New item
            {
                var orderBookId = tradeTickerMessage.OrderbookId.Value;
                var symbol = await _cacheServiceHelper.GetSymbolByOrderBookId(orderBookId);

                if (string.IsNullOrEmpty(symbol))
                    throw new InvalidOperationException("Symbol cannot be null or empty!");

                var venue = await _cacheServiceHelper.GetVenueByOrderBookId(
                    orderBookId,
                    symbol
                );

                if (string.IsNullOrEmpty(venue))
                    throw new InvalidOperationException("Venue cannot be null or empty!");

                if (!string.IsNullOrEmpty(symbol) && !string.IsNullOrEmpty(venue))
                {
                    var decimals = await _cacheService.GetAsync<int>($"decimals-in-price-{orderBookId.ToString()}");
                    if (decimals == 0)
                    {
                        var instrumentDetail = await _instrumentDetailService.GetByOrderBookId(
                            orderBookId.ToString()
                        );
                        decimals = instrumentDetail?.DecimalsInPrice ?? 0;
                    }

                    var price = double.Parse(
                        FormatDecimals(tradeTickerMessage.Price.Value.ToString(), decimals)
                    );
                    var realtimeMarketData = new RealtimeMarketData
                    {
                        DateTime = (DateTimeOffset)tradeTickerMessage.DealDateTime,
                        Symbol = symbol,
                        Venue = GetVenue(venue),
                        Price = price,
                        Volume = tradeTickerMessage.Quantity.Value,
                        Amount = price * tradeTickerMessage.Quantity.Value
                    };

                    return new DataProcessingResult
                    {
                        Channel = DataProcessingChannel.TimescaleDb,
                        Values =
                        [
                            new DataProcessingValue
                            {
                                TableName = RealtimeTableName,
                                Value = realtimeMarketData
                            }
                        ]
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing message: {Message}", ex.Message);
        }

        return new DataProcessingResult { Channel = DataProcessingChannel.TimescaleDb };
    }

    private async Task<DataProcessingResult> MapToDatabase(
        IndexPriceMessageWrapper indexPriceMessage
    )
    {
        try
        {
            var orderBookId = indexPriceMessage.OrderBookId.Value;
            var symbol = await _cacheServiceHelper.GetSymbolByOrderBookId(orderBookId);

            if (string.IsNullOrEmpty(symbol))
                throw new InvalidOperationException("Symbol cannot be null or empty!");

            var venue = await _cacheServiceHelper.GetVenueByOrderBookId(
                orderBookId,
                symbol
            );

            if (string.IsNullOrEmpty(venue))
                throw new InvalidOperationException("Venue cannot be null or empty!");

            if (!string.IsNullOrEmpty(symbol) && !string.IsNullOrEmpty(venue))
            {
                var decimals = await _cacheService.GetAsync<int>($"decimals-in-price-{orderBookId.ToString()}");
                if (decimals == 0)
                {
                    var instrumentDetail = await _instrumentDetailService.GetByOrderBookId(
                        orderBookId.ToString()
                    );
                    decimals = instrumentDetail?.DecimalsInPrice ?? 0;
                }

                var price = double.Parse(
                    FormatDecimals(indexPriceMessage.Value.Value.ToString(), decimals)
                );
                var realtimeMarketData = new RealtimeMarketData
                {
                    DateTime = (DateTimeOffset)indexPriceMessage.Timestamp,
                    Symbol = symbol,
                    Venue = GetVenue(venue),
                    Price = price,
                    Volume = indexPriceMessage.TradedVolume.Value,
                    Amount = indexPriceMessage.TradedValue.Value
                };

                return new DataProcessingResult
                {
                    Channel = DataProcessingChannel.TimescaleDb,
                    Values =
                    [
                        new DataProcessingValue
                        {
                            TableName = RealtimeTableName,
                            Value = realtimeMarketData
                        }
                    ]
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing message: {Message}", ex.Message);
        }

        return new DataProcessingResult { Channel = DataProcessingChannel.TimescaleDb };
    }

    private string GetVenue(string venue)
    {
        if (!string.IsNullOrEmpty(venue))
        {
            if (venue.Trim().Equals(InstrumentConstants.SET, StringComparison.OrdinalIgnoreCase))
                return InstrumentConstants.VenueOfSetStock;

            if (venue.Trim().Equals(InstrumentConstants.TFEX, StringComparison.OrdinalIgnoreCase))
                return InstrumentConstants.VenueOfTfexStock;
        }
        else
        {
            if (
                _exchangeServer.StartsWith(
                    InstrumentConstants.SET,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                return InstrumentConstants.VenueOfSetStock;

            if (
                _exchangeServer.StartsWith(
                    InstrumentConstants.TFEX,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                return InstrumentConstants.VenueOfTfexStock;
        }

        return venue;
    }

    private static string FormatDecimals(string value, int decimals)
    {
        // Check for null or empty value
        if (string.IsNullOrEmpty(value))
            return "0." + new string('0', decimals);

        // Handle the case where decimals is 0
        if (decimals == 0)
        {
            // If value contains a decimal point, return it unchanged
            if (value.Contains('.'))
                return value;

            // If value does not contain a decimal point, append ".00"
            return value + ".00";
        }

        // Remove any existing decimal point
        value = value.Replace(".", string.Empty);

        // Calculate the position to insert the decimal point
        var insertPosition = value.Length - decimals;

        // If the position is less than or equal to 0, the value will have all zeros before the decimal point
        if (insertPosition <= 0)
            return "0." + value.PadLeft(decimals, '0');

        // Insert the decimal point
        var formattedValue = value.Insert(insertPosition, ".");

        return formattedValue;
    }
}