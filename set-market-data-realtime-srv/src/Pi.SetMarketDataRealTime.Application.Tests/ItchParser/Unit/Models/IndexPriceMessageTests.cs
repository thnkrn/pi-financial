using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class IndexPriceMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.J
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(message))]
    public async Task IndexPriceMessage_Constructor_SetsInputCorrectly(
        byte[] input,
        object[] result
    )
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as IndexPriceMessage;
        var (dateTime, extraPrecision) = Timestamp.Parse((long)result[13]);
        // format to match the expected output
        var expectedTimeStamp = Timestamp.Formatter(dateTime, extraPrecision);

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderbookId.Value);
        Assert.Equal(result[3], (int)output.Value.Value);
        Assert.Equal(result[4], (int)output.HighValue.Value);
        Assert.Equal(result[5], (int)output.LowValue.Value);
        Assert.Equal(result[6], (int)output.OpenValue.Value);
        Assert.Equal(result[7], (int)output.TradedVolume.Value);
        Assert.Equal(result[9], (int)output.Change.Value);
        Assert.Equal(result[10], output.ChangePercent.Value);
        Assert.Equal(result[11], (int)output.PreviousClose.Value);
        Assert.Equal(result[12], (int)output.Close.Value);
        Assert.Equal(expectedTimeStamp, output.Timestamp.ToString());
    }

    [Fact]
    public async Task IndexPriceMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.J, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }
}