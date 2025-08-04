using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.DataMigrationJobProducer.Services;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketData.DataMigrationJobProducer.Tests;

public class MigrationJobsProducerServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<MigrationJobsProducerService>> _mockLogger;
    private readonly Mock<IKafkaPublisher<string, string>> _mockPublisher;

    public MigrationJobsProducerServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<MigrationJobsProducerService>>();
        _mockPublisher = new Mock<IKafkaPublisher<string, string>>();
        SetupMockConfiguration();
        SetupMockLogger();
    }

    [Fact]
    public async Task ProduceMigrationJobsAsync_ShouldProduceCorrectNumberOfJobs()
    {
        // Arrange
        SetupMockPublisher();
        var service = new MigrationJobsProducerService(
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockPublisher.Object
        );

        var migrationDateFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var migrationDateTo = new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Unspecified);
        var stockSymbols = new[] { "CPALL", "SET50" };

        // Act
        await service.ProduceMigrationJobsAsync(migrationDateFrom, migrationDateTo, "Equity", stockSymbols);

        // Assert
        _mockPublisher.Verify(
            p => p.PublishAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>()),
            Times.Exactly(4) // 2 symbols * 2 weeks = 4 jobs
        );

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals("Total 4 jobs sent", o.ToString(), StringComparison.OrdinalIgnoreCase)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ProduceMigrationJobsAsync_ShouldHandleProduceException()
    {
        // Arrange
        var service = new MigrationJobsProducerService(
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockPublisher.Object
        );

        var migrationDateFrom = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var migrationDateTo = new DateTime(2024, 1, 7, 0, 0, 0, DateTimeKind.Unspecified);
        var stockSymbols = new[] { "CPALL" };

        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>()))
            .Throws(new ProduceException<string, string>(new Error(ErrorCode.Unknown), null));

        // Act
        await service.ProduceMigrationJobsAsync(migrationDateFrom, migrationDateTo, "Equity", stockSymbols);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => (o.ToString() ?? "").Contains("Delivery failed")),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => string.Equals("Total 0 jobs sent", o.ToString(), StringComparison.OrdinalIgnoreCase)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    private void SetupMockConfiguration()
    {
        var mockTopicConfigSection = new Mock<IConfigurationSection>();
        mockTopicConfigSection.Setup(s => s.Value).Returns("test-topic");

        var mockTopicChildren = new List<IConfigurationSection> { mockTopicConfigSection.Object };

        var mockKafkaTopicSection = new Mock<IConfigurationSection>();
        mockKafkaTopicSection.Setup(s => s.GetChildren()).Returns(mockTopicChildren);

        _mockConfiguration
            .Setup(c => c.GetSection(ConfigurationKeys.KafkaTopic))
            .Returns(mockKafkaTopicSection.Object);
    }

    private void SetupMockPublisher()
    {
        _mockPublisher
            .Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>())).Throws((string topic, Message<string, string> message) =>
                new DeliveryResult<string, string>
                {
                    Topic = topic,
                    Message = message,
                    TopicPartitionOffset = new TopicPartitionOffset(topic, 0, 0)
                });
    }

    private void SetupMockLogger()
    {
        _mockLogger
            .Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)));
    }
}