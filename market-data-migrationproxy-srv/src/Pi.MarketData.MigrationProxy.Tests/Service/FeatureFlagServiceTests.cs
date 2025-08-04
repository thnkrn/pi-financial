using GrowthBook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.MarketData.Application.Services;
using Pi.MarketData.Domain.ConstantConfigurations;

namespace Pi.MarketData.MigrationProxy.Tests.Service;

public class FeatureFlagServiceTests
{
    private readonly FeatureFlagService _featureFlagService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IGrowthBookFeatureRepository> _mockFeatureRepository;

    public FeatureFlagServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        Mock<IOptions<GrowthBookConfigurationOptions>> mockOptions = new();
        Mock<ILogger<FeatureFlagService>> mockLogger = new();
        _mockFeatureRepository = new Mock<IGrowthBookFeatureRepository>();

        var options = new GrowthBookConfigurationOptions
        {
            ApiHost = "https://cdn.growthbook.io",
            ClientKey = "test-client-key"
        };
        mockOptions.Setup(x => x.Value).Returns(options);

        SetupConfigurationMock();

        _featureFlagService = new FeatureFlagService(
            _mockConfiguration.Object,
            mockOptions.Object,
            mockLogger.Object,
            _mockFeatureRepository.Object);
    }

    private void SetupConfigurationMock()
    {
        var configurationSection = new Mock<IConfigurationSection>();
        configurationSection.Setup(x => x.Value).Returns("true");

        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.GrowthBookByPassActivated))
            .Returns(configurationSection.Object);

        // Setup other required configuration values
        SetupConfigValue(ConfigurationKeys.GrowthBookByPassGeWebsocket, "true");
        SetupConfigValue(ConfigurationKeys.GrowthBookByPassTfexWebsocket, "true");
        SetupConfigValue(ConfigurationKeys.GrowthBookByPassSetWebsocket, "true");
    }

    private void SetupConfigValue(string key, string value)
    {
        var section = new Mock<IConfigurationSection>();
        section.Setup(x => x.Value).Returns(value);
        _mockConfiguration.Setup(x => x.GetSection(key)).Returns(section.Object);
    }

    [Fact]
    public void IsGEXProxyEnabled_ShouldReturnTrue_WhenFeatureIsOn()
    {
        // Arrange
        var key = "GEProxyEnabledKey";
        //Mock Configuration
        var geProxySection = new Mock<IConfigurationSection>();
        geProxySection.Setup(s => s.Value).Returns(key);

        _mockConfiguration.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookGeWebsocketProxyKey))
            .Returns(geProxySection.Object);
        var setProxyFeature = new Feature
        {
            DefaultValue = true,
            Rules = null
        };
        var features = new Dictionary<string, Feature>
        {
            { key, setProxyFeature }
        };
        _mockFeatureRepository.Setup(x => x.GetFeatures(It.IsAny<GrowthBookRetrievalOptions>(), null))
            .ReturnsAsync(features);

        // Act
        var result = _featureFlagService.IsGeWebsocketProxyEnabled();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTFEXProxyEnabled_ShouldReturnTrue_WhenFeatureIsOn()
    {
        // Arrange
        var key = "TFEXProxyEnabledKey";
        //Mock Configuration
        var tfexProxySection = new Mock<IConfigurationSection>();
        tfexProxySection.Setup(s => s.Value).Returns(key);

        _mockConfiguration.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookTfexWebsocketProxyKey))
            .Returns(tfexProxySection.Object);
        var setProxyFeature = new Feature
        {
            DefaultValue = true,
            Rules = null
        };
        var features = new Dictionary<string, Feature>
        {
            { key, setProxyFeature }
        };
        _mockFeatureRepository.Setup(x => x.GetFeatures(It.IsAny<GrowthBookRetrievalOptions>(), null))
            .ReturnsAsync(features);

        // Act
        var result = _featureFlagService.IsTfexWebsocketProxyEnabled();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSETProxyEnabled_ShouldReturnTrue_WhenFeatureIsOn()
    {
        // Arrange
        var key = "SETProxyEnabledKey";
        //Mock Configuration
        var setProxySection = new Mock<IConfigurationSection>();
        setProxySection.Setup(s => s.Value).Returns(key);

        _mockConfiguration.Setup(c => c.GetSection(ConfigurationKeys.GrowthBookSetWebsocketProxyKey))
            .Returns(setProxySection.Object);
        var setProxyFeature = new Feature
        {
            DefaultValue = true,
            Rules = null
        };
        var features = new Dictionary<string, Feature>
        {
            { key, setProxyFeature }
        };
        _mockFeatureRepository.Setup(x => x.GetFeatures(It.IsAny<GrowthBookRetrievalOptions>(), null))
            .ReturnsAsync(features);

        // Act
        var result = _featureFlagService.IsSetWebsocketProxyEnabled();

        // Assert
        Assert.True(result);
    }
}