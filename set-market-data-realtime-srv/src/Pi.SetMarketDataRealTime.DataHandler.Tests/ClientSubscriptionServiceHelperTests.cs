using Pi.SetMarketDataRealTime.DataHandler.Helpers;
using Xunit.Abstractions;

namespace Pi.SetMarketDataRealTime.DataHandler.Tests;

public class ClientSubscriptionServiceHelperTests
{
    private static readonly DateTime BaseDate = new(2024, 1, 1);
    private static readonly DateTime NextOpenRun = BaseDate.AddHours(8);
    private static readonly DateTime NextCloseRun = BaseDate.AddHours(18);
    private readonly ITestOutputHelper _output;

    /// <inheritdoc>
    ///     <cref></cref>
    /// </inheritdoc>
    public ClientSubscriptionServiceHelperTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(7, 59, false, false, true)] // Just before open
    [InlineData(8, 0, true, false, false)] // Exactly at open
    [InlineData(8, 1, true, true, false)] // Just after open
    [InlineData(13, 0, true, true, false)] // Middle of the day
    [InlineData(17, 59, true, true, false)] // Just before close
    [InlineData(18, 0, false, false, true)] // Exactly at close
    [InlineData(18, 1, false, false, true)] // Just after close
    [InlineData(23, 59, false, false, true)] // Late night
    [InlineData(0, 0, false, false, true)] // Midnight
    [InlineData(7, 0, false, false, true)] // Early morning next day
    public void EvaluateSchedule_ReturnsCorrectResult(int hour, int minute, bool expectedIsWithinValidTimeRange,
        bool expectedHasMissedStart, bool expectedHasMissedStop)
    {
        // Arrange
        var thaiNow = BaseDate.AddHours(hour).AddMinutes(minute);

        // Act
        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(thaiNow, NextOpenRun, NextCloseRun);

        // Log detailed information
        _output.WriteLine($"Test Case: {hour:D2}:{minute:D2}");
        _output.WriteLine($"thaiNow: {thaiNow}");
        _output.WriteLine($"NextOpenRun: {NextOpenRun}");
        _output.WriteLine($"NextCloseRun: {NextCloseRun}");
        _output.WriteLine(
            $"Expected IsWithinValidTimeRange: {expectedIsWithinValidTimeRange}, Actual: {result.IsWithinValidTimeRange}");
        _output.WriteLine($"Expected HasMissedStart: {expectedHasMissedStart}, Actual: {result.HasMissedStart}");
        _output.WriteLine($"Expected HasMissedStop: {expectedHasMissedStop}, Actual: {result.HasMissedStop}");
        _output.WriteLine("");

        // Assert
        Assert.Equal(expectedIsWithinValidTimeRange, result.IsWithinValidTimeRange);
        Assert.Equal(expectedHasMissedStart, result.HasMissedStart);
        Assert.Equal(expectedHasMissedStop, result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_HandlesOvernight()
    {
        // Arrange
        var baseDate = new DateTime(2024, 1, 1);
        var nextOpenRun = baseDate.AddHours(22); // 10:00 PM
        var nextCloseRun = baseDate.AddDays(1).AddHours(6); // 6:00 AM next day

        var testCases = new[]
        {
            (time: baseDate.AddHours(21).AddMinutes(59), isWithin: false, missedStart: false,
                missedStop: true), // 9:59 PM
            (time: baseDate.AddHours(22), isWithin: true, missedStart: false, missedStop: false), // 10:00 PM
            (time: baseDate.AddHours(23), isWithin: true, missedStart: true, missedStop: false), // 11:00 PM
            (time: baseDate.AddDays(1), isWithin: true, missedStart: true, missedStop: false), // 12:00 AM
            (time: baseDate.AddDays(1).AddHours(5).AddMinutes(59), isWithin: true, missedStart: true,
                missedStop: false), // 5:59 AM
            (time: baseDate.AddDays(1).AddHours(6), isWithin: false, missedStart: false, missedStop: true), // 6:00 AM
            (time: baseDate.AddDays(1).AddHours(7), isWithin: false, missedStart: false, missedStop: true) // 7:00 AM
        };

        foreach (var (time, isWithin, missedStart, missedStop) in testCases)
        {
            // Act
            var result = ClientSubscriptionServiceHelper.EvaluateSchedule(time, nextOpenRun, nextCloseRun);

            // Log detailed information
            _output.WriteLine($"Test Case: Overnight - {time:HH:mm}");
            _output.WriteLine($"thaiNow: {time}");
            _output.WriteLine($"NextOpenRun: {nextOpenRun}");
            _output.WriteLine($"NextCloseRun: {nextCloseRun}");
            _output.WriteLine($"Expected IsWithinValidTimeRange: {isWithin}, Actual: {result.IsWithinValidTimeRange}");
            _output.WriteLine($"Expected HasMissedStart: {missedStart}, Actual: {result.HasMissedStart}");
            _output.WriteLine($"Expected HasMissedStop: {missedStop}, Actual: {result.HasMissedStop}");
            _output.WriteLine("");

            // Assert
            Assert.Equal(isWithin, result.IsWithinValidTimeRange);
            Assert.Equal(missedStart, result.HasMissedStart);
            Assert.Equal(missedStop, result.HasMissedStop);
        }
    }

    [Fact]
    public void EvaluateSchedule_MidnightSpanningSchedule_BeforeMidnight_AfterStart()
    {
        var now = new DateTime(2023, 8, 2, 23, 30, 0);
        var open = new DateTime(2023, 8, 2, 22, 0, 0);
        var close = new DateTime(2023, 8, 3, 6, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, open, close);

        // Log detailed information
        _output.WriteLine($"Test Case: Overnight - {now:HH:mm}");
        _output.WriteLine($"thaiNow: {now}");
        _output.WriteLine($"NextOpenRun: {open}");
        _output.WriteLine($"NextCloseRun: {close}");
        _output.WriteLine($"Expected IsWithinValidTimeRange: {true}, Actual: {result.IsWithinValidTimeRange}");
        _output.WriteLine($"Expected HasMissedStart: {true}, Actual: {result.HasMissedStart}");
        _output.WriteLine($"Expected HasMissedStop: {false}, Actual: {result.HasMissedStop}");
        _output.WriteLine("");

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_MidnightSpanningSchedule_JustAfterMidnight()
    {
        var now = new DateTime(2023, 8, 3, 0, 1, 0);
        var open = new DateTime(2023, 8, 2, 22, 0, 0);
        var close = new DateTime(2023, 8, 3, 6, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, open, close);

        // Log detailed information
        _output.WriteLine($"Test Case: Overnight - {now:HH:mm}");
        _output.WriteLine($"thaiNow: {now}");
        _output.WriteLine($"NextOpenRun: {open}");
        _output.WriteLine($"NextCloseRun: {close}");
        _output.WriteLine($"Expected IsWithinValidTimeRange: {true}, Actual: {result.IsWithinValidTimeRange}");
        _output.WriteLine($"Expected HasMissedStart: {true}, Actual: {result.HasMissedStart}");
        _output.WriteLine($"Expected HasMissedStop: {false}, Actual: {result.HasMissedStop}");
        _output.WriteLine("");

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_MidnightSpanningSchedule_CloseToStop()
    {
        var now = new DateTime(2023, 8, 3, 5, 59, 0);
        var open = new DateTime(2023, 8, 2, 22, 0, 0);
        var close = new DateTime(2023, 8, 3, 6, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, open, close);

        // Log detailed information
        _output.WriteLine($"Test Case: Overnight - {now:HH:mm}");
        _output.WriteLine($"thaiNow: {now}");
        _output.WriteLine($"NextOpenRun: {open}");
        _output.WriteLine($"NextCloseRun: {close}");
        _output.WriteLine($"Expected IsWithinValidTimeRange: {true}, Actual: {result.IsWithinValidTimeRange}");
        _output.WriteLine($"Expected HasMissedStart: {true}, Actual: {result.HasMissedStart}");
        _output.WriteLine($"Expected HasMissedStop: {false}, Actual: {result.HasMissedStop}");
        _output.WriteLine("");

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_MidnightSpanningSchedule_AfterStop()
    {
        var now = new DateTime(2023, 8, 3, 6, 1, 0);
        var open = new DateTime(2023, 8, 2, 22, 0, 0);
        var close = new DateTime(2023, 8, 3, 6, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, open, close);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.True(result.HasMissedStop);
    }
}