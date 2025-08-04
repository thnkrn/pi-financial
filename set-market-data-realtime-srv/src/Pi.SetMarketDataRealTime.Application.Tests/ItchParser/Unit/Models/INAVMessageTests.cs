using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public partial class INAVMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.f
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public async Task Parse_INAVMessage_VerifyBehavior2(byte[] input, object[] result)
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as INAVMessage;
        var (dateTime, extraPrecision) = Timestamp.Parse((long)result[6]);

        // format to match the expected output
        var expectedTimeStamp = Timestamp.Formatter(dateTime, extraPrecision);

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderBookId.Value);
        Assert.Equal(result[3], output.INAV.Value);
        Assert.Equal(result[4], output.Change.Value);
        Assert.Equal(result[5], output.PercentageChange.Value);
        Assert.Equal(expectedTimeStamp, output.Timestamp.ToString());
    }

    [Fact]
    public async Task Parse_INAVMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.f, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }
}