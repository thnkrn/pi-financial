using AutoFixture;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Handlers.Tests;
public class KafkaMessageHandlerTest
{
    private readonly Mock<ILogger<KafkaMessageHandler>> _mockLogger;
    private readonly Mock<IKafkaPublisher<string, string>> _mockKafkaPublisher;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
    private readonly Mock<IVelexaHttpApiJwtTokenGenerator> _mockVelexaHttpApiJwtTokenGenerator;
    private readonly KafkaMessageHandler _handler;

    public KafkaMessageHandlerTest()
    {
        _mockLogger = new Mock<ILogger<KafkaMessageHandler>>();
        _mockKafkaPublisher = new Mock<IKafkaPublisher<string, string>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _mockVelexaHttpApiJwtTokenGenerator = new Mock<IVelexaHttpApiJwtTokenGenerator>();

        var mockedTopic = "mocked-topic";
        _mockConfiguration.Setup(x => x[ConfigurationKeys.KafkaMigrationDataTopicName])
        .Returns(mockedTopic);

        _handler = new KafkaMessageHandler(
            _mockLogger.Object,
            _mockKafkaPublisher.Object,
            _mockConfiguration.Object,
            _mockHttpClientFactory.Object,
            _mockVelexaHttpApiJwtTokenGenerator.Object
        );


        
    }
    private readonly string _mockMessageValue =
    @"{
            'Symbol': 'AAPL.NASDAQ',
            'DateTimeFrom': '2015-01-22T00:00:00Z',
            'DateTimeTo': '2015-01-28T23:59:59Z'
        }";

    [Fact]
    public void HandleAsync_CompletelyConsumeMessage()
    {
        // Arrange
        var msg = "{\"Key\":\"MSFT\",\"Value\":\"{\\\"Symbol\\\":\\\"MSFT\\\",\\\"Venue\\\":\\\"NASDAQ\\\",\\\"DateTimeFrom\\\":\\\"2015-01-08T07:00:00+07:00\\\",\\\"DateTimeTo\\\":\\\"2015-01-15T06:59:59+07:00\\\"}\",\"Timestamp\":{\"Type\":0,\"UnixTimestampMs\":0,\"UtcDateTime\":\"1970-01-01T00:00:00Z\"},\"Headers\":null}";
        
        var consumeResult = new ConsumeResult<string, string>
        {
            Message = new Message<string, string>
            {
                Value = msg
            }
        };
        
        // Act
        _handler.HandleAsync(consumeResult);

        // Assert
        _mockVelexaHttpApiJwtTokenGenerator.Verify(x => x.GenerateJwtToken(It.IsAny<int>()), Times.Once());
        _mockLogger.Verify(
            x =>
                x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (o, t) => o.ToString().Contains("Consumed message")
                    ),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
            Times.Once
        );
    }
}