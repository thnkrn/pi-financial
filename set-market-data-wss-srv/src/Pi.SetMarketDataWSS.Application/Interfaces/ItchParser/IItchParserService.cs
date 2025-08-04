using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Interfaces.ItchParser;

public interface IItchParserService
{
    ItchMessage Parse(byte[] bytes);
}