using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Models.Redis;

namespace Pi.SetMarketDataWSS.Application.Interfaces.ItchMapper;

public interface IItchMapperService
{
    RedisValueResult? MapToDataCache(ItchMessage message, Dictionary<string, string> currentCacheValue);
}