using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.ItchParser;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Tests.Services;

public class ItchParserServiceTest
{
    private readonly Mock<IMemoryCacheHelper> _memoryCache;
    private readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger;
    private readonly ItchMessageMetadataHandler _messageMetadataHandler;
    private readonly IItchParserService itchParserService;

    public ItchParserServiceTest()
    {
        _memoryCache = new Mock<IMemoryCacheHelper>();
        _memoryLogger = new Mock<ILogger<ItchMessageMetadataHandler>>();
        _messageMetadataHandler = new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object);
        itchParserService = new ItchParserService(_messageMetadataHandler);
    }

    [Fact]
    public async Task ItchParserService_Parse_null_Should_Throw_ArgumentException()
    {
        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => itchParserService.Parse(null));
    }

    [Fact]
    public void ItchParserService_Parse_Zero_length_Should_Throw_ArgumentException()
    {
        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => itchParserService.Parse(new byte[0]));
    }

    [Fact]
    public async void ItchParserService_Parse_MsgType_T_Return_SecondMessage()
    {
        // Arrange
        byte[] msg = { 84, 1, 1, 0, 1 }; // T 1 2 7 6

        // Act
        var result = await itchParserService.Parse(msg);

        // Assert
        Assert.IsType<SecondsMessage>(result);
    }
}