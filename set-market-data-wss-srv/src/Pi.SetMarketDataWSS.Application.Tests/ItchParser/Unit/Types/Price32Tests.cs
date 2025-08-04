using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class Price32Tests
{
    [Theory]
    [InlineData(new byte[] { 0x00, 0x01, 0x86, 0xA0 }, 2, 1000.00)] // 100000
    [InlineData(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF }, 2, 21474836.47)] // 2147483647
    public void Price32_ConvertsToExpectedValue(byte[] input, int decimals, decimal expected)
    {
        Price32 price = new Price32(input) { NumberOfDecimals = decimals };
        decimal result = price;
        Assert.Equal(expected, result, decimals);
    }

    [Fact]
    public void Price32_NoPriceAvailable_ThrowsInvalidOperationException()
    {
        var input = new byte[] { 0x80, 0x00, 0x00, 0x00 };

        Assert.Throws<InvalidOperationException>(() => new Price32(input).ToFloat());
    }
}
