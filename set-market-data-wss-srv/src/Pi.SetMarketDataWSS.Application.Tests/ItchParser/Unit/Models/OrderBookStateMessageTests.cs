using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models
{
    public class OrderBookStateMessageTests
    {
        private static readonly ItchParserService itchParserService = new();
        public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
            ItchMessageType.O
        );

        [Theory]
        [MemberData(nameof(message))]
        public void OrderBookStateMessage_Constructor_SetsInputCorrectly(
            byte[] input,
            object[] result
        )
        {
            // Arrange
            // Act
            OrderBookStateMessage? output = itchParserService.Parse(input) as OrderBookStateMessage;

            // Assert
            Assert.NotNull(output);
            Assert.Equal(result[0], output.MsgType);
            Assert.Equal(result[1], (int)output.Nanos.Value);
            Assert.Equal(result[2], (int)output.OrderBookId.Value);
            Assert.Equal(result[3], output.StateName.Value);
        }

        [Fact]
        public void OrderBookStateMessage_Constructor_SetsInputWithIncorrectFormat()
        {
            // Arrange
            byte[] input = [(byte)ItchMessageType.O, 0, 0, 0, 1];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
