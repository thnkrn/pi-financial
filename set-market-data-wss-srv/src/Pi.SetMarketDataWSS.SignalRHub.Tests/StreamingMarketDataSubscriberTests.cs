using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataWSS.Domain.Models.Request;
using Pi.SetMarketDataWSS.Domain.Models.Response;
using Pi.SetMarketDataWSS.SignalRHub.Hubs;
using Pi.SetMarketDataWSS.SignalRHub.Services;
using StackExchange.Redis;
using Polly;
using System.Diagnostics;

namespace Pi.SetMarketDataWSS.SignalRHub.Tests;

public class StreamingMarketDataSubscriberTests
{
    private readonly Mock<IConnectionMultiplexer> _mockRedisConnection;
    private readonly Mock<IHubContext<StreamingHub>> _mockHubContext;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<StreamingMarketDataSubscriber>> _mockLogger;
    private readonly Mock<ISubscriber> _mockSubscriber;
    private readonly Mock<ISingleClientProxy> _mockClientProxy;
    private readonly Mock<IHubClients> _mockHubClients;
    private readonly IAsyncPolicy _circuitBreakerPolicy;

    public StreamingMarketDataSubscriberTests()
    {
        _mockRedisConnection = new Mock<IConnectionMultiplexer>();
        _mockHubContext = new Mock<IHubContext<StreamingHub>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<StreamingMarketDataSubscriber>>();
        _mockSubscriber = new Mock<ISubscriber>();
        _mockClientProxy = new Mock<ISingleClientProxy>();
        _mockHubClients = new Mock<IHubClients>();

        // Setup configuration
        var configurationSection = new Mock<IConfigurationSection>();
        configurationSection.Setup(s => s.Value).Returns("TestValue");
        _mockConfiguration.Setup(c => c.GetSection(It.IsAny<string>())).Returns(configurationSection.Object);

        _mockConfiguration.Setup(x => x["SIGNALR_HUB:GROUP_NAME"]).Returns("ReceiveMarketDataGroup");
        _mockConfiguration.Setup(x => x["SIGNALR_HUB:METHOD_NAME"]).Returns("ReceiveMarketData");
        _mockConfiguration.Setup(x => x["REDIS:CHANNEL"]).Returns("stock_market_data");

        _mockRedisConnection.Setup(x => x.GetSubscriber(null)).Returns(_mockSubscriber.Object);

        // Setup HubContext
        _mockHubContext.Setup(x => x.Clients).Returns(_mockHubClients.Object);
        _mockHubClients.Setup(x => x.Client(It.IsAny<string>())).Returns(_mockClientProxy.Object);

        // Setup circuit breaker policy
        _circuitBreakerPolicy = Policy
            .Handle<RedisConnectionException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }

    [Fact]
    public async Task StartAsync_ShouldSubscribeToRedisChannel()
    {
        // Arrange
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        // Act
        await subscriber.StartAsync(CancellationToken.None);

        // Assert
        _mockSubscriber.Verify(
            x => x.SubscribeAsync(
                It.Is<RedisChannel>(c => c.ToString() == "stock_market_data"),
                It.IsAny<Action<RedisChannel, RedisValue>>(),
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task UpdateSubscriptionAsync_ShouldAddSubscription()
    {
        // Arrange
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        var request = new MarketStreamingRequest
        {
            Data = new Data
            {
                Param = new List<MarketStreamingParameter>
                {
                    new MarketStreamingParameter { Symbol = "AAPL", Market = "NASDAQ" },
                    new MarketStreamingParameter { Symbol = "GOOGL", Market = "NASDAQ" }
                },
                SubscribeType = "MARKET_DATA"
            },
            Op = "subscribe",
            SessionId = "test-session-id"
        };

        // Act
        await subscriber.UpdateSubscriptionAsync("testConnection", request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v.ToString().Contains("Updated subscription for client testConnection. Symbols: AAPL, GOOGL")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveSubscriptionAsync_ShouldRemoveSubscription()
    {
        // Arrange
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        var request = new MarketStreamingRequest
        {
            Data = new Data
            {
                Param = new List<MarketStreamingParameter>
                {
                    new MarketStreamingParameter { Symbol = "AAPL", Market = "NASDAQ" }
                },
                SubscribeType = "MARKET_DATA"
            },
            Op = "subscribe",
            SessionId = "test-session-id"
        };

        await subscriber.UpdateSubscriptionAsync("testConnection", request);

        // Act
        await subscriber.RemoveSubscriptionAsync("testConnection");

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Removed subscription for client testConnection")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSendUpdatesToMatchingClients()
    {
        // Arrange
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        var request = new MarketStreamingRequest
        {
            Data = new Data
            {
                Param = new List<MarketStreamingParameter>
                {
                    new MarketStreamingParameter { Symbol = "AAPL", Market = "NASDAQ" }
                },
                SubscribeType = "MARKET_DATA"
            },
            Op = "subscribe",
            SessionId = "test-session-id"
        };

        await subscriber.UpdateSubscriptionAsync("testConnection", request);

        var response = new MarketStreamingResponse
        {
            Code = "0",
            Op = "push",
            Message = "Success",
            Response = new StreamingResponse
            {
                Data = new List<StreamingBody>
                {
                    new StreamingBody
                    {
                        Symbol = "AAPL",
                        Price = "150.00",
                        Volume = "1000000"
                    }
                }
            }
        };

        // Simulate Redis message
        Action<RedisChannel, RedisValue> callback = null;
        _mockSubscriber.Setup(x => x.SubscribeAsync(It.IsAny<RedisChannel>(),
                It.IsAny<Action<RedisChannel, RedisValue>>(), It.IsAny<CommandFlags>()))
            .Callback<RedisChannel, Action<RedisChannel, RedisValue>, CommandFlags>((channel, handler, flags) =>
                callback = handler);

        // Act
        await subscriber.StartAsync(CancellationToken.None);
        callback(new RedisChannel("stock_market_data", RedisChannel.PatternMode.Auto),
            System.Text.Json.JsonSerializer.Serialize(response));

        // Assert
        _mockClientProxy.Verify(x => x.SendCoreAsync(
                "ReceiveMarketData",
                It.Is<object[]>(args => args[0] is MarketStreamingResponse),
                default(CancellationToken)),
            Times.Once);
    }

    [Fact]
    public async Task StopAsync_ShouldCancelExecutingTask()
    {
        // Arrange
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        await subscriber.StartAsync(CancellationToken.None);

        // Act
        await subscriber.StopAsync(CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("StreamingMarketDataSubscriber is stopping")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce());
    }

    [Fact]
    public void IsHealthy_ShouldReturnTrueWhenConnectedAndExecuting()
    {
        // Arrange
        _mockRedisConnection.Setup(x => x.IsConnected).Returns(true);
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        // Act
        subscriber.StartAsync(CancellationToken.None).Wait();
        var result = subscriber.IsHealthy();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsHealthy_ShouldReturnFalseWhenNotConnected()
    {
        // Arrange
        _mockRedisConnection.Setup(x => x.IsConnected).Returns(false);
        var subscriber = new StreamingMarketDataSubscriber(
            _mockRedisConnection.Object,
            _mockHubContext.Object,
            _mockConfiguration.Object,
            _mockLogger.Object,
            _circuitBreakerPolicy);

        // Act
        var result = subscriber.IsHealthy();

        // Assert
        Assert.False(result);
    }
}