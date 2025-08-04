using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.TransactionIdAggregate;

public interface ITransactionIdRepository : IRepository<TransactionId>
{
    Task<TransactionId?> GetTransactionAsync(string referId, CancellationToken cancellationToken = default);
    Task<TransactionId> GetNextAsync(string prefix, DateOnly date, string referId, string customerCode, CancellationToken cancellationToken = default);
}