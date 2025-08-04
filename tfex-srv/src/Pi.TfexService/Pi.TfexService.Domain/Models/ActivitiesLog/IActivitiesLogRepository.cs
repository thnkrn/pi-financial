using Pi.Common.SeedWork;

namespace Pi.TfexService.Domain.Models.ActivitiesLog;

public interface IActivitiesLogRepository : IRepository<ActivitiesLog>
{
    Task AddAsync(ActivitiesLog activitiesLog, CancellationToken cancellationToken = default);
    Task<ActivitiesLog?> Get(string orderNo, CancellationToken cancellationToken = default);
}
