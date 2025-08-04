using System.Linq.Expressions;
using AutoFixture;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Moq;
using Pi.GlobalMarketDataWSS.Application.Constants;
using Pi.GlobalMarketDataWSS.Application.Interfaces.FixMapper;
using Pi.GlobalMarketDataWSS.DataSubscriber.Handlers;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.Entities;
using Pi.GlobalMarketDataWSS.Domain.Models.Fix;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Mongo;
using Pi.GlobalMarketDataWSS.Infrastructure.Interfaces.Redis;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Tests.Handlers;

public class KafkaMessageHandlerTest
{
    private readonly Mock<ICacheService> _cacheService;
    private readonly Mock<IConfiguration> _configuration;
    private readonly KafkaMessageHandler _kafkaMessageHandler;
    private readonly Mock<ILogger<KafkaMessageHandler>> _logger;
    private readonly Mock<IMongoService<MarketSchedule>> _mongoService;
    private readonly Mock<IMongoService<GeInstrument>> _geInstrumentService;
    private readonly Mock<IPriceInfoMapperService> _priceInfoMapperService;
    private readonly Mock<IRedisV2Publisher> _redisPublisher;

    //IPriceInfoMapperService priceInfoMapperService

    public KafkaMessageHandlerTest()
    {
        _logger = new Mock<ILogger<KafkaMessageHandler>>();
        _redisPublisher = new Mock<IRedisV2Publisher>();
        _configuration = new Mock<IConfiguration>();
        _cacheService = new Mock<ICacheService>();
        _mongoService = new Mock<IMongoService<MarketSchedule>>();
        _geInstrumentService = new Mock<IMongoService<GeInstrument>>();
        _priceInfoMapperService = new Mock<IPriceInfoMapperService>();
        _configuration.Setup(x => x["Redis:Channel"]).Returns("mockedRedisChannel");
        _mongoService
            .Setup(x => x.GetByFilterAsync(It.IsAny<Expression<Func<MarketSchedule, bool>>>()))
            .ReturnsAsync(new MarketSchedule());

        _kafkaMessageHandler = new KafkaMessageHandler(
            _logger.Object,
            _redisPublisher.Object,
            _configuration.Object, 
            null,
            null,
            null,
            null,
            null
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData(null)]
    public async Task HandleAsync_ShouldNever_PublishMessage_When_GotEmptyParameter(Confluent.Kafka.Message<string, string> message)
    {
        // Arrange
        _configuration.Setup(x => x["Redis:Channel"]).Returns(string.Empty);

        // Act
        await _kafkaMessageHandler.HandleAsync(message);

        // Assert
        _redisPublisher.Verify(
            r => r.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never
        );
        _cacheService.Verify(
            r => r.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData(GlobalMarketMessageType.SnapshotFullRefresh)]
    public void HandleAsync_ShouldThrowArgumentException_When_GotItchMessageWithEmptyMessage(
        string messageType
    )
    {
        // Arrange
        _configuration.Setup(x => x["Redis:Channel"]).Returns(string.Empty);
        var fixMessage = new FixMessage
        {
            MessageType = messageType,
            Message = string.Empty
        };
        
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = fixMessage.ToString(),
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(
            () => _kafkaMessageHandler.HandleAsync(kafkaMessage)
        );

        // Assert
        _redisPublisher.Verify(
            r => r.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never
        );
        _cacheService.Verify(
            r => r.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()),
            Times.Never
        );
    }

    [Fact]
    public async Task HandleAsync_NeverSetCacheAndPublishToRedis_WhenNoMatchMessageType()
    {
        // Arrange
        var fixture = new Fixture();
        _configuration.Setup(x => x["Redis:Channel"]).Returns(string.Empty);
        var fixMessage = new FixMessage
        {
            MessageType = fixture.Create<string>(),
            Message = fixture.Create<string>()
        };
        
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = fixMessage.ToString(),
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        await _kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _redisPublisher.Verify(
            r => r.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never
        );
        _cacheService.Verify(
            r => r.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>()),
            Times.Never
        );
    }

