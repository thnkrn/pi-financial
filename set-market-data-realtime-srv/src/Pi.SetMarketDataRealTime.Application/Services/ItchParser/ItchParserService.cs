using Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Services.ItchParser;

public class ItchParserService : IItchParserService
{
    private readonly ItchMessageMetadataHandler _metadataHandler;

    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    public ItchParserService(ItchMessageMetadataHandler metadataHandler)
    {
        _metadataHandler = metadataHandler;
    }

    public async Task<ItchMessage> Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 1) throw new ArgumentException("Invalid message format.");

        var msgType = (char)bytes[0];
        var messageData = bytes[1..];

        switch (msgType)
        {
            case ItchMessageType.T:
                var secondsMessage = SecondsMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(secondsMessage,
                    string.Empty);
                return secondsMessage;
            case ItchMessageType.R:
                var orderBookDirectoryMessage = OrderBookDirectoryMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(orderBookDirectoryMessage,
                    orderBookDirectoryMessage.OrderBookID.Value.ToString());
                return orderBookDirectoryMessage;
            case ItchMessageType.e:
                var exchangeDirectoryMessage = ExchangeDirectoryMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(exchangeDirectoryMessage,
                    string.Empty);
                return exchangeDirectoryMessage;
            case ItchMessageType.m:
                var marketDirectoryMessage = MarketDirectoryMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(marketDirectoryMessage,
                    string.Empty);
                return marketDirectoryMessage;
            case ItchMessageType.M:
                var combinationOrderBookDirectoryMessage = CombinationOrderBookDirectoryMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(combinationOrderBookDirectoryMessage,
                    string.Empty);
                return combinationOrderBookDirectoryMessage;
            case ItchMessageType.L:
                var tickSizeMessage = TickSizeMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(tickSizeMessage,
                    tickSizeMessage.OrderBookId.Value.ToString());
                return tickSizeMessage;
            case ItchMessageType.k:
                var priceLimitMessage = PriceLimitMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(priceLimitMessage,
                    priceLimitMessage.OrderbookId.Value.ToString());
                return priceLimitMessage;
            case ItchMessageType.S:
                var systemEventMessage = SystemEventMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(systemEventMessage,
                    string.Empty);
                return systemEventMessage;
            case ItchMessageType.O:
                var orderBookStateMessage = OrderBookStateMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(orderBookStateMessage,
                    orderBookStateMessage.OrderBookId.Value.ToString());
                return orderBookStateMessage;
            case ItchMessageType.l:
                var haltInformationMessage = HaltInformationMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(haltInformationMessage,
                    haltInformationMessage.OrderBookId.Value.ToString());
                return haltInformationMessage;
            case ItchMessageType.b:
                var marketByPriceMessage = MarketByPriceMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(marketByPriceMessage,
                    marketByPriceMessage.OrderBookID.Value.ToString());
                return marketByPriceMessage;
            case ItchMessageType.Z:
                var equilibriumPriceMessage = EquilibriumPriceMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(equilibriumPriceMessage,
                    equilibriumPriceMessage.OrderBookID.Value.ToString());
                return equilibriumPriceMessage;
            case ItchMessageType.i:
                var tradeTickerMessage = TradeTickerMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(tradeTickerMessage,
                    tradeTickerMessage.OrderbookId.Value.ToString());
                return tradeTickerMessage;
            case ItchMessageType.I:
                var tradeStatisticsMessage = TradeStatisticsMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(tradeStatisticsMessage,
                    tradeStatisticsMessage.OrderBookId.Value.ToString());
                return tradeStatisticsMessage;
            case ItchMessageType.f:
                var iNavMessage = INAVMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(iNavMessage,
                    iNavMessage.OrderBookId.Value.ToString());
                return iNavMessage;
            case ItchMessageType.J:
                var indexPriceMessage = IndexPriceMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(indexPriceMessage,
                    indexPriceMessage.OrderbookId.Value.ToString());
                return indexPriceMessage;
            case ItchMessageType.g:
                var marketStatisticMessage = MarketStatisticMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(marketStatisticMessage,
                    string.Empty);
                return marketStatisticMessage;
            case ItchMessageType.Q:
                var referencePriceMessage = ReferencePriceMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(referencePriceMessage,
                    referencePriceMessage.OrderbookId.Value.ToString());
                return referencePriceMessage;
            case ItchMessageType.h:
                var openInterestMessage = OpenInterestMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(openInterestMessage,
                    openInterestMessage.OrderbookId.Value.ToString());
                return openInterestMessage;
            case ItchMessageType.N:
                var marketAnnouncementMessage = MarketAnnouncementMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(marketAnnouncementMessage,
                    marketAnnouncementMessage.OrderBookId.Value.ToString());
                return marketAnnouncementMessage;
            case ItchMessageType.j:
                var newsMessage = NewsMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(newsMessage,
                    string.Empty);
                return newsMessage;
            case ItchMessageType.G:
                var endOfSnapshotMessage = EndOfSnapshotMessage.Parse(messageData);
                await _metadataHandler.AssignMetadataAsync(endOfSnapshotMessage,
                    string.Empty);
                return endOfSnapshotMessage;
            default:
                throw new NotSupportedException($"Message type {msgType} is not supported.");
        }
    }
}