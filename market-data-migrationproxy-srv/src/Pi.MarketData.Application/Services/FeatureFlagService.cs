using GrowthBook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Domain.ConstantConfigurations;

namespace Pi.MarketData.Application.Services;

public class FeatureFlagService : IFeatureFlagService
{
    private const string LogError = "Configuration key {Key} was not found.";
    private readonly IConfiguration _configuration;
    private readonly IGrowthBookFeatureRepository _featureRepository;
    private readonly bool _growthBookByPassIsActivated;
    private readonly ILogger<FeatureFlagService> _logger;
    private readonly GrowthBookConfigurationOptions _options;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    /// <param name="featureRepository"></param>
    public FeatureFlagService(
        IConfiguration configuration,
        IOptions<GrowthBookConfigurationOptions> options,
        ILogger<FeatureFlagService> logger,
        IGrowthBookFeatureRepository featureRepository
    )
    {
        _configuration = configuration;
        _logger = logger;
        _featureRepository = featureRepository;
        _options = options.Value;
        _growthBookByPassIsActivated = configuration.GetValue(ConfigurationKeys.GrowthBookByPassActivated, false);
        _logger.LogDebug("GrowthBookByPassActivated: {GrowthBookByPassActivated}", _growthBookByPassIsActivated);
    }

    public bool IsSetWebsocketProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassSetWebsocket, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookSetWebsocketProxyKey, string.Empty);

        // Failed safe mechanism, return false if there is no configuration key defined
        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookSetWebsocketProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsTfexWebsocketProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassTfexWebsocket, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookTfexWebsocketProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookTfexWebsocketProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsGeWebsocketProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassGeWebsocket, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookGeWebsocketProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookGeWebsocketProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsGenericHttpProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassGenericHttp, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookGenericHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookGenericHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsSetHttpProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassSetHttp, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookSetHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookSetHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsTfexHttpProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassTfexHttp, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookTfexHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookTfexHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsGeHttpProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassGeHttp, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookGeHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookGeHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsUserFavoriteProxyEnabled()
    {
        if (_growthBookByPassIsActivated)
            return _configuration.GetValue(ConfigurationKeys.GrowthBookByPassUserFavoriteHttp, true);

        var key = _configuration.GetValue(ConfigurationKeys.GrowthBookUserFavoriteHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(LogError, ConfigurationKeys.GrowthBookUserFavoriteHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    private async Task<bool> GetFeatureFlagByKeyAsync(string key)
    {
        var context = new Context
        {
            Enabled = true,
            ApiHost = _options.ApiHost,
            ClientKey = _options.ClientKey,
            FeatureRepository = _featureRepository
        };
        var growthBook = new GrowthBook.GrowthBook(context);
        await growthBook.LoadFeatures();
        return growthBook.IsOn(key);
    }
}