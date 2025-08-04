using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Interfaces.ItchParser;
using Pi.SetMarketData.Application.Services.ItchParser;
using Pi.SetMarketData.Application.Services.Models.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Tests.ItchParser.Unit.Models;

public class CombinationOrderBookDirectoryParserTests
{
    private readonly IItchParserService itchParserService = new ItchParserService();

    public static byte[] CreateCombinationOrderBookDirectoryMessage(
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

    [Theory]
    [MemberData(nameof(CombinationOrderBookDirectoryMessageTestData))]
    public void Parse_CombinationOrderBookDirectoryMessage_VerifyBehavior(
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
            var message = itchParserService.Parse(input) as CombinationOrderBookDirectoryMessage;

            Assert.NotNull(message);
            Assert.Equal(expectedNanos.Value, message.Nanos);
            Assert.Equal(expectedCombinationOrderBookId.Value, message.CombinationOrderBookId);
            Assert.Equal(expectedLegOrderBookId.Value, message.LegOrderBookId);
            Assert.Equal(expectedLegSide.Value.ToString(), message.LegSide);
            Assert.Equal(expectedLegRatio.Value, message.LegRatio);
        }
        else
        {
            Assert.Throws<ArgumentException>(() => itchParserService.Parse(input));
        }
    }
}
