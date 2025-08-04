using Pi.Client.Sirius.Model;
using Pi.TfexService.Application.Queries.Market;

namespace Pi.TfexService.Application.Tests.Mock;

public class MockMarketData
{
    public static Ticker GenerateTicker(string symbol, string logo, string instrumentCategory)
    {
        return new Ticker
        {
            Symbol = symbol,
            Logo = logo,
            InstrumentCategory = instrumentCategory
        };
    }

    public static MarketData GenerateMarketDate(string symbol, string logo, string instrumentCategory)
    {
        return new MarketData
        (
            Series: symbol,
            InstrumentCategory: instrumentCategory,
            Logo: logo
        );
    }

}