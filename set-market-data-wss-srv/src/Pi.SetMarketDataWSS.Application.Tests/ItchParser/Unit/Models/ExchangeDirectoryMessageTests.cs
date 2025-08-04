using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public class ExchangeDirectoryParserTests
{
     private static readonly ItchParserService itchParserService = new();
        public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
            ItchMessageType.e
        );

        [Theory]
        [MemberData(nameof(message))]
        public void ExchangeDirectoryMessage_Constructor_SetsInputCorrectly(
            byte[] input,
            object[] result
        )
        {
            // Arrange
            // Act
            ExchangeDirectoryMessage? output = itchParserService.Parse(input) as ExchangeDirectoryMessage;

            // Assert
            Assert.NotNull(output);
            Assert.Equal(result[0], output.MsgType);
            Assert.Equal(result[1], (int)output.Nanos.Value);
            Assert.Equal(result[2], (int)output.ExchangeCode.Value);
            Assert.Equal(result[3], output.ExchangeName.ToString());
        }

        [Fact]
        public void ExchangeDirectoryMessage_Constructor_SetsInputWithIncorrectFormat()
        {
            // Arrange
            byte[] input = [(byte)ItchMessageType.e, 0, 0, 0, 1];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
}
