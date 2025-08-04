namespace Pi.SetMarketDataRealTime.Application.Interfaces.Holiday;

public interface IHolidayApiQuery
{
    Task<bool> IsNotBusinessDays(DateTime? date = null);
}