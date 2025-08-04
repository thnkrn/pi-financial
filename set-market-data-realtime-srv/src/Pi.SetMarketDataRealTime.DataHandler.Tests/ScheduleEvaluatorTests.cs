using Pi.SetMarketDataRealTime.DataHandler.Helpers;

namespace Pi.SetMarketDataRealTime.DataHandler.Tests;

public class ScheduleEvaluatorTests
{
    [Fact]
    public void EvaluateSchedule_WithinValidRange_SubscriptionRunning()
    {
        var now = new DateTime(2024, 9, 12, 10, 0, 0); // 10:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_WithinValidRange_SubscriptionNotRunning()
    {
        var now = new DateTime(2024, 9, 12, 10, 0, 0); // 10:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_OutsideValidRange_BeforeOpen()
    {
        var now = new DateTime(2024, 9, 12, 6, 0, 0); // 6:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_OutsideValidRange_AfterClose()
    {
        var now = new DateTime(2024, 9, 13, 4, 0, 0); // 4:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM previous day
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.True(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_EdgeCase_ExactlyAtOpenTime()
    {
        var now = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_EdgeCase_JustBeforeCloseTime()
    {
        var now = new DateTime(2024, 9, 13, 2, 59, 0); // 2:59 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM previous day
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_EdgeCase_ExactlyAtCloseTime()
    {
        var now = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM previous day
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.True(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_CrossingMidnight_BeforeMidnight()
    {
        var now = new DateTime(2024, 9, 12, 23, 0, 0); // 11:00 PM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_CrossingMidnight_AfterMidnight()
    {
        var now = new DateTime(2024, 9, 13, 1, 0, 0); // 1:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM previous day
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_WeekendScenario()
    {
        var now = new DateTime(2024, 9, 14, 12, 0, 0); // Saturday noon
        var nextOpen = new DateTime(2024, 9, 16, 7, 0, 0); // Monday 7:00 AM
        var nextClose = new DateTime(2024, 9, 17, 3, 0, 0); // Tuesday 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_SubscriptionExpiredDuringValidTime()
    {
        var now = new DateTime(2024, 9, 12, 14, 0, 0); // 2:00 PM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_SubscriptionStartedOutsideValidTime()
    {
        var now = new DateTime(2024, 9, 13, 5, 0, 0); // 5:00 AM
        var nextOpen = new DateTime(2024, 9, 13, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 14, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Theory]
    [InlineData(2024, 9, 12, 6, 59, 0, false, false, false)] // Just before open
    [InlineData(2024, 9, 12, 7, 1, 0, true, false, false)] // Just after open
    [InlineData(2024, 9, 13, 2, 59, 0, true, false, false)] // Just before close
    [InlineData(2024, 9, 13, 3, 1, 0, false, false, true)] // Just after close
    public void EvaluateSchedule_MinuteAccuracy(int year, int month, int day, int hour, int minute, int second,
        bool expectedIsWithinValidTimeRange, bool expectedHasMissedStart, bool expectedHasMissedStop)
    {
        var now = new DateTime(year, month, day, hour, minute, second);
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.Equal(expectedIsWithinValidTimeRange, result.IsWithinValidTimeRange);
        Assert.Equal(expectedHasMissedStart, result.HasMissedStart);
        Assert.Equal(expectedHasMissedStop, result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_MultiDayClosure_DuringClosure()
    {
        var now = new DateTime(2024, 12, 25, 12, 0, 0); // Christmas Day
        var nextOpen = new DateTime(2024, 12, 24, 7, 0, 0); // Last open before Christmas
        var nextClose = new DateTime(2024, 12, 27, 3, 0, 0); // First close after Christmas

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_MultiDayClosure_JustBeforeReopening()
    {
        var now = new DateTime(2024, 12, 26, 23, 59, 0); // Just before reopening
        var nextOpen = new DateTime(2024, 12, 27, 0, 0, 0); // Reopening after Christmas
        var nextClose = new DateTime(2024, 12, 27, 3, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_DaylightSavingsTransition()
    {
        // Assuming Thailand doesn't observe DST, but the system might be running in a location that does
        var now = new DateTime(2024, 3, 10, 2, 30, 0); // During "spring forward" transition
        var nextOpen = new DateTime(2024, 3, 10, 1, 0, 0);
        var nextClose = new DateTime(2024, 3, 10, 3, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_LeapYearFebruary29()
    {
        var now = new DateTime(2024, 2, 29, 12, 0, 0); // Leap day
        var nextOpen = new DateTime(2024, 2, 29, 7, 0, 0);
        var nextClose = new DateTime(2024, 3, 1, 3, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_YearEndTransition()
    {
        var now = new DateTime(2024, 12, 31, 23, 30, 0); // New Year's Eve
        var nextOpen = new DateTime(2024, 12, 31, 7, 0, 0);
        var nextClose = new DateTime(2025, 1, 1, 3, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_ExtendedTradingHours()
    {
        var now = new DateTime(2024, 9, 13, 4, 30, 0); // 4:30 AM, assuming extended hours
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // Regular open from previous day
        var nextClose = new DateTime(2024, 9, 13, 5, 0, 0); // Extended close time

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_SubscriptionExpiredJustBeforeClose()
    {
        var now = new DateTime(2024, 9, 13, 2, 59, 0); // 1 minute before close
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0);
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0);

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_WithinValidRange_SubscriptionNotRunning_OvernightScenario()
    {
        var now = new DateTime(2024, 9, 13, 1, 0, 0); // 1:00 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM previous day
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_OutsideValidRange_SubscriptionRunning()
    {
        var now = new DateTime(2024, 9, 13, 6, 0, 0); // 6:00 AM
        var nextOpen = new DateTime(2024, 9, 13, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 14, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_ExactlyAtOpenTime_SubscriptionNotRunning()
    {
        var now = new DateTime(2024, 9, 13, 7, 0, 0); // 7:00 AM
        var nextOpen = new DateTime(2024, 9, 13, 7, 0, 0); // 7:00 AM
        var nextClose = new DateTime(2024, 9, 14, 3, 0, 0); // 3:00 AM next day

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, false);

        Assert.True(result.IsWithinValidTimeRange);
        Assert.True(result.HasMissedStart);
        Assert.False(result.HasMissedStop);
    }

    [Fact]
    public void EvaluateSchedule_JustAfterClose_SubscriptionStillRunning()
    {
        var now = new DateTime(2024, 9, 13, 3, 1, 0); // 3:01 AM
        var nextOpen = new DateTime(2024, 9, 12, 7, 0, 0); // 7:00 AM previous day
        var nextClose = new DateTime(2024, 9, 13, 3, 0, 0); // 3:00 AM

        var result = ClientSubscriptionServiceHelper.EvaluateSchedule(now, nextOpen, nextClose, true);

        Assert.False(result.IsWithinValidTimeRange);
        Assert.False(result.HasMissedStart);
        Assert.True(result.HasMissedStop);
    }
}