using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public partial class TradeStatisticsMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> itchMockMessage = ItchMockMessage.GetMessage(
        ItchMessageType.I
    );

    [Theory]
    [MemberData(nameof(itchMockMessage))]
    public void Parse_TradeStatisticsMessage_VerifyBehavior2(byte[] input, object[] result)
    {
        // Arrange
        // Act
        TradeStatisticsMessage? output = itchParserService.Parse(input) as TradeStatisticsMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderBookId.Value);
        Assert.Equal(result[3], output.OpenPrice.Value);
        Assert.Equal(result[4], output.HighPrice.Value);
        Assert.Equal(result[5], output.LowPrice.Value);
        Assert.Equal(result[6], output.LastPrice.Value);
        Assert.Equal(result[7], output.LastAuctionPrice.Value);
        Assert.Equal(result[8], (int)output.TurnoverQuantity.Value);
        Assert.Equal(result[9], (int)output.ReportedTurnoverQuantity.Value);
        Assert.Equal(Convert.ToInt64(result[10]), output.TurnoverValue.Value);
        Assert.Equal(Convert.ToInt64(result[11]), output.ReportedTurnoverValue.Value);
        Assert.Equal(result[12], output.AveragePrice.Value);
        Assert.Equal(result[13], (int)output.TotalNumberOfTrades.Value);
    }

    [Fact]
    public void Parse_TradeStatisticsMessage_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.I, 0, 0, 0, 1];

        // Act
        // Assert

        Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
    }
}
