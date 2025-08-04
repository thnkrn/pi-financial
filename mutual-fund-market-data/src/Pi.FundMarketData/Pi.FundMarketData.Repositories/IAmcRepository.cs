using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public interface IAmcRepository
{
    Task<AmcProfile> GetAmcProfile(string amcCode, CancellationToken ct);
    Task<Dictionary<string, AmcProfile>> GetAmcProfiles(CancellationToken ct);
}
