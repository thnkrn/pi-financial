using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Domain.Models.Redis;

namespace Pi.SetMarketDataWSS.Application.Interfaces.ItchHousekeeper;

public interface IItchHousekeeperService
{
    public RedisValueResult? ResetStat(ItchMessage message, IReadOnlyDictionary<string, string> currentCacheValue);
}