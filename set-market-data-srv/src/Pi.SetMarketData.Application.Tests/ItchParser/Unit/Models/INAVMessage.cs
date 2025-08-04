using Pi.SetMarketData.Application.Interfaces.ItchParser;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;
public partial class INAVMessageTests
{
    public static byte[] CreateINAVMessage(
        uint nanos,
        uint orderBookId,
        int inav,
        int change,
        int percentageChange,
        string timestamp
    )
    {
        var messageBuilder = new List<byte> { (byte)'f' }; // Message type 'f'

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, orderBookId, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, inav, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, change, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, percentageChange, 4);
        MockMessageCreator.AddTimestamp(ref messageBuilder, timestamp);

        return messageBuilder.ToArray();
    }

    public static IEnumerable<object[]> INAVMessageTestData =>
        new List<object[]>
        {
            new object[]
            {
                CreateINAVMessage(106001694, 65537, 1000000, 5000, 50, "2021-06-01 09:08:12"),
                true, // Should succeed
                106001694U,
                65537U,
                1000000,
                5000,
                50,
                "2021-06-01 09:08:12.000000000 UTC"
            },
            new object[]
            {
                CreateINAVMessage(
                    111729974,
                    65537,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    "2021-06-01 09:08:12"
                ),
                true, // Should succeed even with reset values
                111729974U,
                65537U,
                -2147483648,
                -2147483648,
                -2147483648,
                "2021-06-01 09:08:12.000000000 UTC"
            },
            new object[]
            {
                new byte[] { (byte)'f' }, // Malformed message due to insufficient data
                false, // Expected to fail
                null,
                null,
                null,
                null,
                null,
                null
            }
        };

    [Theory]
    [MemberData(nameof(INAVMessageTestData))]
    public void Parse_INAVMessage_VerifyBehavior1(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookId,
        int expectedINAV,
        int expectedChange,
        int expectedPercentageChange,
        string expectedTimestamp
    )
    {
        IItchParserService itchParserService = new ItchParserService();
        if (shouldSucceed)
        {
            var message = itchParserService.Parse(input) as INAVMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos.Value);
            Assert.Equal(expectedOrderBookId, message.OrderBookId);
            Assert.Equal(expectedINAV, message.INAV.Value);
            Assert.Equal(expectedChange, message.Change.Value);
            Assert.Equal(expectedPercentageChange, message.PercentageChange.Value);
            Assert.Equal(expectedTimestamp, message.Timestamp.ToString());
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
