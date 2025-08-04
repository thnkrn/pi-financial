namespace Pi.SetMarketDataRealTime.Domain.AggregatesModels.HolidayAggregate;

public interface IHolidayApiRepository
{
    Task<bool> IsHoliday(string date);
}