using Pi.SetMarketDataRealTime.Application.Interfaces.MessageValidator;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Services.MessageValidator;

public class MessageValidator : IMessageValidator
{
    private const char SystemEventMessageType = 'S';
    private const string EndOfMessageEventCode = "C";

    public bool IsEndOfMessage(ItchMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        return IsSystemEventMessage(message) && IsEndOfMessageEvent(message);
    }

    private static bool IsSystemEventMessage(ItchMessage message)
    {
        return message.MsgType == SystemEventMessageType;
    }

    private static bool IsEndOfMessageEvent(ItchMessage message)
    {
        if (message is not SystemEventMessage systemEventMessage) return false;

        return systemEventMessage.EventCode.Value == EndOfMessageEventCode;
    }
}