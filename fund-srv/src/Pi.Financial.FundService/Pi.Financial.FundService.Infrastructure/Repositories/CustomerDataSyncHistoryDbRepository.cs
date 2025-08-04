using Pi.Common.SeedWork;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate;

namespace Pi.Financial.FundService.Infrastructure.Repositories;

public class CustomerDataSyncHistoryDbRepository : ICustomerDataSyncHistoryRepository
{
    private readonly FundDbContext _fundDbContext;

    public IUnitOfWork UnitOfWork => _fundDbContext;
    public CustomerDataSyncHistoryDbRepository(FundDbContext fundDbContext)
    {
        _fundDbContext = fundDbContext;
    }

    public async Task AddAsync(CustomerDataSyncHistory customerDataSyncHistory, CancellationToken cancellationToken = default)
    {
        await _fundDbContext.CustomerDataSyncHistories.AddAsync(customerDataSyncHistory, cancellationToken);
    }
    public async Task<CustomerDataSyncHistory?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default)
    {
        return await _fundDbContext.CustomerDataSyncHistories.FindAsync(new object[] { correlationId }, cancellationToken);
    }
    public void Update(CustomerDataSyncHistory customerDataSyncHistory)
    {
        _fundDbContext.CustomerDataSyncHistories.Update(customerDataSyncHistory);
    }
}
