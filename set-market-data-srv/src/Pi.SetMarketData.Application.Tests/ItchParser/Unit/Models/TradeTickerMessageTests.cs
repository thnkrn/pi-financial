using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;

public class TradeTickerMessageParserTests
{

    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.i
    );

    [Theory]
    [MemberData(nameof(message))]
    public void TradeTickerMessage_Constructor_SetsInputCorrectly(
             byte[] input,
             object[] result
         )
    {
        TradeTickerMessage? output = itchParserService.Parse(input) as TradeTickerMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderbookId.Value);
        Assert.Equal(result[4], (int)output.DealSource.Value);
        Assert.Equal(result[5], output.Price.Value);
        Assert.Equal(result[6], (int)output.Quantity.Value);
        Assert.Equal(result[8], (int)output.Action.Value);
        Assert.Equal(result[9], output.Aggressor.ToString());
        Assert.Equal(result[10], (int)output.TradeReportCode.Value);
    }
    [Fact]
    public void TradeTickerMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.i, 0, 0, 0, 1];

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
    }

}
