using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class TimeTests
{
    [Theory]
    [InlineData(120000, "12:00:00")]
    [InlineData(000000, "00:00:00")]
    [InlineData(235959, "23:59:59")]
    [InlineData(010203, "01:02:03")]
    public void Time_IntegerConstructor_ParsesCorrectly(int input, string expected)
    {
        Time time = new Time(input);
        Assert.Equal(expected, time.ToString());
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x01, 0xD4, 0xC0 }, "12:00:00")] // 120000 in big endian
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "00:00:00")] // 000000 in big endian
    [InlineData(new byte[] { 0x00, 0x03, 0x99, 0xB7 }, "23:59:59")] // 235959 in big endian
    [InlineData(new byte[] { 0x00, 0x00, 0x27, 0xDB }, "01:02:03")] // 010203 in big endian
    public void Time_BytesConstructor_ParsesCorrectly(byte[] input, string expected)
    {
        Time time = new Time(new ReadOnlySpan<byte>(input));
        Assert.Equal(expected, time.ToString());
    }

    [Theory]
    [InlineData(-10000)]
    [InlineData(240000)] // Invalid hour
    [InlineData(006061)] // Invalid minute
    [InlineData(000060)] // Invalid second
    public void Time_InvalidTime_ThrowsArgumentException(int input)
    {
        Assert.Throws<ArgumentException>(() => new Time(input));
    }
}
