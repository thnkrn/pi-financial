namespace Pi.SetMarketData.Domain.AggregatesModels.HolidayAggregate;

public interface IHolidayRepository
{
    Task<bool> IsHoliday(long epoch);
}