using System.Linq.Expressions;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Domain.AggregatesModel.DepositAggregate;

public class DepositStateFilters : TransactionFilters<DepositState>
{
    public DateTime? PaymentReceivedFrom { get; set; }
    public DateTime? PaymentReceivedTo { get; set; }
    public string? BankName { get; set; }

    public override List<Expression<Func<DepositState, bool>>> GetExpressions()
    {
        var result = base.GetExpressions();

        if (PaymentReceivedFrom != null) result.Add(q => q.PaymentReceivedDateTime >= PaymentReceivedFrom);

        if (PaymentReceivedTo != null) result.Add(q => q.PaymentReceivedDateTime <= PaymentReceivedTo);

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.BankName != null && q.BankName.ToLower() == BankName.ToLower());

        return result;
    }
}
