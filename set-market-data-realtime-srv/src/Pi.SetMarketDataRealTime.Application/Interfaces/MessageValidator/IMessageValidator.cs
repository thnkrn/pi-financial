using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Interfaces.MessageValidator;

public interface IMessageValidator
{
    bool IsEndOfMessage(ItchMessage message);
}