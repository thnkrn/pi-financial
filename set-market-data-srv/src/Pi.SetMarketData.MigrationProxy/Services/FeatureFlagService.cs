using GrowthBook;
using Microsoft.Extensions.Options;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using Pi.SetMarketData.MigrationProxy.ConstantConfigurations;

namespace Pi.SetMarketData.MigrationProxy.Services;
public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FeatureFlagService> _logger;
    private readonly IGrowthBookFeatureRepository _featureRepository;
    private readonly GrowthBookConfigurationOptions _options;
    private readonly string _logError = "Configuration key {Key} was not found." ;

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

    }

    private async Task<bool> GetFeatureFlagByKeyAsync(string key)
    {
        var context = new Context()
        {
            Enabled = true,
            ApiHost = _options.ApiHost,
            ClientKey = _options.ClientKey,
            FeatureRepository = _featureRepository
        };
        var GB = new GrowthBook.GrowthBook(context);
        await GB.LoadFeatures();
        return GB.IsOn(key);
    }

    public bool IsSETWebsocketProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookSetWebsocketProxyKey, string.Empty);

        // Failed safe mechanism, return false if there is no configuration key defined
        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookSetWebsocketProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsTFEXWebsocketProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookTFEXWebsocketProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookTFEXWebsocketProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsGEWebsocketProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookGEWebsocketProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookGEWebsocketProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsGenericHttpProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookGenericHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookGenericHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsSETHttpProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookSetHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookSetHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsTFEXHttpProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookTFEXHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookTFEXHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }

    public bool IsGEHttpProxyEnabled()
    {
        var key = _configuration.GetValue<string>(ConfigurationKeys.GrowthBookGEHttpProxyKey, string.Empty);

        if (string.IsNullOrEmpty(key))
        {
            _logger.LogError(_logError, ConfigurationKeys.GrowthBookGEHttpProxyKey);
            return false;
        }

        var result = GetFeatureFlagByKeyAsync(key).GetAwaiter().GetResult();
        return result;
    }
}