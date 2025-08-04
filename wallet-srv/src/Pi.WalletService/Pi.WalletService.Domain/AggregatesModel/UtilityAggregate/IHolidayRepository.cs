using Pi.Common.SeedWork;

namespace Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

public interface IHolidayRepository : IRepository<Holiday>
{
    Task<List<Holiday>> GetAll();
}