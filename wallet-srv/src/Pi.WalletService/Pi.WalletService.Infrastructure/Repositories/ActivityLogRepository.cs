using Pi.Common.SeedWork;
using Pi.WalletService.Domain.AggregatesModel.WalletAggregate;
namespace Pi.WalletService.Infrastructure.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly WalletDbContext _walletDbContext;

    public IUnitOfWork UnitOfWork => _walletDbContext;

    public ActivityLogRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;

    }

    public ActivityLog Create(ActivityLog activityLog)
    {
        return _walletDbContext.ActivityLogs.Add(activityLog).Entity;
    }
}
