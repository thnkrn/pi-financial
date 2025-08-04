using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;
using Xunit.Abstractions;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public partial class INAVMessageTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public INAVMessageTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
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
                new[] { (byte)'f' }, // Malformed message due to insufficient data
                false, // Expected to fail
                null,
                null,
                null,
                null,
                null,
                null
            }
        };

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

    [Theory]
    [MemberData(nameof(INAVMessageTestData))]
    public async Task Parse_INAVMessage_VerifyBehavior(
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
        var _memoryCache = new Mock<IMemoryCacheHelper>();
        var _memoryLogger = new Mock<ILogger<ItchMessageMetadataHandler>>();
        ItchParserService itchParserService =
            new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

        if (shouldSucceed)
        {
            var message = await itchParserService.Parse(input) as INAVMessage;

            _testOutputHelper.WriteLine($"Parsed Timestamp: {message.Timestamp}");
            _testOutputHelper.WriteLine($"Expected Timestamp: {expectedTimestamp}");

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
            await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
        }
    }
}