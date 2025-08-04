using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class CombinationOrderBookDirectoryParserTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    public static IEnumerable<object[]> CombinationOrderBookDirectoryMessageTestData =>
        new List<object[]>
        {
            new object[]
            {
                // Correctly formatted message
                CreateCombinationOrderBookDirectoryMessage(123456789U, 101U, 201U, 'B', 1U),
                (uint?)123456789,
                (uint?)101,
                (uint?)201,
                'B',
                (uint?)1
            },
            new object[]
            {
                // Malformed message (short length)
                new byte[] { (byte)'M', 0, 0, 1 },
                null,
                null,
                null,
                null,
                null // Explicitly stating all expected parameters as null
            },
            new object[]
            {
                // Edge case (invalid Leg Side, represented by 'X')
                CreateCombinationOrderBookDirectoryMessage(987654321U, 102U, 202U, 'X', 1U),
                (uint?)987654321,
                (uint?)102,
                (uint?)202,
                'X',
                (uint?)1
            }
        };

    private static byte[] CreateCombinationOrderBookDirectoryMessage(
        uint nanos,
        uint combinationOrderBookId,
        uint legOrderBookId,
        char legSide,
        uint legRatio
    )
    {
        var messageBuilder = new List<byte>
        {
            (byte)ItchMessageType.M // Message type 'M'
        };

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4); // Nanos
        MockMessageCreator.AddNumeric(ref messageBuilder, combinationOrderBookId, 4); // Combination Order book ID
        MockMessageCreator.AddNumeric(ref messageBuilder, legOrderBookId, 4); // Leg Order book ID
        MockMessageCreator.AddAlpha(ref messageBuilder, legSide.ToString(), 1); // Leg Side
        MockMessageCreator.AddNumeric(ref messageBuilder, legRatio, 4); // Leg Ratio

        return messageBuilder.ToArray();
    }

    [Theory]
    [MemberData(nameof(CombinationOrderBookDirectoryMessageTestData))]
    public async Task Parse_CombinationOrderBookDirectoryMessage_VerifyBehavior(
        byte[] input,
        uint? expectedNanos,
        uint? expectedCombinationOrderBookId,
        uint? expectedLegOrderBookId,
        char? expectedLegSide,
        uint? expectedLegRatio
    )
    {
        if (expectedNanos.HasValue)
        {
            var message = await itchParserService.Parse(input) as CombinationOrderBookDirectoryMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos.Value, message.Nanos);
            if (expectedCombinationOrderBookId != null)
                Assert.Equal(expectedCombinationOrderBookId.Value, message.CombinationOrderBookId);
            if (expectedLegOrderBookId != null) Assert.Equal(expectedLegOrderBookId.Value, message.LegOrderBookId);
            if (expectedLegSide != null) Assert.Equal(expectedLegSide.Value.ToString(), message.LegSide);
            if (expectedLegRatio != null) Assert.Equal(expectedLegRatio.Value, message.LegRatio);
        }
        else
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
        }
    }
}