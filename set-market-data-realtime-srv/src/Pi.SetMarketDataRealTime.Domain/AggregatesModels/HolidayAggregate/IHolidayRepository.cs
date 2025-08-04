namespace Pi.SetMarketDataRealTime.Domain.AggregatesModels.HolidayAggregate;

public interface IHolidayRepository
{
    Task<bool> IsHoliday(long epoch);
}