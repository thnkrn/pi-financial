using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Repositories;

public interface ICommonDataRepository
{
    Task<IList<FundCategory>> GetFundCategories(CancellationToken ct);
    Task<IList<ExtFundCategory>> GetExtFundCategories(CancellationToken ct);
    Task<BusinessCalendar> GetBusinessCalendar(CancellationToken ct);
    Task UpsertBusinessCalendar(IEnumerable<DateTime> businessHolidays, CancellationToken ct);
}
