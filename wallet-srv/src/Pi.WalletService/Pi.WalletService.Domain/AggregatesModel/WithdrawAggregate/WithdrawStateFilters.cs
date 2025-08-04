using System.Linq.Expressions;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;

public class WithdrawStateFilters : TransactionFilters<WithdrawState>
{
    public DateTime? PaymentDisbursedDateTimeFrom { get; set; }
    public DateTime? PaymentDisbursedDateTimeTo { get; set; }

    public override List<Expression<Func<WithdrawState, bool>>> GetExpressions()
    {
        var result = base.GetExpressions();

        if (PaymentDisbursedDateTimeFrom != null) result.Add(q => q.PaymentDisbursedDateTime >= PaymentDisbursedDateTimeFrom);

        if (PaymentDisbursedDateTimeTo != null) result.Add(q => q.PaymentDisbursedDateTime <= PaymentDisbursedDateTimeTo);

        return result;
    }
}
