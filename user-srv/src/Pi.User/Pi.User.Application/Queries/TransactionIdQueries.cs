using Pi.User.Domain.AggregatesModel.TransactionIdAggregate;

namespace Pi.User.Application.Queries;

public class TransactionIdQueries : ITransactionIdQueries
{
    private readonly ITransactionIdRepository _transactionIdRepository;

    public TransactionIdQueries(ITransactionIdRepository transactionIdRepository)
    {
        _transactionIdRepository = transactionIdRepository;
    }

    public async Task<TransactionId> GetTransactionIdWithReferIdAsync(string referId, CancellationToken cancellationToken = default)
    {
        var transaction = await _transactionIdRepository.GetTransactionAsync(referId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidDataException($"Transaction not found. ReferId: {referId}");
        }

        return transaction;
    }
}