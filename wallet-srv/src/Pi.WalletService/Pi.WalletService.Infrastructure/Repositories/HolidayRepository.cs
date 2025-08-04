using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;

namespace Pi.WalletService.Infrastructure.Repositories;

public class HolidayRepository : IHolidayRepository
{
    private readonly WalletDbContext _walletDbContext;
    public IUnitOfWork UnitOfWork => _walletDbContext;

    public HolidayRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<List<Holiday>> GetAll()
    {
        return await _walletDbContext.Holidays.Where(x => x.Valid).AsNoTracking().ToListAsync();
    }
}