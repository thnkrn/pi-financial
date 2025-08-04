using Pi.WalletService.Application.Utilities;

namespace Pi.WalletService.Application.Tests.Utilities;

public class DateUtilsTest
{
    [Fact]
    public void Should_Return_Correct_Result_When_GetThDateTimeNow()
    {
        var expected = DateTime.UtcNow.AddHours(7);

        var actual = DateUtils.GetThDateTimeNow();

        Assert.Equal(expected.Year, actual.Year);
        Assert.Equal(expected.Month, actual.Month);
        Assert.Equal(expected.Day, actual.Day);
        Assert.Equal(expected.Hour, actual.Hour);
        Assert.Equal(expected.Minute, actual.Minute);
    }

    [Fact]
    public void Should_Return_Correct_Result_When_GetThDateTime()
    {
        var expected = DateTime.UtcNow.AddHours(7);
        var actual = DateUtils.GetThDateTime(DateOnly.FromDateTime(expected), TimeOnly.FromDateTime(expected));

        Assert.Equal(expected.Year, actual.Year);
        Assert.Equal(expected.Month, actual.Month);
        Assert.Equal(expected.Day, actual.Day);
        Assert.Equal(expected.Hour, actual.Hour);
        Assert.Equal(expected.Minute, actual.Minute);
    }

    [Theory]
    [InlineData("2023-01-01T00:00:00.000Z", "00:00", "2022-12-31T17:00:00.000Z", "2023-01-01T16:59:59.9999999Z", true)]
    [InlineData("2023-01-01T00:00:00.000Z", "00:00", "2023-01-01T00:00:00.000Z", "2023-01-01T23:59:59.9999999Z", false)]
    [InlineData("2024-05-07T00:00:00.000Z", "05:01", "2024-05-07T05:01:00.000Z", "2024-05-08T05:00:59.9999999Z", false)]
    [InlineData("2024-05-07T00:00:00.000Z", "05:01", "2024-05-06T22:01:00.000Z", "2024-05-07T22:00:59.9999999Z", true)]
    public void Should_Return_Correct_Result_When_GetStartEndDateTime(string dateStr, string cutOffStr, string startStr, string endStr, bool isThTime)
    {
        var date = DateOnly.FromDateTime(DateTime.Parse(dateStr).ToUniversalTime());
        var cutOffTime = TimeOnly.Parse(cutOffStr);
        var start = DateTime.Parse(startStr).ToUniversalTime();
        var end = DateTime.Parse(endStr).ToUniversalTime();

        var (actualStart, actualEnd) = DateUtils.GetUtcStartEndDateTime(date, cutOffTime, isThTime);

        Assert.Equal(start, actualStart);
        Assert.Equal(end, actualEnd);
    }
}