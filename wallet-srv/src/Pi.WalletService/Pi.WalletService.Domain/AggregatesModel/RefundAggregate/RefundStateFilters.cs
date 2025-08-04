using System.Linq.Expressions;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.RefundAggregate;

public class RefundStateFilters : TransactionFilters<RefundState>
{
    public string? DepositTransactionNo { get; set; }
    public DateTime? RefundAtFrom { get; set; }
    public DateTime? RefundAtTo { get; set; }

    public override List<Expression<Func<RefundState, bool>>> DefaultExpressions()
    {
        return new List<Expression<Func<RefundState, bool>>>();
    }

    public override List<Expression<Func<RefundState, bool>>> GetExpressions()
    {
        var result = base.GetExpressions();

        if (DepositTransactionNo != null) result.Add(q => q.DepositTransactionNo != null && q.DepositTransactionNo.ToLower() == DepositTransactionNo.ToLower());

        if (RefundAtFrom != null) result.Add(q => q.RefundedAt >= RefundAtFrom);

        if (RefundAtTo != null) result.Add(q => q.RefundedAt <= RefundAtTo);

        return result;
    }
}
