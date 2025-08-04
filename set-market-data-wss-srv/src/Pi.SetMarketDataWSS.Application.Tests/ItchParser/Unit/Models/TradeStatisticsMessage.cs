using Pi.SetMarketDataWSS.Application.Interfaces.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.ItchParser;
using Pi.SetMarketDataWSS.Application.Services.Models.ItchParser;
using Pi.SetMarketDataWSS.Application.Utils;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Models;
public partial class TradeStatisticsMessageTests
{
    public static byte[] CreateTradeStatisticsMessage(
        uint nanos,
        uint orderBookId,
        int openPrice,
        int highPrice,
        int lowPrice,
        int lastPrice,
        int lastAuctionPrice,
        long turnOverQuantity,
        long reportedTurnOverQuantity,
        long turnOverValue,
        long reportedTurnOverValue,
        int averagePrice,
        long totalNumberOfTrades
    )
    {
        var messageBuilder = new List<byte> { (byte)'I' }; // Message type 'I'

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, orderBookId, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, openPrice, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, highPrice, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, lowPrice, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, lastPrice, 4);
        MockMessageCreator.AddPrice(ref messageBuilder, lastAuctionPrice, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, turnOverQuantity, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, reportedTurnOverQuantity, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, turnOverValue, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, reportedTurnOverValue, 8);
        MockMessageCreator.AddPrice(ref messageBuilder, averagePrice, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, totalNumberOfTrades, 8);

        return messageBuilder.ToArray();
    }

    public static IEnumerable<object[]> TradeStatisticsMessageTestData =>
        new List<object[]>
        {
            //TC1-4: SET examples
            new object[]
            {
                CreateTradeStatisticsMessage(
                    106001694,
                    65537,
                    410000,
                    410000,
                    400000,
                    400000,
                    410000,
                    6003200,
                    6000000,
                    2451310000000L,
                    2450000000000L,
                    409375,
                    19
                ),
                true, // Should succeed
                106001694U,
                65537U,
                410000,
                410000,
                400000,
                400000,
                410000,
                6003200UL,
                6000000UL,
                2451310000000L,
                2450000000000L,
                409375,
                19L
            },
            new object[]
            {
                CreateTradeStatisticsMessage(
                    111729974,
                    65537,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    1000000,
                    1000000,
                    410000000000L,
                    410000000000L,
                    -2147483648,
                    1
                ),
                true, // Should succeed
                111729974U,
                65537U,
                -2147483648,
                -2147483648,
                -2147483648,
                -2147483648,
                -2147483648,
                1000000UL,
                1000000UL,
                410000000000L,
                410000000000L,
                -2147483648,
                1L
            },
            new object[]
            {
                CreateTradeStatisticsMessage(
                    348248857,
                    413273,
                    6000000,
                    6010000,
                    6000000,
                    6010000,
                    -2147483648,
                    141,
                    100,
                    0,
                    0,
                    6000975,
                    0
                ),
                true, // Should succeed
                348248857U,
                413273U,
                6000000,
                6010000,
                6000000,
                6010000,
                -2147483648,
                141UL,
                100UL,
                0L,
                0L,
                6000975,
                0L
            },
            new object[]
            {
                CreateTradeStatisticsMessage(
                    584043385,
                    413273,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    -2147483648,
                    0,
                    0,
                    0,
                    0,
                    -2147483648,
                    0
                ),
                true, // Should succeed
                584043385U,
                413273U,
                -2147483648,
                -2147483648,
                -2147483648,
                -2147483648,
                -2147483648,
                0UL,
                0UL,
                0L,
                0L,
                -2147483648,
                0L
            },
            new object[]
            {
                // TC5: Malformed message due to missing bytes
                new byte[] { (byte)'I', 0, 1, 2 }, // Insufficient data for a complete message
                false, // Expected to fail
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            },
        };

    [Theory]
    [MemberData(nameof(TradeStatisticsMessageTestData))]
    public void Parse_TradeStatisticsMessage_VerifyBehavior1(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        uint expectedOrderBookId,
        int expectedOpenPrice,
        int expectedHighPrice,
        int expectedLowPrice,
        int expectedLastPrice,
        int expectedLastAuctionPrice,
        ulong expectedTurnOverQuantity,
        ulong expectedReportedTurnOverQuantity,
        long expectedTurnOverValue,
        long expectedReportedTurnOverValue,
        int expectedAveragePrice,
        ulong expectedTotalNumberOfTrades
    )
    {
        IItchParserService itchParserService = new ItchParserService();
        if (shouldSucceed)
        {
            var message = itchParserService.Parse(input) as TradeStatisticsMessage;

            Assert.NotNull(message);

            Assert.Equal(expectedNanos, message.Nanos.Value);
            Assert.Equal(expectedOrderBookId, message.OrderBookId);
            Assert.Equal(expectedOpenPrice, message.OpenPrice.Value);
            Assert.Equal(expectedHighPrice, message.HighPrice.Value);
            Assert.Equal(expectedLowPrice, message.LowPrice.Value);
            Assert.Equal(expectedLastPrice, message.LastPrice.Value);
            Assert.Equal(expectedLastAuctionPrice, message.LastAuctionPrice.Value);
            Assert.Equal(expectedTurnOverQuantity, message.TurnoverQuantity);
            Assert.Equal(expectedReportedTurnOverQuantity, message.ReportedTurnoverQuantity);
            Assert.Equal(expectedTurnOverValue, message.TurnoverValue.Value);
            Assert.Equal(expectedReportedTurnOverValue, message.ReportedTurnoverValue.Value);
            Assert.Equal(expectedAveragePrice, message.AveragePrice.Value);
            Assert.Equal(expectedTotalNumberOfTrades, message.TotalNumberOfTrades);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
