using Pi.SetMarketData.Application.Models.ItchMessageWrapper;
using Pi.SetMarketData.Application.Services.Models.ItchParser;

namespace Pi.SetMarketData.Application.Interfaces.ItchMapper;

public interface IItchOrderBookMapperService
{
    public Domain.Entities.OrderBook? Map(MarketByPriceLevelWrapper? message, Domain.Entities.OrderBook? storedData);
}