using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models
{
    public partial class ReferencePriceMessageTests
    {
        private static readonly ItchParserService itchParserService = new();
        public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
            ItchMessageType.Q
        );

        [Theory]
        [MemberData(nameof(message))]
        public void ReferencePriceMessage_Constructor_SetsInputCorrectly(
            byte[] input,
            object[] result
        )
        {
            // Arrange
            // Act
            ReferencePriceMessage? output = itchParserService.Parse(input) as ReferencePriceMessage;
            var (dateTime, extraPrecision) = Timestamp.Parse((long)result[5]);
            // format to match the expected output
            var expectedTimeStamp = Timestamp.Formatter(dateTime, extraPrecision);


            // Assert
            Assert.NotNull(output);
            Assert.Equal(result[0], output.MsgType);
            Assert.Equal(result[1], (int)output.Nanos.Value);
            Assert.Equal(result[2], (int)output.OrderbookId.Value);
            Assert.Equal(result[3], (int)output.PriceType.Value);
            Assert.Equal(result[4], output.Price.Value);
            Assert.Equal(expectedTimeStamp, output.UpdatedTimestamp.ToString());

        }

        [Fact]
        public void ReferencePriceMessage_Constructor_SetsInputWithIncorrectFormat()
        {
            // Arrange
            byte[] input = [(byte)ItchMessageType.Q, 0, 0, 0, 1];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }

    }
}

