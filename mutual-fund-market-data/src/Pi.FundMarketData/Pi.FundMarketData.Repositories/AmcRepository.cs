using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public class AmcRepository : IAmcRepository
{
    private MongoDbConfig _config;
    private IMongoClient _client;
    private ICache _cache;

    public AmcRepository(IMongoClient client, ICache cache, IOptions<MongoDbConfig> options)
    {
        _config = options.Value ?? throw new ArgumentNullException(nameof(options));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<AmcProfile> GetAmcProfile(string amcCode, CancellationToken ct)
    {
        if (!_cache.TryGetAmcProfile(amcCode, out var amcProfile))
        {
            var amcProfiles = await GetProfiles(ct);
            if (!amcProfiles.TryGetValue(amcCode, out amcProfile)) return null;
        }

        return amcProfile;
    }

    public async Task<Dictionary<string, AmcProfile>> GetAmcProfiles(CancellationToken ct)
    {
        if (!_cache.TryGetAmcProfiles(out var amcProfiles))
            amcProfiles = await GetProfiles(ct);

        return amcProfiles;
    }

    private async Task<Dictionary<string, AmcProfile>> GetProfiles(CancellationToken ct)
    {
        var db = _client.GetDatabase(_config.Database);
        var col = db.GetCollection<AmcProfile>(MongoDbConfig.AmcProfilesColName);
        var amcProfilesData = await col.AsQueryable().ToListAsync(ct);
        var amcProfiles = amcProfilesData.ToDictionary(x => x.Code);
        _cache.AddAmcProfiles(amcProfiles);

        return amcProfiles;
    }
}
