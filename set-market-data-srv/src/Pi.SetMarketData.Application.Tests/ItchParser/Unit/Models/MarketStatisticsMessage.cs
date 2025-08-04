using Pi.SetMarketData.Application.Interfaces.ItchParser;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;
public partial class MarketStatisticMessageTests
{
    public static byte[] CreateMarketStatisticMessage(
        uint nanos,
        string marketStatisticsID,
        string currency,
        string marketStatisticsTime,
        uint totalTrades,
        long totalQuantity,
        ulong totalValue,
        ulong upQuantity,
        ulong downQuantity,
        ulong noChangeVolume,
        uint upShares,
        uint downShares,
        uint noChangeShares
    )
    {
        var messageBuilder = new List<byte> { (byte)'g' }; // Message type 'g'

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4);
        MockMessageCreator.AddAlpha(ref messageBuilder, marketStatisticsID, 12);
        MockMessageCreator.AddAlpha(ref messageBuilder, currency, 3);
        MockMessageCreator.AddTimestamp(ref messageBuilder, marketStatisticsTime);
        MockMessageCreator.AddNumeric(ref messageBuilder, totalTrades, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, totalQuantity, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, totalValue, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, upQuantity, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, downQuantity, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, noChangeVolume, 8);
        MockMessageCreator.AddNumeric(ref messageBuilder, upShares, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, downShares, 4);
        MockMessageCreator.AddNumeric(ref messageBuilder, noChangeShares, 4);

        return messageBuilder.ToArray();
    }

    public static IEnumerable<object[]> MarketStatisticMessageTestData =>
        new List<object[]>
        {
            new object[]
            {
                CreateMarketStatisticMessage(
                    106001694,
                    "SET",
                    "THB",
                    "2021-06-01 09:08:12",
                    15000,
                    250000000,
                    35000000000,
                    150000000,
                    100000000,
                    0,
                    200,
                    150,
                    50
                ),
                true, // Should succeed
                106001694U,
                "SET",
                "THB",
                "2021-06-01 09:08:12.000000000 UTC",
                15000U,
                250000000UL,
                35000000000UL,
                150000000UL,
                100000000UL,
                0UL,
                200U,
                150U,
                50U
            },
            new object[]
            {
                CreateMarketStatisticMessage(
                    111729974,
                    "MAI",
                    "USD",
                    "2021-06-02 10:09:13",
                    20000,
                    300000000,
                    45000000000,
                    200000000,
                    100000000,
                    0,
                    250,
                    180,
                    70
                ),
                true, // Should succeed
                111729974U,
                "MAI",
                "USD",
                "2021-06-02 10:09:13.000000000 UTC",
                20000U,
                300000000UL,
                45000000000UL,
                200000000UL,
                100000000UL,
                0UL,
                250U,
                180U,
                70U
            },
            // Malformed message due to insufficient data
            new object[]
            {
                new byte[] { (byte)'g' }, // Insufficient data for a complete message
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
            }
        };

    [Theory]
    [MemberData(nameof(MarketStatisticMessageTestData))]
    public void Parse_MarketStatisticMessage_VerifyBehavior(
        byte[] input,
        bool shouldSucceed,
        uint expectedNanos,
        string expectedMarketStatisticsID,
        string expectedCurrency,
        string expectedMarketStatisticsTime,
        uint expectedTotalTrades,
        ulong expectedTotalQuantity,
        ulong expectedTotalValue,
        ulong expectedUpQuantity,
        ulong expectedDownQuantity,
        ulong expectedNoChangeVolume,
        uint expectedUpShares,
        uint expectedDownShares,
        uint expectedNoChangeShares
    )
    {
        IItchParserService itchParserService = new ItchParserService();
        if (shouldSucceed)
        {
            var message = itchParserService.Parse(input) as MarketStatisticMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos, message.Nanos);
            Assert.Equal(expectedMarketStatisticsID, message.MarketStatisticsID.ToString().Trim());
            Assert.Equal(expectedCurrency, message.Currency.ToString());
            Assert.Equal(expectedMarketStatisticsTime, message.MarketStatisticsTime.ToString());
            Assert.Equal(expectedTotalTrades, message.TotalTrades);
            Assert.Equal(expectedTotalQuantity, message.TotalQuantity);
            Assert.Equal(expectedTotalValue, message.TotalValue);
            Assert.Equal(expectedUpQuantity, message.UpQuantity);
            Assert.Equal(expectedDownQuantity, message.DownQuantity);
            Assert.Equal(expectedNoChangeVolume, message.NoChangeVolume);
            Assert.Equal(expectedUpShares, message.UpShares);
            Assert.Equal(expectedDownShares, message.DownShares);
            Assert.Equal(expectedNoChangeShares, message.NoChangeShares);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
