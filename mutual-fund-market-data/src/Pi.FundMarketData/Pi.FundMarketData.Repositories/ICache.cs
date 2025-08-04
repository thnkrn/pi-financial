using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public interface ICache
{
    bool TryGetAmcProfile(string amcCode, out AmcProfile value);
    bool TryGetAmcProfiles(out Dictionary<string, AmcProfile> value);
    void AddAmcProfiles(Dictionary<string, AmcProfile> value);
}
