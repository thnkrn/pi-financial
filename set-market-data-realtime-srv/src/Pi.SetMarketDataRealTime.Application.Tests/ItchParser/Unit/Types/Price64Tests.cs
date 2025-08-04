using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Types;

public class Price64Tests
{
    public static IEnumerable<object[]> PriceData =>
        new List<object[]>
        {
            new object[]
            {
                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x1D, 0xCD, 0x65, 0x00 },
                3,
                500000.000m
            },
            new object[]
            {
                new byte[] { 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF },
                5,
                92233720368547.75807m
            }
        };

    [Theory]
    [MemberData(nameof(PriceData))]
    public void Price64_ConvertsToExpectedValue(byte[] input, int decimals, decimal expected)
    {
        var price = new Price64(input) { NumberOfDecimals = decimals };
        decimal result = price;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Price64_NoPriceAvailable_ThrowsInvalidOperationException()
    {
        var input = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x80, 0x00, 0x00, 0x00 };

        Assert.Throws<InvalidOperationException>(() => new Price64(input).ToFloat());
    }
}