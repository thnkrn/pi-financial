using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Tests.Services;

public class MessageSamplerServiceTests
{
    private readonly Mock<IConfigurationSection> _mockConfigSection;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<MessageSamplerService>> _mockLogger;

    public MessageSamplerServiceTests()
    {
        _mockLogger = new Mock<ILogger<MessageSamplerService>>();
        _mockConfiguration = new Mock<IConfiguration>();
        
        var enabledSection = new Mock<IConfigurationSection>();
        enabledSection.Setup(s => s.Value).Returns("true");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:ENABLED")).Returns(enabledSection.Object);
        
        var countSection = new Mock<IConfigurationSection>();
        countSection.Setup(s => s.Value).Returns("8");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:SAMPLING_COUNT")).Returns(countSection.Object);
        
        var timeWindowSection = new Mock<IConfigurationSection>();
        timeWindowSection.Setup(s => s.Value).Returns("20");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:TIME_WINDOW_MS")).Returns(timeWindowSection.Object);
        
        var cleanupSection = new Mock<IConfigurationSection>();
        cleanupSection.Setup(s => s.Value).Returns("30");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:CLEANUP_INTERVAL_MINUTES"))
            .Returns(cleanupSection.Object);
        
        var inactiveSection = new Mock<IConfigurationSection>();
        inactiveSection.Setup(s => s.Value).Returns("10");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:INACTIVE_THRESHOLD_MINUTES"))
            .Returns(inactiveSection.Object);
    }

    [Fact]
    public void ShouldPublishMessage_WhenDisabled_AlwaysReturnsTrue()
    {
        // Arrange
        var mockDisabledSection = new Mock<IConfigurationSection>();
        mockDisabledSection.Setup(s => s.Value).Returns("false");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:ENABLED")).Returns(mockDisabledSection.Object);

        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var response = CreateSampleResponse("AAPL");

        // Act
        var result = service.ShouldPublishMessage("AAPL", response);

        // Assert
        Assert.True(result, "When sampling is disabled, all messages should be published");
    }

    [Fact]
    public void ShouldPublishMessage_WithEmptySymbol_AlwaysReturnsTrue()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var response = CreateSampleResponse("");

        // Act
        var result = service.ShouldPublishMessage("", response);

        // Assert
        Assert.True(result, "Messages with empty symbol should always be published");
    }

    [Fact]
    public void ShouldPublishMessage_CountBelowThreshold_ReturnsFalse()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "AAPL";

        // Act & Assert - First 7 messages should be sampled out (not published)
        for (var i = 0; i < 7; i++)
        {
            var response = CreateSampleResponse(symbol);
            var result = service.ShouldPublishMessage(symbol, response);
            Assert.False(result, $"Message {i + 1} should be sampled out (not published)");
        }
    }
    
    [Fact]
    public void ShouldPublishMessage_CountReachesThreshold_ReturnsTrue()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "AAPL";
    
        // Act - Send 7 messages first (these should be sampled out)
        for (int i = 0; i < 7; i++)
        {
            var response = CreateSampleResponse(symbol);
            service.ShouldPublishMessage(symbol, response);
        }
    
        // Act & Assert - 8th message should be published
        var result = service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));
        Assert.True(result, "8th message should be published as it reaches the threshold");
    }

    [Fact]
    public void ShouldPublishMessage_TimeWindowExceeded_ReturnsTrue()
    {
        // Arrange - Setup very short time window (5ms)
        var mockTimeSection = new Mock<IConfigurationSection>();
        mockTimeSection.Setup(s => s.Value).Returns("5");
        _mockConfiguration.Setup(c => c.GetSection("MESSAGE_SAMPLING:TIME_WINDOW_MS")).Returns(mockTimeSection.Object);

        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "AAPL";

        // Act - Send first message
        service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));

        // Wait longer than the time window - เพิ่มเวลา wait ให้มากขึ้น
        Thread.Sleep(50);

        // Act & Assert - Next message should be published due to time window
        var result = service.ShouldPublishMessage(symbol, CreateSampleResponse(symbol));
        Assert.True(result, "Message should be published when time window is exceeded");
    }

    [Fact]
    public void ShouldPublishMessage_MultipleDifferentSymbols_TracksIndependently()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);

        // Act & Assert - Send 7 messages for each symbol, none should be published
        for (var i = 0; i < 7; i++)
        {
            Assert.False(service.ShouldPublishMessage("AAPL", CreateSampleResponse("AAPL")));
            Assert.False(service.ShouldPublishMessage("MSFT", CreateSampleResponse("MSFT")));
            Assert.False(service.ShouldPublishMessage("GOOG", CreateSampleResponse("GOOG")));
        }

        // 8th message for each symbol should be published
        Assert.True(service.ShouldPublishMessage("AAPL", CreateSampleResponse("AAPL")));
        Assert.True(service.ShouldPublishMessage("MSFT", CreateSampleResponse("MSFT")));
        Assert.True(service.ShouldPublishMessage("GOOG", CreateSampleResponse("GOOG")));

        // Count should reset, so 9th message should not be published
        Assert.False(service.ShouldPublishMessage("AAPL", CreateSampleResponse("AAPL")));
    }

    [Fact]
    public void GetLatestResponse_NoMessageStored_ReturnsNull()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);

        // Act
        var result = service.GetLatestResponse("AAPL");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetLatestResponse_WithMessageStored_ReturnsLatestMessage()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "AAPL";
        var response = CreateSampleResponse(symbol);

        // Store a message
        service.ShouldPublishMessage(symbol, response);

        // Act
        var result = service.GetLatestResponse(symbol);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(response, result);
    }

    [Fact]
    public void StoreLatestResponse_StoresMessageForRetrieval()
    {
        // Arrange
        var service = new MessageSamplerService(_mockLogger.Object, _mockConfiguration.Object);
        var symbol = "AAPL";
        var response = CreateSampleResponse(symbol);

        // Act
        service.StoreLatestResponse(symbol, response);

        // Assert
        var result = service.GetLatestResponse(symbol);
        Assert.NotNull(result);
        Assert.Equal(response, result);
    }

    private MarketStreamingResponse CreateSampleResponse(string symbol)
    {
        var streamingBody = new StreamingBody { Symbol = symbol };
        var streamingResponse = new StreamingResponse { Data = new List<StreamingBody> { streamingBody } };

        return new MarketStreamingResponse
        {
            Code = "200",
            Op = "Streaming",
            SendingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
            SequenceNumber = 12345,
            ProcessingTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
            SendingId = Guid.NewGuid().ToString("N"),
            CreationTime = DateTime.UtcNow.ToString("yyyyMMdd-HH:mm:ss.fff"),
            MdEntryType = "0",
            Response = streamingResponse
        };
    }
}