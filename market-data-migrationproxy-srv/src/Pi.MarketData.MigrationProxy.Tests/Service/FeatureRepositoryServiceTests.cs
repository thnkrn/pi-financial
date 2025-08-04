using GrowthBook;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.MarketData.Application.Services;

namespace Pi.MarketData.MigrationProxy.Tests.Service;

public class FeatureRepositoryServiceTests
{
    private readonly Mock<IGrowthBookFeatureCache> _mockFeatureCache;
    private readonly FeatureRepositoryService _service;

    public FeatureRepositoryServiceTests()
    {
        Mock<IHttpClientFactory> mockHttpClientFactory = new();
        Mock<ILogger<FeatureRepositoryService>> mockLogger = new();
        Mock<IOptions<GrowthBookConfigurationOptions>> mockOptions = new();
        _mockFeatureCache = new Mock<IGrowthBookFeatureCache>();

        var options = new GrowthBookConfigurationOptions
        {
            ApiHost = "https://example.com",
            ClientKey = "test-client-key"
        };
        mockOptions.Setup(o => o.Value).Returns(options);

        _service = new FeatureRepositoryService(
            mockHttpClientFactory.Object,
            mockLogger.Object,
            mockOptions.Object,
            _mockFeatureCache.Object
        );
    }

    [Fact]
    public async Task GetFeatures_ReturnsCachedFeatures_WhenCacheIsNotExpired()
    {
        // Arrange
        _mockFeatureCache.Setup(c => c.IsCacheExpired).Returns(false);

        var feature = new Feature
        {
            DefaultValue = true,
            Rules = null
        };
        var cachedFeatures = new Dictionary<string, Feature>
        {
            { "feature1", feature }
        };
        _mockFeatureCache.Setup(c => c.GetFeatures(It.IsAny<CancellationToken>())).ReturnsAsync(cachedFeatures);

        // Act
        var result = await _service.GetFeatures();

        // Assert
        Assert.Equal(cachedFeatures, result);
    }
}