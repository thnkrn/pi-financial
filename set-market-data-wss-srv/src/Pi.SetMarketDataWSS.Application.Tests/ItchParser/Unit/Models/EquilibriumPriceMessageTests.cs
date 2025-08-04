using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public partial class EquilibriumPriceMessageTests
{
    private static readonly ItchParserService itchParserService = new();
    public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
        ItchMessageType.Z
    );

    [Theory]
    [MemberData(nameof(message))]
    public void EquilibriumPriceMessage_Constructor_SetsInputCorrectly(
        byte[] input,
        object[] result
    )
    {
        // Arrange
        // Act
        EquilibriumPriceMessage? output = itchParserService.Parse(input) as EquilibriumPriceMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(result[0], output.MsgType);
        Assert.Equal(result[1], (int)output.Nanos.Value);
        Assert.Equal(result[2], (int)output.OrderBookID.Value);
        Assert.Equal(result[3], (int)output.BidQuantity.Value);
        Assert.Equal(result[4], (int)output.AskQuantity.Value);
        Assert.Equal(result[5], output.EquilibriumPrice.Value);
    }

    [Fact]
    public void EquilibriumPriceMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.Z, 0, 0, 0, 1];

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
    }
}
