using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Pi.SetMarketDataRealTime.Application.Interfaces.Holiday;
using Pi.SetMarketDataRealTime.Application.Queries.Holiday;
using Pi.SetMarketDataRealTime.Domain.AggregatesModels.HolidayAggregate;
using Xunit.Abstractions;

namespace Pi.SetMarketDataRealTime.Application.Tests.Queries;

public class HolidayApiQueryTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IHolidayApiRepository> _holidayRepositoryMock = new();
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock = new();
    private readonly ILogger<HolidayApiQuery> _loggerMock = new Logger<HolidayApiQuery>(new NullLoggerFactory());

    /// <summary>
    /// 
    /// </summary>
    /// <param name="testOutputHelper"></param>
    public HolidayApiQueryTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task IsNotBusinessDays_ReturnsFalse_WhenNotWeekendOrHoliday()
    {
        // Arrange
        _holidayRepositoryMock.Setup(x => x.IsHoliday(It.IsAny<string>())).ReturnsAsync(false);
        var holidayQuery = new HolidayApiQuery(_holidayRepositoryMock.Object, _dateTimeProviderMock.Object, _loggerMock);

        // Act
        var result = await holidayQuery.IsNotBusinessDays();
        _testOutputHelper.WriteLine(result.ToString());
        
        // Assert
        Assert.False(result);
    }
}