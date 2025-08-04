using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using WithdrawState = Pi.WalletService.IntegrationEvents.Models.WithdrawState;

namespace Pi.WalletService.Application.Queries.Filters;

public class ThaiWithdrawFilters : EntityTransactionFilters, IQueryFilter<ThaiWithdrawTransaction>
{
    public Product? Product { get; set; }
    public string? UserId { get; set; }

    public List<Expression<Func<ThaiWithdrawTransaction, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<ThaiWithdrawTransaction, bool>>>
        {
            q => EntityFactory.GetProducts(ProductType.ThaiEquity).Contains(q.WithdrawState.Product),
            q => q.WithdrawState.TransactionNo != null,
            q => q.CashWithdrawState != null
        };

        if (Channel != null) result.Add(q => q.WithdrawState.Channel == Channel);

        if (Status != null)
        {
            var withdrawStates = Status != TransactionStatus.Success ? EntityFactory.GetWithdrawStates((TransactionStatus)Status) : Array.Empty<string>();
            var cashWithdrawStates = EntityFactory.GetCashWithdrawStates((TransactionStatus)Status);

            if (Status is TransactionStatus.Fail or TransactionStatus.Pending)
            {
                var excludeStates = new[]
                {
                    WithdrawState.GetName(() => WithdrawState.WithdrawalFailed),
                    WithdrawState.GetName(() => WithdrawState.WaitingForConfirmation)
                };
                withdrawStates = withdrawStates
                    .Where(q => !excludeStates.Contains(q))
                    .ToArray();
            }

            result.Add(q => withdrawStates.Contains(q.WithdrawState.CurrentState) || cashWithdrawStates.Contains(q.CashWithdrawState!.CurrentState));
        }

        if (Product != null) result.Add(q => q.WithdrawState.Product == Product);

        if (!string.IsNullOrEmpty(State))
        {
            result.Add(q => q.WithdrawState.CurrentState == State || (q.CashWithdrawState != null && q.CashWithdrawState.CurrentState == State));
        }

        if (!string.IsNullOrEmpty(BankCode)) result.Add(q => q.WithdrawState.BankCode == BankCode);

        if (!string.IsNullOrEmpty(BankAccountNo))
        {
            result.Add(q => q.WithdrawState.BankAccountNo != null && q.WithdrawState.BankAccountNo.Replace("-", "") == BankAccountNo.Replace("-", ""));
        }

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.WithdrawState.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.WithdrawState.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.WithdrawState.TransactionNo == TransactionNo);

        if (CreatedAtFrom != null) result.Add(q => q.WithdrawState.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.WithdrawState.CreatedAt <= CreatedAtTo);

        if (UserId != null) result.Add(q => q.WithdrawState.UserId == UserId);

        return result;
    }
}
