using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Interfaces.ItchMapper;
using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.DataProcessing;

namespace Pi.SetMarketData.Application.Services.ItchMapper;

public class ItchMapperService(
    IItchPriceInfoMapperService priceInfoMapperService,
    IItchOrderBookMapperService itchOrderBookMapperService,
    IItchOrderBookDirectoryMapperService itchOrderBookDirectoryMapperService,
    IItchTickSizeTableEntryMapperService itchTickSizeTableMapperService) : IItchMapperService
{
    public IEnumerable<DataProcessingResult?> MapToDatabase
    (
        ItchMessage message, 
        OrderBook? storedData, 
        bool? exchangeServer, 
        string? venue,
        MarketDirectory? marketDirectory
    )
    {
        switch (message.MsgType)
        {
            case ItchMessageType.i: // Convert message to PriceInfo
                var tradeTickerPriceInfo = priceInfoMapperService.Map(message);
                var tradeTickerPriceInfoResult = new DataProcessingResult
                {
                    Channel = DataProcessingChannel.MongoDb,
                    Values =
                    [
                        new DataProcessingValue
                        {
                            TableName = "price_info",
                            Value = tradeTickerPriceInfo
                        }
                    ]
                };
                return [tradeTickerPriceInfoResult];
            case ItchMessageType.I: // Convert message to PriceInfo
                var tradeStatisticsPriceInfo = priceInfoMapperService.Map(message);
                var tradeStatisticsPriceInfoResult = new DataProcessingResult
                {
                    Channel = DataProcessingChannel.MongoDb,
                    Values =
                    [
                        new DataProcessingValue
                        {
                            TableName = "price_info",
                            Value = tradeStatisticsPriceInfo
                        }
                    ]
                };
                return [tradeStatisticsPriceInfoResult];
            case ItchMessageType.Q:
                break;

            case ItchMessageType.b: // Convert message to OrderBook
                var wrapper = (MarketByPriceLevelWrapper)message;
                var orderBook = itchOrderBookMapperService.Map(wrapper, storedData);
                var orderBookMessageResult = new DataProcessingResult
                {
                    Channel = DataProcessingChannel.MongoDb,
                    Values =
                    [
                        new DataProcessingValue
                        {
                            TableName = "order_book",
                            Value = orderBook
                        },
                    ]
                };
                return [orderBookMessageResult];
            case ItchMessageType.R: // Convert message to OrderBookDirectory
                var convertedResult = (OrderBookDirectoryMessageWrapper)message;
                var orderBookValue = itchOrderBookDirectoryMapperService.MapToOrderBook(convertedResult);
                var instrumentDetailValue = itchOrderBookDirectoryMapperService.MapToInstrumentDetail(convertedResult);
                var instrumentValue = itchOrderBookDirectoryMapperService.MapToInstrument(convertedResult, marketDirectory);
                if (instrumentValue != null) instrumentValue.Venue = venue;
                var whiteListValue =
                    itchOrderBookDirectoryMapperService.MapToWhiteList(convertedResult, exchangeServer);
                var corporateActionValue = itchOrderBookDirectoryMapperService.MapToCorporateAction(convertedResult);
                var tradingSignValue = itchOrderBookDirectoryMapperService.MapToTradingSign(convertedResult);

                var orderBookDirectoryResult = new DataProcessingResult
                {
                    Channel = DataProcessingChannel.MongoDb,
                    Values =
                    [
                        new DataProcessingValue
                        {
                            TableName = "instrument",
                            Value = instrumentValue
                        },
                        new DataProcessingValue
                        {
                            TableName = "order_book",
                            Value = orderBookValue
                        },
                        new DataProcessingValue
                        {
                            TableName = "instrument_detail",
                            Value = instrumentDetailValue
                        },
                        new DataProcessingValue
                        {
                            TableName = "white_list",
                            Value = whiteListValue
                        },
                        new DataProcessingValue
                        {
                            TableName = "corporate_action",
                            Value = corporateActionValue
                        },
                        new DataProcessingValue
                        {
                            TableName = "trading_sign",
                            Value = tradingSignValue
                        }
                    ]
                };
                var cacheResult = new DataProcessingResult
                {
                    Channel = DataProcessingChannel.Redis,
                    Values =
                    [
                        new DataProcessingValue
                        {
                            TableName = CacheKey.TradingSign,
                            Value = tradingSignValue
                        }
                    ]
                };
                return [orderBookDirectoryResult, cacheResult];
            case ItchMessageType.L:
                var convertedTickSizeTable = (TickSizeTableMessageWrapper)message;

                instrumentValue = itchTickSizeTableMapperService.MapToInstrument(convertedTickSizeTable) ?? new Instrument();
                var tickSizeTableResult = new DataProcessingResult
                {
                    Channel = DataProcessingChannel.MongoDb,
                    Values =
                    [
                        new DataProcessingValue
                    {
                        TableName = "instrument",
                        Value = instrumentValue
                    }
                    ]
                };
                return [tickSizeTableResult];
        }

        return [];
    }
}