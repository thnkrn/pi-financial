using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketData.Infrastructure.Interfaces.Helpers;
using Pi.SetMarketData.DataMigrationConsumer.Constants;
using Pi.SetMarketData.DataMigrationConsumer.Handlers;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.SetMarketData.DataMigrationConsumer.Tests.Services;

public class ConsumerServiceTest
{
    private readonly Mock<IHttpClientFactory> _client;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<KafkaMessageHandler>> _mockLogger;
    private readonly Mock<IConsumerHelper> _mockConsumerHelper;
    private readonly Mock<IKafkaPublisher<string, string>> _mockPublisher;
    private readonly string _mockConsumerData =
        @"{
            'Symbol': 'CPALL',
            'DateTimeFrom': '2015-01-22T00:00:00Z',
            'DateTimeTo': '2015-01-28T23:59:59Z'
        }";

    public ConsumerServiceTest()
    {
        _client = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<KafkaMessageHandler>>();
        _mockConsumerHelper = new Mock<IConsumerHelper>();
        _mockPublisher = new Mock<IKafkaPublisher<string, string>>();

        var mockConfigSection = new Mock<IConfigurationSection>();

        _client.Setup(x => x.CreateClient(HttpClientKeys.Sirius)).Returns(new HttpClient());

        mockConfigSection.Setup(x => x.Value).Returns("SETJobTopicName");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaMigrationJobTopicName))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns("SETDataTopicName");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaMigrationDataTopicName))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns("KafkaBootstrapServers");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaBootstrapServers))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns("KraftConsumerGroupId");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaConsumerGroupId))
            .Returns(mockConfigSection.Object);
    }

    [Fact]
    public async Task ConsumerService_ShouldRunSuccessfully()
    {
        // Arrange
        var cancellationToken = new CancellationToken(true);

        var _mockConsumer = new Mock<IConsumer<Ignore, string>>();
        var _mockProducer = new Mock<IProducer<string, string>>();

        _mockConsumer
            .Setup(x => x.Consume(cancellationToken))
            .Returns(
                new ConsumeResult<Ignore, string>()
                {
                    Message = new Message<Ignore, string> { Value = _mockConsumerData }
                }
            );

        _mockConsumerHelper
            .Setup(x => x.GetConsumer(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_mockConsumer.Object);

        _mockConsumerHelper
            .Setup(x => x.GetProducer(It.IsAny<string>()))
            .Returns(_mockProducer.Object);

        var _handler = new KafkaMessageHandler(
            _client.Object,
            _mockConfiguration.Object,
            _mockPublisher.Object, // Pass publisher mock
            _mockLogger.Object
        );

        // Act
        await _handler.HandleAsync(new ConsumeResult<string, string>
        {
            Message = new Message<string, string> { Value = _mockConsumerData }
        });

        // Assert
        _client.Verify();
        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => (o.ToString() ?? "").Contains("Migration Job")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)
                ),
            Times.Once
        );
        _mockPublisher.Verify(p => p.PublishAsync(It.IsAny<string>(), It.IsAny<Message<string, string>>()), Times.Once);
    }
}
