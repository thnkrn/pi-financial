using Pi.SetMarketData.Application.Interfaces.ItchParser;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;
public partial class ReferencePriceMessageTests
{
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
                new byte[] { (byte)'Q' }, // Malformed message due to insufficient data
                false, // Expected to fail
                null,
                null,
                null,
                null,
                null
            }
        };

    [Theory]
    [MemberData(nameof(ReferencePriceMessageTestData))]
    public void Parse_ReferencePriceMessage_VerifyBehavior(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookId,
        byte expectedPriceType,
        int expectedPrice,
        string expectedUpdatedTimestamp
    )
    {
        IItchParserService itchParserService = new ItchParserService();
        if (shouldSucceed)
        {
            var message = itchParserService.Parse(input) as ReferencePriceMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedOrderBookId, message.OrderbookId);
            Assert.Equal(expectedPriceType, message.PriceType);
            Assert.Equal(expectedPrice, message.Price.Value);
            Assert.Equal(expectedUpdatedTimestamp, message.UpdatedTimestamp.ToString());
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
