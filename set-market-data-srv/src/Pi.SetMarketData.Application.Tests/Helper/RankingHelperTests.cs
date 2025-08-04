using Pi.SetMarketData.Application.Helper;
using Pi.SetMarketData.Application.Interfaces.Holiday;

namespace Pi.SetMarketData.Application.Tests.Helper;

public class RankingHelperTests
{
    private readonly Mock<IHolidayApiQuery> _holidayApiQuery;
    public RankingHelperTests()
    {
        _holidayApiQuery = new Mock<IHolidayApiQuery>();
    }
    [Fact]
    public async Task CalculateRankingStartDate_OnWeekday_ReturnsCorrectDate()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 4, 10, 0, 0, DateTimeKind.Utc); // Friday
        var marketStartTimeBangkokTimeZone = "09:30:00";
        _holidayApiQuery.Setup(query => query.IsNotBusinessDays(currentUtcDate)).ReturnsAsync(true);

        // Act
        var result = await RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTimeBangkokTimeZone, _holidayApiQuery.Object);

        // Assert
        var expectedDate = new DateTime(2025, 4, 4, 2, 30, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public async Task CalculateRankingStartDate_OnWeekday_ButBeforeMarketStartTime_ReturnsPreviousDay()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 4, 8, 0, 0, DateTimeKind.Utc); // Friday
        var marketStartTimeBangkokTimeZone = "20:30:00";
        _holidayApiQuery.Setup(query => query.IsNotBusinessDays(currentUtcDate)).ReturnsAsync(true);

        // Act
        var result = await RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTimeBangkokTimeZone, _holidayApiQuery.Object);

        // Assert
        var expectedDate = new DateTime(2025, 4, 3, 13, 30, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public async Task CalculateRankingStartDate_OnMonday_ReturnsFridayWithCorrectTime()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 7, 0, 0, 0, DateTimeKind.Utc); // Monday
        var marketStartTime = "09:30:00";
        _holidayApiQuery.Setup(query => query.IsNotBusinessDays(currentUtcDate)).ReturnsAsync(true);

        // Act
        var result = await RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTime, _holidayApiQuery.Object);

        // Assert
        var expectedDate = new DateTime(2025, 4, 4, 2, 30, 0, DateTimeKind.Utc); // Friday

        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public async Task CalculateRankingStartDate_WithDifferentTimes_ReturnsCorrectTimeAdjusted()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 3, 0, 0, 0, DateTimeKind.Utc); // Thursday
        var marketStartTime = "16:45:00";
        _holidayApiQuery.Setup(query => query.IsNotBusinessDays(currentUtcDate)).ReturnsAsync(true);

        // Act
        var result = await RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTime, _holidayApiQuery.Object);

        // Assert
        var expectedDate = new DateTime(2025, 4, 2, 09, 45, 0, DateTimeKind.Utc);

        Assert.Equal(expectedDate, result);
    }

    [Fact]
    public async Task CalculateRankingStartDate_ShouldNotUseHolidayAsRankingStartDate()
    {
        // Arrange
        var currentUtcDate = new DateTime(2025, 4, 3, 2, 30, 0, DateTimeKind.Utc); // Thursday
        var marketStartTime = "08:30:00";

        // Set up mock to return false for current date (is a holiday)
        _holidayApiQuery.Setup(query => query.IsNotBusinessDays(currentUtcDate))
            .ReturnsAsync(false);

        // Set up mock to return true for current date (is a business days)
        _holidayApiQuery.Setup(query => query.IsNotBusinessDays(It.Is<DateTime>(d => 
            d != currentUtcDate && d != currentUtcDate)))
            .ReturnsAsync(true);

        // Act
        var result = await RankingHelper.CalculateRankingStartDate(currentUtcDate, marketStartTime, _holidayApiQuery.Object);

        // Assert
        var expectedDate = new DateTime(2025, 4, 2, 1, 30, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDate, result);
    }
}