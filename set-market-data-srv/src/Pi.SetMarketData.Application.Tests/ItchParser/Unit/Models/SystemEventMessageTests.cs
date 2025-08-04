using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Tests.ItchParser.Mocks;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models
{
    public class SystemEventMessageTests
    {
        private static readonly ItchParserService itchParserService = new();
        public static readonly IEnumerable<object[]> message = ItchMockMessage.GetMessage(
            ItchMessageType.S
        );

        [Theory]
        [MemberData(nameof(message))]
        public void SystemEventMessage_Constructor_SetsInputCorrectly(
            byte[] input,
            object[] result
        )
        {
            // Arrange
            // Act
            SystemEventMessage? output = itchParserService.Parse(input) as SystemEventMessage;

            // Assert
            Assert.NotNull(output);
            Assert.Equal(result[0], output.MsgType);
            Assert.Equal(result[1], (int)output.Nanos.Value);
            Assert.Equal(result[2], output.EventCode.ToString());
        }

        [Fact]
        public void SystemEventMessage_Constructor_SetsInputWithIncorrectFormat()
        {
            // Arrange
            byte[] input = [(byte)ItchMessageType.S, 0, 0, 0, 1];

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
