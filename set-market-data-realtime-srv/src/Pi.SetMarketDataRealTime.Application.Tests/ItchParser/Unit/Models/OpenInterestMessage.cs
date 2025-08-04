using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class OpenInterestMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

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
                new[] { (byte)ItchMessageType.h }, // Malformed message due to insufficient data
                false, // Expected to fail
                null,
                null,
                null,
                null
            }
            // Additional test cases as needed...
        };

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

    [Theory]
    [MemberData(nameof(OpenInterestMessageTestData))]
    public async Task Parse_OpenInterestMessage_VerifyBehavior(
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
            var message = await itchParserService.Parse(input) as OpenInterestMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedOrderBookId, message.OrderbookId);
            Assert.Equal(expectedOpenInterest, message.OpenInterest);
            Assert.Equal(expectedTimestamp,
                message.Timestamp.ToString()); // Assuming Timestamp has ToDateTime() conversion method
        }
        else
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
        }
    }
}