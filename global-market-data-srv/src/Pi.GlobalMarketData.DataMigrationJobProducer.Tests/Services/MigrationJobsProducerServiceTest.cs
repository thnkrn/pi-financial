using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.DataMigrationJobProducer.Services;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.DataMigrationJobProducer.Tests;

public class MigrationJobsProducerServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<MigrationJobsProducerService>> _mockLogger;
    private readonly Mock<IKafkaPublisher<string, string>> _mockKafkaPublisher;

    public MigrationJobsProducerServiceTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<MigrationJobsProducerService>>();
        _mockKafkaPublisher = new Mock<IKafkaPublisher<string, string>>();
        SetupMockConfiguration();
        SetupMockLogger();
    }

    [Fact]
    public async Task ProduceMigrationJobsAsync_ShouldProduceCorrectNumberOfJobs()
    {
        // Arrange
        var service = new MigrationJobsProducerService(
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockKafkaPublisher.Object
        );

        var migrationDateFrom = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var migrationDateTo = new DateTime(2024, 1, 13, 0, 0, 0, DateTimeKind.Unspecified);
        var stockSymbols = new[] { "CPALL", "SET50" };
        var venue = "NASDAQ";

        // Act
        await service.ProduceMigrationJobsAsync(migrationDateFrom, migrationDateTo, venue, stockSymbols);

        // Assert
        _mockKafkaPublisher.Verify(
           kp => kp.PublishAsync(
               It.IsAny<string>(), // kafka topic
               It.IsAny<Message<string, string>>()
           ),
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
        _mockKafkaPublisher
            .Setup(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>()))
            .ThrowsAsync(new ProduceException<string, string>(new Error(ErrorCode.Unknown), null));

        var service = new MigrationJobsProducerService(
            _mockConfiguration.Object,
            _mockLogger.Object,
            _mockKafkaPublisher.Object
        );

        var migrationDateFrom = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        var migrationDateTo = new DateTime(2024, 1, 7, 0, 0, 0, DateTimeKind.Unspecified);
        var stockSymbols = new[] { "CPALL" };
        var venue = "NASDAQ";

        // Act
        await service.ProduceMigrationJobsAsync(migrationDateFrom, migrationDateTo, venue, stockSymbols);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
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
        _mockConfiguration
            .Setup(c => c[ConfigurationKeys.KafkaTopic])
            .Returns("test-topic");

        _mockConfiguration
            .Setup(c => c[ConfigurationKeys.KafkaBootstrapServers])
            .Returns("localhost:9092");
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