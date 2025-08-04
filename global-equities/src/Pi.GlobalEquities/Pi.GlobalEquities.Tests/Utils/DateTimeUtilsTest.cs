using Pi.GlobalEquities.Utils;
using Xunit;

namespace Pi.GlobalEquities.Tests.Utils;

public class DateTimeUtilsTest
{
    public class ConvertToTimestamp_Test
    {
        [Fact]
        void WhenCovertDateTimeToTimestamp_ReturnTimeStamp()
        {
            var dateTime = new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc);

            var timestampInMillisec = DateTimeUtils.ConvertToTimestamp(dateTime);

            Assert.Equal(1735084800000, timestampInMillisec);
        }
    }

    public class ConvertToDateTimeUtc_Test
    {
        [Fact]
        void WhenCovertTimestampToDateTime_ReturnDateTime()
        {
            var timeStamp = 1735084800000;

            var dateTime = DateTimeUtils.ConvertToDateTimeUtc(timeStamp);

            var dateTimeUtc = new DateTime(2024, 12, 25, 0, 0, 0, 0, DateTimeKind.Utc);
            Assert.Equal(dateTimeUtc, dateTime);
        }
    }

    public class ConvertToDateTimeUs_Test
    {
        [Fact]
        void WhenCovertUtcDateTimeToUsTime_ReturnUSDateTime()
        {
            var dateTimeUtc = new DateTime(2025, 01, 02, 4, 10, 10, DateTimeKind.Utc);

            var dateTimeUs = DateTimeUtils.ConvertToDateTimeUs(dateTimeUtc);

            var expected = new DateTime(2025, 01, 01, 23, 10, 10, DateTimeKind.Unspecified); //-4/-5
            Assert.Equal(expected, dateTimeUs);
        }

        [Fact]
        void WhenCovertUtcDateTimeToHKTime_ReturnHKDateTime()
        {
            var dateTimeUtc = new DateTime(2025, 01, 02, 4, 10, 10, DateTimeKind.Utc);

            var dateTimeUs = DateTimeUtils.ConvertToDateTimeHk(dateTimeUtc);

            var expected = new DateTime(2025, 01, 02, 12, 10, 10, DateTimeKind.Unspecified);  //+8
            Assert.Equal(expected, dateTimeUs);
        }
    }
}
