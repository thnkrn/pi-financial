using Pi.Common.SeedWork;

namespace Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate
{
    public interface ICustomerDataSyncHistoryRepository : IRepository<CustomerDataSyncHistory>
    {
        Task<CustomerDataSyncHistory?> GetByCorrelationIdAsync(Guid correlationId, CancellationToken cancellationToken = default);
        Task AddAsync(CustomerDataSyncHistory customerDataSyncHistory, CancellationToken cancellationToken = default);
        void Update(CustomerDataSyncHistory customerDataSyncHistory);
    }
}
