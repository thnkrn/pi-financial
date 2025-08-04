using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Entities;

namespace Pi.SetMarketDataWSS.Application.Interfaces;
public interface IItchOpenInterestMapperService {
    public void Map(ItchMessage message, ref OpenInterest cachedOpenInterest);
}