    [Fact]
    public async Task HandleAsync_WhenMessageIsValid()
    {
        // Arrange
        var fixture = new Fixture();
        var msg =
            "{\"Symbol\": \"AAPL.NASDAQ\",\"MDReqID\": \"reqID88\",\"Entries\": [{\"MDEntryType\": \"2\",\"MDEntryPx\": 227.39,\"MDEntrySize\": 100.0,\"MDEntryDate\": \"2024-08-26T00:00:00\",\"MDEntryTime\": \"2024-08-26T09:17:46.512\"}]}";
        var fixMsg = fixture
            .Build<FixMessage>()
            .With(x => x.Message, msg)
            .With(x => x.MessageType, GlobalMarketMessageType.SnapshotFullRefresh)
            .Create();
        
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = fixMsg.ToString(),
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await _kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _mongoService.Verify(
            r => r.GetByFilterAsync(It.IsAny<Expression<Func<MarketSchedule, bool>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CallHandleAsync_WhenMessageIsIncorrectFormat_NeverPublisRedis_WhenOrderBookIsNull()
    {
        // Arrange
        var fixture = new Fixture();
        var symbol = fixture.Create<string>();
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = symbol,
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await _kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _cacheService.Verify(
            x => x.SetAsync(It.IsAny<string>(), It.IsAny<StreamingBody>(), It.IsAny<TimeSpan>()),
            Times.Never
        );
        _redisPublisher.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<MarketStreamingResponse>(), It.IsAny<bool>()),
            Times.Never
        );
    }

    [Fact]
    public async Task CallHandleAsync_PublishToRedis_WhenOrderBookIsValid()
    {
        // Arrange
        var fixture = new Fixture();
        var msg =
            "{\"Symbol\": \"AAPL.NASDAQ\",\"MDReqID\": \"reqID88\",\"Entries\": [{\"MDEntryType\": \"1\",\"MDEntryPx\": 227.39,\"MDEntrySize\": 100.0,\"MDEntryDate\": \"2024-08-26T00:00:00\",\"MDEntryTime\": \"2024-08-26T09:17:46.512\"}]}";
        var fixMsg = fixture
            .Build<FixMessage>()
            .With(x => x.Message, msg)
            .With(x => x.MessageType, GlobalMarketMessageType.SnapshotFullRefresh)
            .Create();
        
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = fixMsg.ToString(),
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        _cacheService
            .Setup(x => x.GetAsync<StreamingBody>(It.IsAny<string>()))
            .ReturnsAsync(new StreamingBody());

        // Act
        await _kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _cacheService.Verify(
            x => x.SetAsync(It.IsAny<string>(), It.IsAny<StreamingBody>(), null),
            Times.AtLeastOnce
        );
        _redisPublisher.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<MarketStreamingResponse>(), It.IsAny<bool>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task CallbackPublicTradeMessageAsync_PublishToRedis_WhenOrderBookIsValid()
    {
        // Arrange
        var fixture = new Fixture();
        var msg =
            "{\"Symbol\": \"AAPL.NASDAQ\",\"MDReqID\": \"reqID88\",\"Entries\": [{\"MDEntryType\": \"2\",\"MDEntryPx\": 227.39,\"MDEntrySize\": 100.0,\"MDEntryDate\": \"2024-08-26T00:00:00\",\"MDEntryTime\": \"2024-08-26T09:17:46.512\"}]}";
        var fixMsg = fixture
            .Build<FixMessage>()
            .With(x => x.Message, msg)
            .With(x => x.MessageType, GlobalMarketMessageType.SnapshotFullRefresh)
            .Create();
        
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = fixMsg.ToString(),
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        _cacheService
            .Setup(x => x.GetAsync<StreamingBody>(It.IsAny<string>()))
            .ReturnsAsync(new StreamingBody());

        // Act
        await _kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _cacheService.Verify(
            x => x.SetAsync(It.IsAny<string>(), It.IsAny<StreamingBody>(), null),
            Times.AtLeastOnce
        );
        _redisPublisher.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<MarketStreamingResponse>(), It.IsAny<bool>()),
            Times.AtLeastOnce
        );
    }

    [Fact]
    public async Task CallbackPriceInfoMessageAsync_PublishToRedis_WhenOrderBookIsValid()
    {
        // Arrange
        var fixture = new Fixture();
        var msg =
            "{\"Symbol\": \"AAPL.NASDAQ\",\"MDReqID\": \"reqID88\",\"Entries\": [{\"MDEntryType\": \"5\",\"MDEntryPx\": 227.39,\"MDEntrySize\": 100.0,\"MDEntryDate\": \"2024-08-26T00:00:00\",\"MDEntryTime\": \"2024-08-26T09:17:46.512\"}]}";
        var fixMsg = fixture
            .Build<FixMessage>()
            .With(x => x.Message, msg)
            .With(x => x.MessageType, GlobalMarketMessageType.SnapshotFullRefresh)
            .Create();
        _cacheService
            .Setup(x => x.GetAsync<StreamingBody>(It.IsAny<string>()))
            .ReturnsAsync(new StreamingBody());
        
        var kafkaMessage = new Message<string, string>
        {
            Key = "testKey", // Optional key
            Value = fixMsg.ToString(),
            Timestamp = new Timestamp(DateTime.UtcNow) // Current timestamp
        };

        // Act
        await _kafkaMessageHandler.HandleAsync(kafkaMessage);

        // Assert
        _cacheService.Verify(
            x => x.SetAsync(It.IsAny<string>(), It.IsAny<StreamingBody>(), null),
            Times.AtLeastOnce
        );
        _redisPublisher.Verify(
            x => x.PublishAsync(It.IsAny<string>(), It.IsAny<MarketStreamingResponse>(), It.IsAny<bool>()),
            Times.AtLeastOnce
        );
    }
}