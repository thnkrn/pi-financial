using System.Collections.Concurrent;
using GrowthBook;

namespace Pi.MarketData.Application.Services;

public class FeatureCacheService : IGrowthBookFeatureCache
{
    private ConcurrentDictionary<string, Feature> _cachedFeatures = new();

    public int FeatureCount => _cachedFeatures.Count;

    public bool IsCacheExpired => FeatureCount == 0;

    public Task<IDictionary<string, Feature>> GetFeatures(CancellationToken? cancellationToken = null)
    {
        return Task.FromResult<IDictionary<string, Feature>>(_cachedFeatures);
    }

    public Task RefreshWith(IDictionary<string, Feature> features, CancellationToken? cancellationToken = null)
    {
        _cachedFeatures = new ConcurrentDictionary<string, Feature>(features);

        return Task.CompletedTask;
    }
}