using GrowthBook;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.Application.Services;

public class FeatureRepositoryService : IGrowthBookFeatureRepository
{
    private readonly IGrowthBookFeatureCache _featureCache;
    private readonly HttpClient _httpClient;
    private readonly ILogger<FeatureRepositoryService> _logger;
    private readonly GrowthBookConfigurationOptions _options;

    /// <summary>
    /// </summary>
    /// <param name="clientFactory"></param>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <param name="featureCache"></param>
    public FeatureRepositoryService(
        IHttpClientFactory clientFactory,
        ILogger<FeatureRepositoryService> logger,
        IOptions<GrowthBookConfigurationOptions> options,
        IGrowthBookFeatureCache featureCache
    )
    {
        _httpClient = clientFactory.CreateClient("HttpClient");
        _options = options.Value;
        _logger = logger;
        _featureCache = featureCache;
    }

    /// <inheritdoc />
    public void Cancel()
    {
        throw new InvalidOperationException("The implementation does not allow cancelling a repository action");
    }

    /// <inheritdoc />
    public async Task<IDictionary<string, Feature>?> GetFeatures(GrowthBookRetrievalOptions? options = null,
        CancellationToken? cancellationToken = null)
    {
        _logger.LogDebug("Getting features from repository.");

        if (!_featureCache.IsCacheExpired) return await _featureCache.GetFeatures(CancellationToken.None);

        var url = $"{_options.ApiHost}/api/features/{_options.ClientKey}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode) return await _featureCache.GetFeatures(CancellationToken.None);

        var content = await response.Content.ReadAsStringAsync();
        var featureResult = JsonConvert.DeserializeObject<FeaturesResult>(content);
        var features = featureResult?.Features;

        await _featureCache.RefreshWith(features);

        return features;
    }
}