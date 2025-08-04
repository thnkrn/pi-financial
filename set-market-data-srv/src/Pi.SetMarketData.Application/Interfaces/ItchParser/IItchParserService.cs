using Pi.SetMarketData.Application.Services.Models.ItchParser;

namespace Pi.SetMarketData.Application.Interfaces.ItchParser;

public interface IItchParserService
{
    ItchMessage Parse(byte[] bytes);
}