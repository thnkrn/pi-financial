using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Pi.FundMarketData.Repositories.SqlDatabase;

public class HolidayRepository : IHolidayRepository
{
    private readonly IServiceScopeFactory _scopeFactory;
    public HolidayRepository(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<IEnumerable<Holiday>> GetBusinessHolidays(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var ctx = scope.ServiceProvider.GetRequiredService<CommonDbContext>();
        var result = await ctx.Holidays.ToListAsync(ct);

        return result;
    }
}
