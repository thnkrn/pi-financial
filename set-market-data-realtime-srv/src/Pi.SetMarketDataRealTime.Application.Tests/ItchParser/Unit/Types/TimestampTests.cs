using System.Globalization;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Types;

public class TimestampTests
{
    public static IEnumerable<object[]> TimestampTestData =>
        new List<object[]>
        {
            new object[] { 0L, "1970-01-01 00:00:00.000000000 UTC" },
            new object[] { 1596240000000000000L, "2020-08-01 00:00:00.000000000 UTC" }
        };

    public static IEnumerable<object[]> GetCurrentDateTestData()
    {
        var currentDate = DateTime.UtcNow;
        var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var nanosecondsSinceEpoch = (currentDate - epochStart).Ticks * 100; // 1 tick = 100 nanoseconds

        yield return new object[]
        {
            nanosecondsSinceEpoch,
            currentDate.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture)
            + "00"
            + " UTC"
        };
    }

    public static IEnumerable<object[]> GetMaxDateTestData()
    {
        var maxDate = new DateTime(2262, 4, 11, 23, 47, 16, DateTimeKind.Utc); // Max nanoseconds for 8 bytes signed int
        var epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var ticksSinceEpoch = maxDate.Ticks - epochStart.Ticks;
        var nanosecondsSinceEpoch = ticksSinceEpoch * 100;

        yield return new object[]
        {
            nanosecondsSinceEpoch,
            "2262-04-11 23:47:16.000000000" + " UTC"
        };
    }

    [Theory]
    [MemberData(nameof(TimestampTestData))]
    [MemberData(nameof(GetCurrentDateTestData))]
    [MemberData(nameof(GetMaxDateTestData))]
    public void Timestamp_Constructor_ValidatesCorrectly(long input, string expected)
    {
        var timestamp = new Timestamp(input);
        Assert.Equal(expected, timestamp.ToString());
    }

    [Theory]
    [MemberData(nameof(TimestampTestData))]
    [MemberData(nameof(GetCurrentDateTestData))]
    [MemberData(nameof(GetMaxDateTestData))]
    public void Timestamp_BytesConstructor_ValidatesCorrectly(long input, string expected)
    {
        var bytes = BitConverter.GetBytes(input);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        var timestamp = new Timestamp(new ReadOnlySpan<byte>(bytes));
        Assert.Equal(expected, timestamp.ToString());
    }
}