using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Features;
using Pi.SetMarketData.MigrationProxy.Services;
using GrowthBook;
using Pi.SetMarketData.MigrationProxy.ConstantConfigurations;
using Pi.SetMarketData.MigrationProxy.Interfaces;
using System.Threading.Tasks;

namespace Pi.SetMarketData.MigrationProxy.Tests;
public class FeatureFlagServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IOptions<GrowthBookConfigurationOptions>> _mockOptions;
    private readonly Mock<ILogger<FeatureFlagService>> _mockLogger;
    private readonly Mock<IGrowthBookFeatureRepository> _mockFeatureRepository;
    private readonly FeatureFlagService _featureFlagService;
    private readonly GrowthBookConfigurationOptions _options;
    public FeatureFlagServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockOptions = new Mock<IOptions<GrowthBookConfigurationOptions>>();
        _mockLogger = new Mock<ILogger<FeatureFlagService>>();
        _mockFeatureRepository = new Mock<IGrowthBookFeatureRepository>();

        _options = new GrowthBookConfigurationOptions
        {
            ApiHost = "https://cdn.growthbook.io",
            ClientKey = "test-client-key"
        };
        _mockOptions.Setup(x => x.Value).Returns(_options);

        _featureFlagService = new FeatureFlagService(
            _mockConfiguration.Object,
            _mockOptions.Object,
            _mockLogger.Object,
            _mockFeatureRepository.Object);
        
    }

    [Fact]
    public void IsGEXProxyEnabled_ShouldReturnTrue_WhenFeatureIsOn()
    {
        // Arrange
        var key = "GEProxyEnabledKey";
        //Mock Configuration
        var GEProxySection = new Mock<IConfigurationSection>();
            GEProxySection.Setup(s => s.Value).Returns(key);
            
            _mockConfiguration.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookGEWebsocketProxyKey)).Returns(GEProxySection.Object);
        var setProxyFeature = new Feature{
            DefaultValue = true,
            Rules = null
        };
        var features = new Dictionary<string, Feature>()
        {
            {key, setProxyFeature}
        };
        _mockFeatureRepository.Setup(x => x.GetFeatures(It.IsAny<GrowthBook.GrowthBookRetrievalOptions>(), null)).ReturnsAsync(features);

        // Act
        var result = _featureFlagService.IsGEWebsocketProxyEnabled();

        // Assert
        Assert.True(result);
    }
    [Fact]
    public void IsTFEXProxyEnabled_ShouldReturnTrue_WhenFeatureIsOn()
    {
        // Arrange
        var key = "TFEXProxyEnabledKey";
        //Mock Configuration
        var TFEXProxySection = new Mock<IConfigurationSection>();
            TFEXProxySection.Setup(s => s.Value).Returns(key);
            
            _mockConfiguration.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookTFEXWebsocketProxyKey)).Returns(TFEXProxySection.Object);
        var setProxyFeature = new Feature{
            DefaultValue = true,
            Rules = null
        };
        var features = new Dictionary<string, Feature>()
        {
            {key, setProxyFeature}
        };
        _mockFeatureRepository.Setup(x => x.GetFeatures(It.IsAny<GrowthBook.GrowthBookRetrievalOptions>(), null)).ReturnsAsync(features);

        // Act
        var result = _featureFlagService.IsTFEXWebsocketProxyEnabled();

        // Assert
        Assert.True(result);
    }
    [Fact]
    public void IsSETProxyEnabled_ShouldReturnTrue_WhenFeatureIsOn()
    {
        // Arrange
        var key = "SETProxyEnabledKey";
        //Mock Configuration
        var SetProxySection = new Mock<IConfigurationSection>();
            SetProxySection.Setup(s => s.Value).Returns(key);
            
            _mockConfiguration.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookSetWebsocketProxyKey)).Returns(SetProxySection.Object);
        var setProxyFeature = new Feature{
            DefaultValue = true,
            Rules = null
        };
        var features = new Dictionary<string, Feature>()
        {
            {key, setProxyFeature}
        };
        _mockFeatureRepository.Setup(x => x.GetFeatures(It.IsAny<GrowthBook.GrowthBookRetrievalOptions>(), null)).ReturnsAsync(features);

        // Act
        var result = _featureFlagService.IsSETWebsocketProxyEnabled();

        // Assert
        Assert.True(result);
    }
}