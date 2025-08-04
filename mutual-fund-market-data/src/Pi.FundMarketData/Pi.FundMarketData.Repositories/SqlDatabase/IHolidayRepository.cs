namespace Pi.FundMarketData.Repositories.SqlDatabase;

public interface IHolidayRepository
{
    Task<IEnumerable<Holiday>> GetBusinessHolidays(CancellationToken ct);
}
