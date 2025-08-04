using Microsoft.Extensions.Options;
using Pi.GlobalEquities.Worker.Configs;
using Pi.GlobalEquities.Worker.ExternalServices.Notification;
using FeatureServiceBase = Pi.Common.Features.FeatureService;
using IFeatureServiceBase = Pi.Common.Features.IFeatureService;

namespace Pi.GlobalEquities.Worker.ExternalServices.FeatureFlags;

public class FeatureService : IFeatureService
{
    private readonly HttpClient _client;
    private readonly IOptions<FeatureApiConfig> _featureOptions;
    private FeatureApiConfig _featureApiConfig => _featureOptions.Value;
    private readonly ILogger<FeatureServiceBase> _logger;
    private readonly IFeatureServiceBase _featureService;

    public FeatureService(HttpClient client, IOptions<FeatureApiConfig> featureOptions, ILogger<FeatureServiceBase> logger)
    {
        _client = client;
        _featureOptions = featureOptions;
        _logger = logger;
        _featureService = new FeatureServiceBase(
            httpClient: _client,
            logger: _logger,
            apiKey: _featureApiConfig.ApiKey,
            projectId: _featureApiConfig.ProjectId,
            baseUrl: _featureApiConfig.Host);
    }

    public bool IsOn(string key)
    {
        return _featureService.IsOn(key);
    }

    public bool IsOn(string userId, string key)
    {
        var userArray = new[] { userId };
        var attributes = new Dictionary<string, string[]> { { "user-id", userArray } };
        var feature = new FeatureServiceBase(
            httpClient: _client,
            logger: _logger,
            apiKey: _featureApiConfig.ApiKey,
            projectId: _featureApiConfig.ProjectId,
            baseUrl: _featureApiConfig.Host,
            attributes: FeatureServiceBase.GetAttributes(attributes));

        var isOn = feature.IsOn(key);
        return isOn;
    }
}
