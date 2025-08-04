using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;

namespace Pi.WalletService.Application.Queries.Filters;

public class CashDepositFilters : EntityTransactionFilters, IQueryFilter<CashDepositState>
{
    public string? BankName { get; set; }
    public Product? Product { get; set; }
    public DateTime? PaymentReceivedFrom { get; set; }
    public DateTime? PaymentReceivedTo { get; set; }
    public string? UserId { get; set; }

    public List<Expression<Func<CashDepositState, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<CashDepositState, bool>>>()
        {
            q => q.TransactionNo != null,
        };

        if (Channel != null) result.Add(q => q.Channel == Channel);

        if (Status != null)
        {
            var depositStates = EntityFactory.GetCashDepositStates((TransactionStatus)Status);
            result.Add(q => depositStates.Contains(q.CurrentState));
        }

        if (Product != null) result.Add(q => q.Product == Product);

        if (!string.IsNullOrEmpty(State)) result.Add(q => q.CurrentState == State);

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.BankName != null && q.BankName.ToLower() == BankName.ToLower());

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.TransactionNo == TransactionNo);

        if (PaymentReceivedFrom != null) result.Add(q => q.PaymentReceivedDateTime >= PaymentReceivedFrom);

        if (PaymentReceivedTo != null) result.Add(q => q.PaymentReceivedDateTime <= PaymentReceivedTo);

        if (EffectiveDateFrom != null || EffectiveDateTo != null)
        {
            var depositStates = EntityFactory.GetCashDepositStates(TransactionStatus.Success);
            result.Add(q => depositStates.Contains(q.CurrentState));
        }

        if (EffectiveDateFrom != null) result.Add(q => q.UpdatedAt >= EffectiveDateFrom);

        if (EffectiveDateTo != null) result.Add(q => q.UpdatedAt <= EffectiveDateTo);

        if (CreatedAtFrom != null) result.Add(q => q.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.CreatedAt <= CreatedAtTo);

        if (UserId != null) result.Add(q => q.UserId == UserId);

        return result;
    }
}
