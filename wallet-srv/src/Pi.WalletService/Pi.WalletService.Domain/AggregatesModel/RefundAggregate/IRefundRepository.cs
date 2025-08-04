namespace Pi.WalletService.Domain.AggregatesModel.RefundAggregate;

public interface IRefundRepository
{
    Task<List<RefundState>> Get(int pageNum, int pageSize, string? orderBy, string? orderDir, IQueryFilter<RefundState>? filters);
    Task<int> CountTransactions(IQueryFilter<RefundState>? filters);
}
