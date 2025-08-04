using Pi.SetMarketDataWSS.Application.Services.Models.ItchOrderBookMapper;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Interfaces.OrderBookMapper;

public interface IOrderBookMapper
{
    OrderBook Map(MarketByPriceLevelWrapper message, OrderBook? orderBook);
}