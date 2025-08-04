using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models
{
    public partial class MarketStatisticMessageTests
    {
        private static readonly ItchParserService itchParserService = new();
        public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
            ItchMessageType.g
        );

        [Theory]
        [MemberData(nameof(message))]
        public void MarketStatisticMessage_Constructor_SetsInputCorrectly(
            byte[] input,
            object[] result
        )
        {
            // Arrange
            // Act
            MarketStatisticMessage? output = itchParserService.Parse(input) as MarketStatisticMessage;
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
        public void MarketStatisticMessage_Constructor_SetsInputWithIncorrectFormat()
        {
            // Arrange
            byte[] input = [(byte)ItchMessageType.g, 0, 0, 0, 1];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }

    }
}

