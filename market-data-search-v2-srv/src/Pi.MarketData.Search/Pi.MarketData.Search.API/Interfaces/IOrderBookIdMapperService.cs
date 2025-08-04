using Pi.MarketData.Search.Domain.Models;

namespace Pi.MarketData.Search.API.Interfaces;

public interface IOrderBookIdMapperService
{
    Task<Tuple<List<string>, List<string>, Dictionary<string, string>>> MapCacheKeysFromSymbols(UserInstrumentFavoriteResponse response);
}