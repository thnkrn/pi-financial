using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.GlobalMarketData.DataMigrationConsumer.Constants;
using Pi.GlobalMarketData.DataMigrationConsumer.Interfaces;
using Pi.GlobalMarketData.DataMigrationConsumer.Services;
using Pi.GlobalMarketData.Domain.ConstantConfigurations;
using Pi.GlobalMarketData.Infrastructure.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Helpers;
using Pi.GlobalMarketData.Infrastructure.Interfaces.Kafka;

namespace Pi.GlobalMarketData.DataMigrationConsumer.Tests.Services;

public class ConsumerServiceTest
{
    private readonly Mock<IHttpClientFactory> _client;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<ConsumerService>> _mockLogger;
    private readonly Mock<IConsumerHelper> _mockConsumerHelper;
    private readonly Mock<IKafkaSubscriber<string, string>> _mockKafkaSubscriber;
    private readonly Mock<IVelexaHttpApiJwtTokenGenerator> _mockTokenGenerator;
    private readonly string _mockConsumerData =
        @"{
                                'Symbol': 'AAPL.NASDAQ',
                                'DateTimeFrom': '2015-01-22T00:00:00Z',
                                'DateTimeTo': '2015-01-28T23:59:59Z'
                            }";

    public ConsumerServiceTest()
    {
        _client = new Mock<IHttpClientFactory>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<ConsumerService>>();
        _mockConsumerHelper = new Mock<IConsumerHelper>();
        _mockTokenGenerator = new Mock<IVelexaHttpApiJwtTokenGenerator>();
        _mockKafkaSubscriber = new Mock<IKafkaSubscriber<string, string>>();

        var mockConfigSection = new Mock<IConfigurationSection>();

        _client.Setup(x => x.CreateClient(HttpClientKeys.VelexaHttpApi)).Returns(new HttpClient());

        mockConfigSection.Setup(x => x.Value).Returns("GEJobTopicName");
        _mockConfiguration
            .Setup(x => x.GetSection(ConfigurationKeys.KafkaMigrationJobTopicName))
            .Returns(mockConfigSection.Object);

        mockConfigSection.Setup(x => x.Value).Returns("GEDataTopicName");
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

}
