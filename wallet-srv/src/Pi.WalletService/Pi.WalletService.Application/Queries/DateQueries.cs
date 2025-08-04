using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

namespace Pi.WalletService.Application.Queries;

public class DateQueries : IDateQueries
{
    private readonly IHolidayRepository _holidayRepository;

    public DateQueries(IHolidayRepository holidayRepository)
    {
        _holidayRepository = holidayRepository;
    }

    public async Task<DateTime> GetNextBusinessDay(DateTime currentDate)
    {
        var holidays = await _holidayRepository.GetAll();
        var nextDay = currentDate.AddDays(1);
        var checkForHoliday = true;

        // return next business day if it's not weekend or holiday
        while (checkForHoliday)
        {
            if (nextDay.DayOfWeek == DayOfWeek.Saturday)
            {
                nextDay = nextDay.AddDays(2);
            }
            else if (nextDay.DayOfWeek == DayOfWeek.Sunday)
            {
                nextDay = nextDay.AddDays(1);
            }
            else if (holidays.Any(x => x.HolidayDate == DateOnly.FromDateTime(nextDay)))
            {
                nextDay = nextDay.AddDays(1);
            }
            else
            {
                checkForHoliday = false;
            }
        }

        return nextDay;
    }
}