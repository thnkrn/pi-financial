using GrowthBook;
using System.Collections.Concurrent;

namespace Pi.SetMarketData.MigrationProxy.Services;

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
        _cachedFeatures = new(features);        

        return Task.CompletedTask;
    }
}