using Pi.GlobalMarketDataWSS.Application.Helpers;
using Xunit;

namespace Pi.GlobalMarketDataWSS.Application.Tests.Helpers;
public class TimestampExtensionTest
{
    public TimestampExtensionTest()
    {
        
    }

    [Theory]
    [InlineData("2024-01-01 00:00:04", 1704067204)]
    [InlineData("2024-09-09 11:45:15", 1725882315)]
    public void ToNanosTimestamp_ConvertValidDatetime_WhenGotUTCDate(string datetime, long expectedEpoch)
    {
        var result = TimestampExtension.ToNanosTimestamp(datetime);

        Assert.Equal(expectedEpoch, result);
    }

    [Theory]
    [InlineData("2024-01-01 20:59:59", "03:59:59")]
    public void ExtractTimeFromDateTime_ShouldReturnCorrectValue(string dateTime, string expectedDateTime)
    {
        string result = TimestampExtension.ExtractTimeFromDateTime(dateTime);

        Assert.Equal(expectedDateTime, result);
    }

    [Theory]
    [InlineData("",0)]
    [InlineData(null, 0)]
    public void ToUnixMillisecondTimestamp_ShouldReturn_0(string datetime, long expectedEpoch)
    {
        var result = TimestampExtension.ToUnixMillisecondTimestamp(datetime);
    
        Assert.Equal(expectedEpoch, result);

    }
}