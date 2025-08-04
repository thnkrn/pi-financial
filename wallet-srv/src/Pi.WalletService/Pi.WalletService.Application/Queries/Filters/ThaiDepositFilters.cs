using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Queries.Filters;

public class ThaiDepositFilters : EntityTransactionFilters, IQueryFilter<ThaiDepositTransaction>
{
    public string? BankName { get; set; }
    public Product? Product { get; set; }
    public DateTime? PaymentReceivedFrom { get; set; }
    public DateTime? PaymentReceivedTo { get; set; }

    public List<Expression<Func<ThaiDepositTransaction, bool>>> GetExpressions()
    {

        var result = new List<Expression<Func<ThaiDepositTransaction, bool>>>()
        {
            q => q.DepositState.TransactionNo != null,
            q => q.DepositState.Product != IntegrationEvents.AggregatesModel.Product.GlobalEquities,
        };

        if (Channel != null) result.Add(q => q.DepositState.Channel == Channel);

        if (Status != null)
        {
            var depositStates = Status != TransactionStatus.Success ? EntityFactory.GetDepositStates((TransactionStatus)Status) : Array.Empty<string>();
            var cashDepositStates = EntityFactory.GetCashDepositStates((TransactionStatus)Status);
            result.Add(q => depositStates.Contains(q.DepositState.CurrentState) || (q.CashDepositState != null && cashDepositStates.Contains(q.CashDepositState.CurrentState)));
        }

        if (Product != null) result.Add(q => q.DepositState.Product == Product);

        if (!string.IsNullOrEmpty(State))
        {
            result.Add(q => q.DepositState.CurrentState == State || (q.CashDepositState != null && q.CashDepositState.CurrentState == State));
        }

        if (!string.IsNullOrEmpty(BankCode)) result.Add(q => q.DepositState.BankCode == BankCode);

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.DepositState.BankName != null && q.DepositState.BankName.ToLower() == BankName.ToLower());

        if (!string.IsNullOrEmpty(BankAccountNo))
        {
            result.Add(q => q.DepositState.BankAccountNo != null && q.DepositState.BankAccountNo.Replace("-", "") == BankAccountNo.Replace("-", ""));
        }

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.DepositState.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.DepositState.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.DepositState.TransactionNo == TransactionNo);

        if (PaymentReceivedFrom != null) result.Add(q => q.DepositState.PaymentReceivedDateTime >= PaymentReceivedFrom);

        if (PaymentReceivedTo != null) result.Add(q => q.DepositState.PaymentReceivedDateTime <= PaymentReceivedTo);

        if (EffectiveDateFrom != null || EffectiveDateTo != null)
        {
            var cashDepositStates = EntityFactory.GetCashDepositStates(TransactionStatus.Success);
            result.Add(q => (q.CashDepositState != null && cashDepositStates.Contains(q.CashDepositState.CurrentState)));
        }

        if (EffectiveDateFrom != null) result.Add(q => q.CashDepositState!.UpdatedAt >= EffectiveDateFrom);

        if (EffectiveDateTo != null) result.Add(q => q.CashDepositState!.UpdatedAt <= EffectiveDateTo);

        if (CreatedAtFrom != null) result.Add(q => q.DepositState.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.DepositState.CreatedAt <= CreatedAtTo);

        return result;
    }
}
