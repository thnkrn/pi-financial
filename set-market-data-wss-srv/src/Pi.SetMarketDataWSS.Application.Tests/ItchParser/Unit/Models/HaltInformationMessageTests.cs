using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models
{
    public class HaltInformationMessageTests
    {
        private static readonly ItchParserService itchParserService = new();

        public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
            ItchMessageType.l
        );

        [Theory]
        [MemberData(nameof(message))]
        public void HaltInformationMessage_Constructor_SetsInputCorrectly(
            byte[] input,
            object[] result
        )
        {
            // Arrange
            // Act

            HaltInformationMessage? output =
                itchParserService.Parse(input) as HaltInformationMessage;

            // Assert
            Assert.NotNull(output);
            Assert.Equal(result[0], output.MsgType);
            Assert.Equal(result[1], (int)output.Nanos.Value);
            Assert.Equal(result[2], (int)output.OrderBookId.Value);
            Assert.Equal(result[3], output.InstrumentState.Value);
        }

        [Fact]
        public void HaltInformationMessage_Constructor_SetsInputWithIncorrectFormat()
        {
            // Arrange
            byte[] input = [(byte)ItchMessageType.l, 0, 0, 0, 1];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
