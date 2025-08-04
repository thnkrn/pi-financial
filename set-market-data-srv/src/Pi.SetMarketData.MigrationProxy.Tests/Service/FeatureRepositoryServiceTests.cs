using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Pi.SetMarketData.MigrationProxy.Models;
using Pi.SetMarketData.MigrationProxy.Services;
using System.Collections.Generic;
using GrowthBook;

namespace Pi.SetMarketData.MigrationProxy.Tests.Services;
public class FeatureRepositoryServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<ILogger<FeatureRepositoryService>> _mockLogger;
    private readonly Mock<IOptions<GrowthBookConfigurationOptions>> _mockOptions;
    private readonly Mock<IGrowthBookFeatureCache> _mockFeatureCache;
    private readonly FeatureRepositoryService _service;

    public FeatureRepositoryServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockLogger = new Mock<ILogger<FeatureRepositoryService>>();
        _mockOptions = new Mock<IOptions<GrowthBookConfigurationOptions>>();
        _mockFeatureCache = new Mock<IGrowthBookFeatureCache>();

        var options = new GrowthBookConfigurationOptions
        {
            ApiHost = "https://example.com",
            ClientKey = "test-client-key"
        };
        _mockOptions.Setup(o => o.Value).Returns(options);

        _service = new FeatureRepositoryService(
            _mockHttpClientFactory.Object,
            _mockLogger.Object,
            _mockOptions.Object,
            _mockFeatureCache.Object
        );
    }

    [Fact]
    public async Task GetFeatures_ReturnsCachedFeatures_WhenCacheIsNotExpired()
    {
        // Arrange
        _mockFeatureCache.Setup(c => c.IsCacheExpired).Returns(false);

        var feature = new Feature{
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