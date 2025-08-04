using Pi.Common.SeedWork;
namespace Pi.WalletService.Domain.AggregatesModel.WalletAggregate;

public interface IActivityLogRepository : IRepository<ActivityLog>
{
    ActivityLog Create(ActivityLog activityLog);
}
