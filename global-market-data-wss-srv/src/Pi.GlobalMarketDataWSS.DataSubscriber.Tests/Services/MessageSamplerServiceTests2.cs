using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.GlobalMarketDataWSS.DataSubscriber.Services;
using Pi.GlobalMarketDataWSS.Domain.Models.Response;

namespace Pi.GlobalMarketDataWSS.DataSubscriber.Tests.Services;

public class MessageSamplerServiceTests2
{
    private readonly Mock<ILogger<MessageSamplerService>> _loggerMock = new();
    private readonly IConfiguration _configEnabled;
    private readonly IConfiguration _configDisabled;

    public MessageSamplerServiceTests2()
    {
        _configEnabled = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["MESSAGE_SAMPLING:ENABLED"] = "true",
                ["MESSAGE_SAMPLING:SAMPLING_COUNT"] = "8",
                ["MESSAGE_SAMPLING:TIME_WINDOW_MS"] = "20"
            })
            .Build();

        _configDisabled = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["MESSAGE_SAMPLING:ENABLED"] = "false"
            })
            .Build();
    }

    [Fact]
    public void ShouldPublish_WhenSamplingDisabled_ShouldAlwaysReturnTrue()
    {
        var service = new MessageSamplerService(_loggerMock.Object, _configDisabled);

        for (int i = 0; i < 100; i++)
        {
            var result = service.ShouldPublishMessage("AAPL", new MarketStreamingResponse());
            Assert.True(result);
        }
    }

    [Fact]
    public void ShouldPublish_Every8Messages_ShouldReturnTrueOn10th()
    {
        var service = new MessageSamplerService(_loggerMock.Object, _configEnabled);
        var symbol = "AAPL";

        bool shouldPublish = false;

        for (int i = 1; i <= 8; i++)
        {
            shouldPublish = service.ShouldPublishMessage(symbol, new MarketStreamingResponse());
        }

        Assert.True(shouldPublish); // 8th should trigger publish

        // next 5 should be false again
        for (int i = 0; i < 5; i++)
        {
            shouldPublish = service.ShouldPublishMessage(symbol, new MarketStreamingResponse());
            Assert.False(shouldPublish);
        }
    }

    [Fact]
    public void ShouldPublish_WhenTimeWindowExceeded_ShouldReturnTrue()
    {
        var service = new MessageSamplerService(_loggerMock.Object, _configEnabled);
        var symbol = "AAPL";

        service.ShouldPublishMessage(symbol, new MarketStreamingResponse()); // init

        // Wait over 25ms (time threshold)
        Thread.Sleep(30);

        var result = service.ShouldPublishMessage(symbol, new MarketStreamingResponse());

        Assert.True(result);
    }

    [Fact]
    public void ShouldKeepLatestResponse_ForEachSymbol()
    {
        var service = new MessageSamplerService(_loggerMock.Object, _configEnabled);
        var symbol = "AAPL";

        var expected = new MarketStreamingResponse { SendingId = "XYZ123" };

        service.ShouldPublishMessage(symbol, expected);

        var latest = service.GetLatestResponse(symbol);

        Assert.NotNull(latest);
        Assert.Equal("XYZ123", latest.SendingId);
    }

    [Fact]
    public void Sampling_ShouldBeIsolated_PerSymbol()
    {
        var service = new MessageSamplerService(_loggerMock.Object, _configEnabled);

        // Add 10 messages to AAPL
        for (int i = 0; i < 7; i++)
        {
            service.ShouldPublishMessage("AAPL", new MarketStreamingResponse());
        }

        // AAPL should return true now
        Assert.True(service.ShouldPublishMessage("AAPL", new MarketStreamingResponse()));

        // MSFT still fresh - should not publish
        Assert.False(service.ShouldPublishMessage("MSFT", new MarketStreamingResponse()));
    }
}