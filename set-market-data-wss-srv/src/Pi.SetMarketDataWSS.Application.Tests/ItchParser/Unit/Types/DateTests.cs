using Pi.SetMarketDataWSS.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataWSS.Application.Tests.ItchParser.Unit.Types;

public class DateTests
{
    public static IEnumerable<object[]> DateTestData =>
        new List<object[]>
        {
            new object[] { 20230301, "2023-03-01", true },
            new object[] { 20240229, "2024-02-29", true },
            new object[] { 00010101, "0001-01-01", true },
            new object[] { 99991231, "9999-12-31", true },
            new object[] { 20230229, null, false }, // Using null for expected value in error case
        };

    [Theory]
    [MemberData(nameof(DateTestData))]
    public void Date_IntConstructor_ValidatesCorrectly(
        uint input,
        string expectedDateString,
        bool isValid
    )
    {
        if (isValid)
        {
            Date date = new Date(input);
            Assert.Equal(expectedDateString, date.ToString());
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new Date(input));
        }
    }

    [Theory]
    [MemberData(nameof(DateTestData))]
    public void Date_BytesConstructor_ValidatesCorrectly(
        uint input,
        string expectedDateString,
        bool isValid
    )
    {
        byte[] inputBytes = BitConverter.GetBytes(input);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(inputBytes);
        }

        if (isValid)
        {
            Date date = new Date(new ReadOnlySpan<byte>(inputBytes));
            Assert.Equal(expectedDateString, date.ToString());
        }
        else
        {
            Assert.Throws<ArgumentException>(() => new Date(new ReadOnlySpan<byte>(inputBytes)));
        }
    }
}
