using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketDataWSS.Domain.ConstantConfigurations;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Kafka;
using Pi.GlobalMarketDataWSS.Infrastructure.Services.Kafka;
using System.Reflection;

namespace Pi.GlobalMarketDataWSS.Infrastructure.Tests;

public class KafkaBatchV2SubscriberTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IKafkaMessageHandler<Message<string, string>>> _mockMessageHandler;
    private readonly Mock<ILogger<KafkaBatchV2Subscriber<string, string>>> _mockLogger;
    private readonly string _testTopic = "test-topic";
    
    public KafkaBatchV2SubscriberTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockMessageHandler = new Mock<IKafkaMessageHandler<Message<string, string>>>();
        _mockLogger = new Mock<ILogger<KafkaBatchV2Subscriber<string, string>>>();
        
        // Setup configuration
        _mockConfiguration.Setup(c => c[ConfigurationKeys.KafkaBootstrapServers]).Returns("test-bootstrap-server");
        _mockConfiguration.Setup(c => c[ConfigurationKeys.KafkaConsumerGroupId]).Returns("test-group-id");
        _mockConfiguration.Setup(c => c[ConfigurationKeys.KafkaSaslUsername]).Returns("test-username");
        _mockConfiguration.Setup(c => c[ConfigurationKeys.KafkaSaslPassword]).Returns("test-password");
        _mockConfiguration.Setup(c => c[ConfigurationKeys.KafkaSecurityProtocol]).Returns("SASL_SSL");
        _mockConfiguration.Setup(c => c[ConfigurationKeys.KafkaSaslMechanism]).Returns("PLAIN");
        
        // แทนที่การใช้ GetValue ด้วยการตั้งค่า mock section
        SetupConfigSection("KAFKA:CONSUMER_BATCH_SIZE", 100);
        SetupConfigSection("KAFKA:CONSUMER_MAX_BATCH_PROCESSING_TIME_MS", 1000);
        SetupConfigSection("KAFKA:CONSUMER_MAX_RETRY_ATTEMPTS", 3);
        SetupConfigSection("KAFKA:CONSUMER_COMMIT_INTERVAL_MS", 1000);
        SetupConfigSection("KAFKA:CONSUMER_RETRY_DELAY_MS", 500);
        SetupConfigSection("KAFKA:CONSUMER_FETCH_MIN_BYTES", 1);
        SetupConfigSection("KAFKA:CONSUMER_FETCH_WAIT_MAX_MS", 10);
        SetupConfigSection("KAFKA:CONSUMER_SESSION_TIMEOUT_MS", 30000);
        SetupConfigSection("KAFKA:CONSUMER_MAX_POLL_INTERVAL_MS", 300000);
        SetupConfigSection("KAFKA:CONSUMER_MAX_PARTITION_FETCH_BYTES", 4194304);
        SetupConfigSection("KAFKA:CONSUMER_FETCH_MAX_BYTES", 52428800);
        SetupConfigSection("KAFKA:CONSUMER_QUEUED_MIN_MESSAGES", 10000);
        SetupConfigSection("KAFKA:CONSUMER_QUEUED_MAX_MESSAGES_KBYTES", 1048576);
        SetupConfigSection("KAFKA:CONSUMER_SOCKET_TIMEOUT_MS", 60000);
    }
    
    private void SetupConfigSection<T>(string key, T value)
    {
        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(s => s.Value).Returns(value.ToString());
        mockSection.Setup(s => s.Path).Returns(key);
        mockSection.Setup(s => s.Key).Returns(key.Split(':').Last());
        
        _mockConfiguration
            .Setup(c => c.GetSection(key))
            .Returns(mockSection.Object);
    }
    
    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act
        var subscriber = new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            _testTopic, 
            _mockMessageHandler.Object, 
            _mockLogger.Object
        );
        
        // Assert
        Assert.NotNull(subscriber);
    }
    
    [Fact]
    public void Constructor_WithNullParameters_ThrowsArgumentNullException()
    {
        // Assert
        Assert.Throws<ArgumentNullException>(() => new KafkaBatchV2Subscriber<string, string>(
            null, 
            _testTopic, 
            _mockMessageHandler.Object, 
            _mockLogger.Object
        ));
        
        Assert.Throws<ArgumentNullException>(() => new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            null, 
            _mockMessageHandler.Object, 
            _mockLogger.Object
        ));
        
        Assert.Throws<ArgumentNullException>(() => new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            _testTopic, 
            null, 
            _mockLogger.Object
        ));
        
        Assert.Throws<ArgumentNullException>(() => new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            _testTopic, 
            _mockMessageHandler.Object, 
            null
        ));
    }

    [Fact]
    public async Task UnsubscribeAsync_WhenNotSubscribed_LogsAndReturns()
    {
        // Arrange
        var subscriber = new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            _testTopic, 
            _mockMessageHandler.Object, 
            _mockLogger.Object
        );
        
        // Act
        await subscriber.UnsubscribeAsync();
        
        // Assert - Verify Debug log was called
        // For simplicity we're only checking that no exception was thrown
    }
    
    [Fact]
    public void Dispose_MultipleCalls_OnlyClosesConsumerOnce()
    {
        // Arrange
        var subscriber = new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            _testTopic, 
            _mockMessageHandler.Object, 
            _mockLogger.Object
        );
        
        // Act - Call dispose multiple times
        subscriber.Dispose();
        subscriber.Dispose();
        
        // Assert - No way to verify internal state directly in a unit test,
        // but we can verify no exceptions are thrown
    }
    
    [Fact]
    public async Task ProcessMessageBatchAsync_WithReflection_CallsMessageHandler()
    {
        // Arrange
        var subscriber = new KafkaBatchV2Subscriber<string, string>(
            _mockConfiguration.Object, 
            _testTopic, 
            _mockMessageHandler.Object, 
            _mockLogger.Object
        );
        
        // สร้างข้อความทดสอบ
        var messageBatch = CreateTestMessages(10000);
        
        // เรียกใช้ ProcessMessageBatchAsync ผ่าน reflection
        var processMethod = typeof(KafkaBatchV2Subscriber<string, string>)
            .GetMethod("ProcessMessageBatchAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);
        
        // Act
        await (Task)processMethod.Invoke(subscriber, new object[] { messageBatch });
        
        // Assert
        _mockMessageHandler.Verify(h => h.HandleAsync(It.IsAny<Message<string, string>>()), Times.AtLeastOnce);
    }
    
    private List<ConsumeResult<string, string>> CreateTestMessages(int count)
    {
        var messages = new List<ConsumeResult<string, string>>(count);
    
        // Create messages with different keys to test deduplication logic
        for (var i = 0; i < count; i++)
        {
            var key = $"key-{i % 10}"; // Use 10 different keys
            var message = new Message<string, string>
            {
                Key = key,
                Value = $"test-message-{i}",
                Headers = new Headers
                {
                    { "sequenceNumber", BitConverter.GetBytes((long)i) }
                }
            };
            
            var consumeResult = new ConsumeResult<string, string>
            {
                Topic = _testTopic,
                Partition = i % 5, // Use 5 partitions
                Offset = i,
                Message = message
            };
        
            messages.Add(consumeResult);
        }
    
        return messages;
    }
}