using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Interfaces.WriteBinlogData;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.WriteBinLogData;
using Pi.SetMarketDataRealTime.DataHandler.Interfaces.SoupBinTcp;
using Pi.SetMarketDataRealTime.DataHandler.Services.SoupBinTcp;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.Kafka;
using Pi.SetMarketDataRealTime.Infrastructure.Services.Kafka.HighPerformance;

namespace Pi.SetMarketDataRealTime.DataHandler.Tests;

public class ClientListenerTests
{
    private readonly IConfiguration _configuration;
    private readonly ClientListenerDependencies _dependencies;
    private readonly Mock<IDisconnectionHandlerFactory> _mockDisconnectionHandlerFactory;
    private readonly Mock<IItchParserService> _mockItchParserService;
    private readonly Mock<IStockDataOptimizedV2Publisher> _mockKafkaPublisher;
    private readonly Mock<ILogger<ClientListener>> _mockLogger;
    private readonly Mock<IMemoryCacheHelper> _mockMemoryCacheHelper;
    private readonly Mock<RealTimeStockMessageLogger> _mockRealTimeStockMessageLogger;
    private readonly Mock<IWriteBinLogsData> _mockWriteBinLogsData;

    public ClientListenerTests()
    {
        _configuration = CreateConfiguration();
        _mockItchParserService = new Mock<IItchParserService>();
        _mockWriteBinLogsData = new Mock<IWriteBinLogsData>();
        _mockKafkaPublisher = new Mock<IStockDataOptimizedV2Publisher>();
        _mockLogger = new Mock<ILogger<ClientListener>>();
        _mockMemoryCacheHelper = new Mock<IMemoryCacheHelper>();
        _mockRealTimeStockMessageLogger = new Mock<RealTimeStockMessageLogger>();
        _mockDisconnectionHandlerFactory = new Mock<IDisconnectionHandlerFactory>();

        _dependencies = new ClientListenerDependencies(
            _mockItchParserService.Object,
            _mockWriteBinLogsData.Object,
            _mockKafkaPublisher.Object,
            _mockMemoryCacheHelper.Object,
            _mockDisconnectionHandlerFactory.Object
        );
    }

    private IConfiguration CreateConfiguration()
    {
        var configValues = new Dictionary<string, string>
        {
            { ConfigurationKeys.KafkaTopic, "test-topic" },
            { ConfigurationKeys.WriteStockMessageToFile, "false" },
            { ConfigurationKeys.ServerConfigStreamDataPath, "test-path" },
            { ConfigurationKeys.SettradeRunLocalMode, "false" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    [Fact]
    public async Task OnMessage_ShouldProcessMessageCorrectly()
    {
        // Arrange
        var clientListener = new ClientListener(
            _dependencies,
            _mockLogger.Object
        );

        var testMessage = new byte[] { 1, 2, 3 };
        var testItchMessage = CreateTestItchMessage('T');

        _mockItchParserService.Setup(x => x.Parse(It.IsAny<byte[]>())).ReturnsAsync(testItchMessage);

        // Act
        await clientListener.OnMessage(testMessage);

        // Assert
        _mockWriteBinLogsData.Verify(x => x.WriteBinLogsDataAsync(testMessage, It.IsAny<string>()), Times.Once);
        _mockItchParserService.Verify(x => x.Parse(testMessage), Times.Once);
        _mockKafkaPublisher.Verify(x => x.EnqueueMessageAsync(It.IsAny<ItchMessage>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task ReceiveMessage_ShouldReturnMessageWhenAvailable()
    {
        // Arrange
        var clientListener = new ClientListener(
            _dependencies,
            _mockLogger.Object
        );

        var testMessage = new byte[] { 1, 2, 3 };
        var testItchMessage = CreateTestItchMessage('S');

        _mockItchParserService.Setup(x => x.Parse(It.IsAny<byte[]>())).ReturnsAsync(testItchMessage);

        // Act
        await clientListener.OnMessage(testMessage);
        var receivedMessage = await clientListener.ReceiveMessage(CancellationToken.None);

        // Assert
        Assert.NotNull(receivedMessage);
        Assert.Equal('S', receivedMessage.MsgType);
    }

    [Fact]
    public async Task OnLoginAccept_ShouldUpdateMemoryCache()
    {
        // Arrange
        var clientListener = new ClientListener(
            _dependencies,
            _mockLogger.Object
        );

        var testSession = "test-session";
        var testSequenceNumber = 12345UL;

        // Act
        await clientListener.OnLoginAccept(testSession, testSequenceNumber);

        // Assert
        _mockMemoryCacheHelper.Verify(x => x.SetCurrentSessionAsync(testSession.Trim()), Times.Once);
        _mockMemoryCacheHelper.Verify(x => x.SetCurrentItchSequenceNoAsync(testSequenceNumber), Times.Once);
    }

    [Fact]
    public async Task OnDisconnect_ShouldHandleUnexpectedDisconnection()
    {
        // Arrange
        var mockDisconnectionHandler = new Mock<IDisconnectionHandler>();
        _mockDisconnectionHandlerFactory.Setup(x => x.CreateHandler()).Returns(mockDisconnectionHandler.Object);

        var clientListener = new ClientListener(
            _dependencies,
            _mockLogger.Object
        );

        // Act
        await clientListener.OnDisconnect(true);

        // Assert
        _mockDisconnectionHandlerFactory.Verify(x => x.CreateHandler(), Times.Once);
        mockDisconnectionHandler.Verify(x => x.HandleUnexpectedDisconnectionAsync(CancellationToken.None), Times.Once);
    }

    private ItchMessage CreateTestItchMessage(char msgType)
    {
        return new TestItchMessage(msgType);
    }

    private class TestItchMessage : ItchMessage
    {
        public TestItchMessage(char msgType)
        {
            MsgType = msgType;
        }

        public override Numeric32 Nanos => new(new byte[] { 0, 0, 0, 0 });
    }
}