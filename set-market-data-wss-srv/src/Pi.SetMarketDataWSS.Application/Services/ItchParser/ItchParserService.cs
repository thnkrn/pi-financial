using Pi.SetMarketDataWSS.Application.Interfaces.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Services.ItchParser;

public class ItchParserService : IItchParserService
{
    public ItchMessage Parse(byte[] bytes)
    {
        if (bytes == null || bytes.Length < 1) throw new ArgumentException("Invalid message format.");

        var msgType = (char)bytes[0];
        var messageData = bytes[1..];

        return msgType switch
        {
            ItchMessageType.T => SecondsMessage.Parse(messageData),
            ItchMessageType.R => OrderBookDirectoryMessage.Parse(messageData),
            ItchMessageType.e => ExchangeDirectoryMessage.Parse(messageData),
            ItchMessageType.m => MarketDirectoryMessage.Parse(messageData),
            ItchMessageType.M => CombinationOrderBookDirectoryMessage.Parse(messageData),
            ItchMessageType.L => TickSizeMessage.Parse(messageData),
            ItchMessageType.k => PriceLimitMessage.Parse(messageData),
            ItchMessageType.S => SystemEventMessage.Parse(messageData),
            ItchMessageType.O => OrderBookStateMessage.Parse(messageData),
            ItchMessageType.l => HaltInformationMessage.Parse(messageData),
            ItchMessageType.b => MarketByPriceMessage.Parse(messageData),
            ItchMessageType.Z => EquilibriumPriceMessage.Parse(messageData),
            ItchMessageType.i => TradeTickerMessage.Parse(messageData),
            ItchMessageType.I => TradeStatisticsMessage.Parse(messageData),
            ItchMessageType.f => INAVMessage.Parse(messageData),
            ItchMessageType.J => IndexPriceMessage.Parse(messageData),
            ItchMessageType.g => MarketStatisticMessage.Parse(messageData),
            ItchMessageType.Q => ReferencePriceMessage.Parse(messageData),
            ItchMessageType.h => OpenInterestMessage.Parse(messageData),
            ItchMessageType.N => MarketAnnouncementMessage.Parse(messageData),
            ItchMessageType.j => NewsMessage.Parse(messageData),
            _ => throw new NotSupportedException($"Message type {msgType} is not supported.")
        };
    }
}