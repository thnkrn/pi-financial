using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;

namespace Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;

public interface IItchPublicTradeMapper
{
    PublicTradeResult Map(TradeTickerMessageWrapper message, PublicTrade[]? cachePublicTrade);
}