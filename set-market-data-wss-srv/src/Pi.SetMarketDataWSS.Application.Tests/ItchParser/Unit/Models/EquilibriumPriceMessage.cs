using Pi.SetMarketDataWSS.Application.Interfaces.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;
public partial class EquilibriumPriceMessageTests
{
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
                new byte[] { (byte)'Z' }, // Malformed message due to insufficient data
                false, // Expected to fail
                0U,
                0U,
                0UL,
                0UL,
                0
            }
            // Additional test cases as needed...
        };

    [Theory]
    [MemberData(nameof(EquilibriumPriceMessageTestData))]
    public void Parse_EquilibriumPriceMessage_VerifyBehavior(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookID,
        ulong expectedBidQuantity,
        ulong expectedAskQuantity,
        int expectedEquilibriumPrice
    )
    {
        IItchParserService itchParserService = new ItchParserService();
        if (shouldSucceed)
        {
            var message = itchParserService.Parse(input) as EquilibriumPriceMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedOrderBookID, message.OrderBookID);
            Assert.Equal(expectedBidQuantity, message.BidQuantity);
            Assert.Equal(expectedAskQuantity, message.AskQuantity);
            Assert.Equal(expectedEquilibriumPrice, message.EquilibriumPrice.Value);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
