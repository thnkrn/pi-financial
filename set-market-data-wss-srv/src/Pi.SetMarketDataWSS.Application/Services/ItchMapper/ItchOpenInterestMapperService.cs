using Pi.SetMarketDataWSS.Application.Interfaces;
using Pi.SetMarketDataWSS.Application.Services.Models;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchMessageWrapper;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Services.ItchMapper;
public class ItchOpenInterestmapperService : IItchOpenInterestMapperService
{
    public void Map(ItchMessage message, ref OpenInterest cachedOpenInterest)
    {
        var openInterestWrapper = (OpenInterestMessageWrapper)message;
        cachedOpenInterest.POI = openInterestWrapper.OpenInterest.Value;
        cachedOpenInterest.OrderBookId = openInterestWrapper.OrderBookId.Value;
    }
}