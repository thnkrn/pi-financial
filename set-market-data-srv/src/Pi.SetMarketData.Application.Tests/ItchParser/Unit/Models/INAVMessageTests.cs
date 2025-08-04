using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;

public partial class INAVMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.f
    );

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public void Parse_INAVMessage_VerifyBehavior2(byte[] input, object[] result)
    {
        // Arrange
        // Act
        INAVMessage? output = itchParserService.Parse(input) as INAVMessage;
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
    public void Parse_INAVMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.f, 0, 0, 0, 1];

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
    }
}
