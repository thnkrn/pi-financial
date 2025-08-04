using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;

public interface IItchParserService
{
    Task<ItchMessage> Parse(byte[] bytes);
}