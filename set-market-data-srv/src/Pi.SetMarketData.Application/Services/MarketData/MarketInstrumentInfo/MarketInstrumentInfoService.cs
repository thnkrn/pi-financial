using Pi.SetMarketData.Application.Services.MarketData.MarketProfileOverview;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketInstrumentInfo;

public static class MarketInstrumentInfoService
{
    public static MarketInstrumentInfoResponse GetResult(TradingSign tradingSign)
    {
        TradingSignsMapper _tradingSignsMapper = new TradingSignsMapper();

        // Mapping the rest values for API response
        return new MarketInstrumentInfoResponse
        {
            Code = "0",
            Message = "",
            Response = new InstrumentInfoResponse
            {
                SpreadSize = "0",
                AmountStepSize = "0",
                MinimumPurchaseAmount = "0",
                MinimumPrice = "0",
                IsNew = false,
                TradingSign = _tradingSignsMapper.MapTradingSigns(tradingSign),
            }
        };
    }
}
