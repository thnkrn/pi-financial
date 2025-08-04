using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public partial class ReferencePriceMessageTests
{
    public static IEnumerable<object[]> ReferencePriceMessageTestData =>
        new List<object[]>
        {
            new object[]
            {
                CreateReferencePriceMessage(106001694, 65537, 11, 1000000, "2021-06-01 09:08:12"),
                true, // Should succeed
                106001694U,
                65537U,
                11,
                1000000,
                "2021-06-01 09:08:12.000000000 UTC"
            },
            new object[]
            {
                CreateReferencePriceMessage(
                    111729974,
                    65538,
                    5,
                    -2147483648,
                    "2021-06-02 10:09:13"
                ),
                true, // Should still succeed with special marker values
                111729974U,
                65538U,
                5,
                -2147483648,
                "2021-06-02 10:09:13.000000000 UTC"
            },
            new object[]
            {
                new[] { (byte)'Q' }, // Malformed message due to insufficient data
                false, // Expected to fail
                null,
                null,
                null,
                null,
                null
            }
        };

    public static byte[] CreateReferencePriceMessage(
        uint nanos,
        uint orderBookId,
        byte priceType,
        int price,
        string updatedTimestamp
    )
    {
        var messageBuilder = new List<byte> { (byte)'Q' }; // Message type 'Q'

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, orderBookId, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, priceType, 1);
        MockMessageCreator.AddPrice(ref messageBuilder, price, 4);
        MockMessageCreator.AddTimestamp(ref messageBuilder, updatedTimestamp);

        return messageBuilder.ToArray();
    }

    [Theory]
    [MemberData(nameof(ReferencePriceMessageTestData))]
    public async Task Parse_ReferencePriceMessage_VerifyBehavior(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookId,
        byte expectedPriceType,
        int expectedPrice,
        string expectedUpdatedTimestamp
    )
    {
        var _memoryCache = new Mock<IMemoryCacheHelper>();
        var _memoryLogger = new Mock<ILogger<ItchMessageMetadataHandler>>();
        ItchParserService itchParserService =
            new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

        if (shouldSucceed)
        {
            var message = await itchParserService.Parse(input) as ReferencePriceMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedOrderBookId, message.OrderbookId);
            Assert.Equal(expectedPriceType, message.PriceType);
            Assert.Equal(expectedPrice, message.Price.Value);
            Assert.Equal(expectedUpdatedTimestamp, message.UpdatedTimestamp.ToString());
        }
        else
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
        }
    }
}