using Pi.OnePort.TCP.API.Utils;

namespace Pi.OnePort.TCP.API.Tests.Utils;

public class OperatingHoursUtilsTests
{
    [Theory]
    [MemberData(nameof(GetOperatingDateTimeTestData))]
    public void GetOperatingDateTime_ShouldReturnCorrectResults(
        DateTime currentDateTime,
        TimeOnly openingTime,
        TimeOnly closingTime,
        DateTime expectedOpeningDateTime,
        DateTime expectedClosingDateTime
    )
    {
        var (openingDateTime, closingDateTime) = OperatingHoursUtils.GetOperatingDateTime(currentDateTime, openingTime, closingTime);

        Assert.Equal(expectedOpeningDateTime, openingDateTime);
        Assert.Equal(expectedClosingDateTime, closingDateTime);
    }

    public static IEnumerable<object[]> GetOperatingDateTimeTestData()
    {
        return new List<object[]>
        {
            // inter-day, inside hour, currentTime after new day
            new object[]
            {
                new DateTime(2023, 12, 12, 8, 0, 0),
                new TimeOnly(23, 0),
                new TimeOnly(11, 0),
                new DateTime(2023, 12, 11, 23, 0, 0),
                new DateTime(2023, 12, 12, 11, 0, 0)
            },
            // inter-day, inside hour, currentTime before new day
            new object[]
            {
                new DateTime(2023, 12, 12, 23, 30, 0),
                new TimeOnly(23, 0),
                new TimeOnly(11, 0),
                new DateTime(2023, 12, 12, 23, 0, 0),
                new DateTime(2023, 12, 13, 11, 0, 0)
            },
            // inter-day, after hours
            new object[]
            {
                new DateTime(2023, 12, 12, 14, 0, 0),
                new TimeOnly(23, 0),
                new TimeOnly(11, 0),
                new DateTime(2023, 12, 12, 23, 0, 0),
                new DateTime(2023, 12, 12, 11, 0, 0)
            },
            // single day, before opening hours
            new object[]
            {
                new DateTime(2023, 12, 12, 3, 0, 0),
                new TimeOnly(7, 0),
                new TimeOnly(18, 0),
                new DateTime(2023, 12, 12, 7, 0, 0),
                new DateTime(2023, 12, 11, 18, 0, 0)
            },
            // single day, within opening hours
            new object[]
            {
                new DateTime(2023, 12, 12, 7, 0, 0),
                new TimeOnly(7, 0),
                new TimeOnly(18, 0),
                new DateTime(2023, 12, 12, 7, 0, 0),
                new DateTime(2023, 12, 12, 18, 0, 0)
            },
            // single day, after hours
            new object[]
            {
                new DateTime(2023, 12, 12, 23, 0, 0),
                new TimeOnly(7, 0),
                new TimeOnly(18, 0),
                new DateTime(2023, 12, 13, 7, 0, 0),
                new DateTime(2023, 12, 12, 18, 0, 0)
            },
        };
    }
}
