using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class OrderBookStateMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.O
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(message))]
    public async Task OrderBookStateMessage_Constructor_SetsInputCorrectly(
        byte[] input,
        object[] result
    )
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as OrderBookStateMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderBookId.Value);
        Assert.Equal(result[3], output.StateName.Value);
    }

    [Fact]
    public async Task OrderBookStateMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.O, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }
}