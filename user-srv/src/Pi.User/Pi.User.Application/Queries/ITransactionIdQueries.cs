using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;

namespace Pi.User.Application.Queries;

public interface ITransactionIdQueries
{
    Task<TransactionId> GetTransactionIdWithReferIdAsync(string referId, CancellationToken cancellationToken = default);
}