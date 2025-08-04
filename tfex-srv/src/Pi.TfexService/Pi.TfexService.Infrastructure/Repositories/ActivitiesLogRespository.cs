using Microsoft.EntityFrameworkCore;
using Pi.Common.SeedWork;
using Pi.TfexService.Domain.Models.ActivitiesLog;

namespace Pi.TfexService.Infrastructure.Repositories;

public class ActivitiesLogRepository(TfexDbContext dbContext) : IActivitiesLogRepository
{
    public IUnitOfWork UnitOfWork => dbContext;

    public async Task AddAsync(ActivitiesLog activitiesLog, CancellationToken cancellationToken = default)
    {
        await dbContext.ActivitiesLogs.AddAsync(activitiesLog, cancellationToken);
    }

    public async Task<ActivitiesLog?> Get(string orderNo, CancellationToken cancellationToken = default)
    {
        return await dbContext.ActivitiesLogs.AsNoTracking().FirstOrDefaultAsync(x => x.OrderNo == orderNo, cancellationToken);
    }
}