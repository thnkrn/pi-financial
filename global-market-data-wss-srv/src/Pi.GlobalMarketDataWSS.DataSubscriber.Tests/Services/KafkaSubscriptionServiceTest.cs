using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Tests.Services;

public class KafkaSubscriptionServiceTest
{
    private readonly Mock<IConfiguration> _mockConfiguration;

    public KafkaSubscriptionServiceTest()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        SetupMockConfiguration();
    }

    private void SetupMockConfiguration()
    {
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns("3");

        _mockConfiguration
            .Setup(c => c.GetSection(ConfigurationKeys.KafkaSubscriptionServiceMaxRetryAttempts))
            .Returns(mockSection.Object);

        mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns("5000");

        _mockConfiguration
            .Setup(c => c.GetSection(ConfigurationKeys.KafkaSubscriptionServiceMaxRetryDelayMs))
            .Returns(mockSection.Object);

        mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns("1000");

        _mockConfiguration
            .Setup(c => c.GetSection(ConfigurationKeys.KafkaSubscriptionServiceInitialRetryDelayMs))
            .Returns(mockSection.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Call_SubscribeAsync()
    {
        // Arrange
        var mockSubscriber = new Mock<IKafkaSubscriber>();
        var logger = new Mock<ILogger<KafkaSubscriptionService<string, string>>>();
        var service =
            new KafkaSubscriptionService<string, string>(mockSubscriber.Object, _mockConfiguration.Object,
                logger.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await service.StartAsync(cancellationToken);

        // Assert
        mockSubscriber.Verify(s => s.SubscribeAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StopAsync_Should_Call_UnsubscribeAsync_And_Base_StopAsync()
    {
        // Arrange
        var mockSubscriber = new Mock<IKafkaSubscriber>();
        var logger = new Mock<ILogger<KafkaSubscriptionService<string, string>>>();
        var service =
            new KafkaSubscriptionService<string, string>(mockSubscriber.Object, _mockConfiguration.Object,
                logger.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await service.StopAsync(cancellationToken);

        // Assert
        mockSubscriber.Verify(s => s.UnsubscribeAsync(), Times.Once);
    }
}