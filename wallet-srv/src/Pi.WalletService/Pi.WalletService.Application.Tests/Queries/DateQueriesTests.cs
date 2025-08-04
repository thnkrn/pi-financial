using Moq;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

namespace Pi.WalletService.Application.Tests.Queries;

public class DateQueriesTests
{
    private readonly IDateQueries _dateQueries;

    public DateQueriesTests()
    {
        Mock<IHolidayRepository> mockHolidayRepository = new();
        var holidaysData = new List<Holiday>()
        {
            new (new DateOnly(2024, 12, 27), "Mock Friday Holiday"),
            new (new DateOnly(2024, 12, 30), "Mock Monday Holiday"),
            new (new DateOnly(2024, 12, 31), "Mock Tuesday Holiday"),
        };
        mockHolidayRepository.Setup(x => x.GetAll()).ReturnsAsync(holidaysData);
        _dateQueries = new DateQueries(mockHolidayRepository.Object);
    }

    [Fact]
    public async Task GetNextBusinessDay_When_TodayIsFriday_Should_ReturnsMonday()
    {
        // Arrange
        var current = new DateTime(2024, 5, 10);
        var expected = new DateTime(2024, 5, 13);

        // Act
        var result = await _dateQueries.GetNextBusinessDay(current);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetNextBusinessDay_When_TodayIsWednesday_Should_ReturnsThursday()
    {
        // Arrange
        var current = new DateTime(2024, 5, 15);
        var expected = new DateTime(2024, 5, 16);

        // Act
        var result = await _dateQueries.GetNextBusinessDay(current);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetNextBusinessDay_When_TodayIsThursdayBeforeNewYear_Should_ReturnsBusinessDayOfNextYear()
    {
        // Arrange
        var current = new DateTime(2024, 12, 26);
        var expected = new DateTime(2025, 1, 1);

        // Act
        var result = await _dateQueries.GetNextBusinessDay(current);

        // Assert
        Assert.Equal(expected, result);
    }
}