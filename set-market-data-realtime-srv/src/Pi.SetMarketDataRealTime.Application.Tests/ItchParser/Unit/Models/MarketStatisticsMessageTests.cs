using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public partial class MarketStatisticMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.g
    );

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    [Theory]
    [MemberData(nameof(message))]
    public async Task MarketStatisticMessage_Constructor_SetsInputCorrectly(
        byte[] input,
        object[] result
    )
    {
        // Arrange
        // Act
        var output = await itchParserService.Parse(input) as MarketStatisticMessage;
        var (dateTime, extraPrecision) = Timestamp.Parse((long)result[4]);
        // format to match the expected output
        var expectedTimeStamp = Timestamp.Formatter(dateTime, extraPrecision);


        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], output.MarketStatisticsID.ToString());
        Assert.Equal(result[3], output.Currency.ToString());
        Assert.Equal(expectedTimeStamp, output.MarketStatisticsTime.ToString());
        Assert.Equal(result[5], (int)output.TotalTrades.Value);
        Assert.Equal(result[6], (int)output.TotalQuantity.Value);
        Assert.Equal(result[7], (int)output.TotalValue.Value);
        Assert.Equal(result[8], (int)output.UpQuantity.Value);
        Assert.Equal(result[9], (int)output.DownQuantity.Value);
        Assert.Equal(result[10], (int)output.NoChangeShares.Value);
        Assert.Equal(result[11], (int)output.UpShares.Value);
        Assert.Equal(result[12], (int)output.DownShares.Value);
        Assert.Equal(result[13], (int)output.NoChangeShares.Value);
    }

    [Fact]
    public async Task MarketStatisticMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.g, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }
}