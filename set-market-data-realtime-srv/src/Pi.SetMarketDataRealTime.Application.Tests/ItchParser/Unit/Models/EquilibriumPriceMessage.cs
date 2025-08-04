using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public partial class EquilibriumPriceMessageTests
{
    public static IEnumerable<object[]> EquilibriumPriceMessageTestData =>
        new List<object[]>
        {
            new object[]
            {
                CreateEquilibriumPriceMessage(106001694, 65537, 100000, 150000, 500000),
                true, // Should succeed
                106001694U,
                65537U,
                100000UL,
                150000UL,
                500000
            },
            new object[]
            {
                new[] { (byte)'Z' }, // Malformed message due to insufficient data
                false, // Expected to fail
                0U,
                0U,
                0UL,
                0UL,
                0
            }
            // Additional test cases as needed...
        };

    public static byte[] CreateEquilibriumPriceMessage(
        uint nanos,
        uint orderBookID,
        ulong bidQuantity,
        ulong askQuantity,
        int equilibriumPrice
    )
    {
        var messageBuilder = new List<byte> { (byte)'Z' }; // Message type 'Z'

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, orderBookID, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, bidQuantity, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, askQuantity, 8);
        MockMessageCreator.AddPrice(ref messageBuilder, equilibriumPrice, 4);

        return messageBuilder.ToArray();
    }

    [Theory]
    [MemberData(nameof(EquilibriumPriceMessageTestData))]
    public async Task Parse_EquilibriumPriceMessage_VerifyBehavior(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookID,
        ulong expectedBidQuantity,
        ulong expectedAskQuantity,
        int expectedEquilibriumPrice
    )
    {
        var _memoryCache = new Mock<IMemoryCacheHelper>();
        var _memoryLogger = new Mock<ILogger<ItchMessageMetadataHandler>>();
        ItchParserService itchParserService =
            new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));
        if (shouldSucceed)
        {
            var message = await itchParserService.Parse(input) as EquilibriumPriceMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedOrderBookID, message.OrderBookID);
            Assert.Equal(expectedBidQuantity, message.BidQuantity);
            Assert.Equal(expectedAskQuantity, message.AskQuantity);
            Assert.Equal(expectedEquilibriumPrice, message.EquilibriumPrice.Value);
        }
        else
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
        }
    }
}