using Pi.SetMarketDataWSS.Application.Interfaces.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Constants;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;

public class OpenInterestMessageTests
{
    private readonly IItchParserService itchParserService = new ItchParserService();

    private static byte[] CreateOpenInterestMessage(
        uint nanos,
        uint orderBookId,
        ulong openInterest,
        string timestamp
    )
    {
        var messageBuilder = new List<byte> { (byte)'h' }; // Message type 'h'

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, orderBookId, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, openInterest, 8);
        MockMessageCreator.AddTimestamp(ref messageBuilder, timestamp);

        return messageBuilder.ToArray();
    }

    public static IEnumerable<object[]> OpenInterestMessageTestData =>
        new List<object[]>
        {
            new object[]
            {
                CreateOpenInterestMessage(106001694, 65537, 300000, "2021-06-01 09:08:12"),
                true, // Should succeed
                106001694U,
                65537U,
                300000UL,
                "2021-06-01 09:08:12.000000000 UTC"
            },
            new object[]
            {
                new byte[] { (byte)ItchMessageType.h }, // Malformed message due to insufficient data
                false, // Expected to fail
                null,
                null,
                null,
                null
            }
            // Additional test cases as needed...
        };

    [Theory]
    [MemberData(nameof(OpenInterestMessageTestData))]
    public void Parse_OpenInterestMessage_VerifyBehavior(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookId,
        ulong expectedOpenInterest,
        string expectedTimestamp
    )
    {
        if (shouldSucceed)
        {
            var message = itchParserService.Parse(input) as OpenInterestMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedOrderBookId, message.OrderbookId);
            Assert.Equal(expectedOpenInterest, message.OpenInterest);
            Assert.Equal(expectedTimestamp, message.Timestamp.ToString()); // Assuming Timestamp has ToDateTime() conversion method
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
