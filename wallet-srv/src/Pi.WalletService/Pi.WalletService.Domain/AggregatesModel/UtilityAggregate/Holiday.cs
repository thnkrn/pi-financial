using Pi.Common.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

public class Holiday : IAggregateRoot
{
    public Holiday(DateOnly holidayDate, string? holidayName)
    {
        HolidayDate = holidayDate;
        HolidayName = holidayName;
    }

    public DateOnly HolidayDate { get; private set; }
    public string? HolidayName { get; private set; }
    public bool Valid { get; private set; } = true;
}