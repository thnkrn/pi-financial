using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using DepositMachineState = Pi.WalletService.IntegrationEvents.Models.DepositState;
using WithdrawState = Pi.WalletService.IntegrationEvents.Models.WithdrawState;

namespace Pi.WalletService.Application.Queries.Filters;

public class GlobalWithdrawFilters : EntityTransactionFilters, IQueryFilter<GlobalWithdrawTransaction>
{
    public List<Expression<Func<GlobalWithdrawTransaction, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<GlobalWithdrawTransaction, bool>>>()
        {
            q => q.WithdrawState.Product == Product.GlobalEquities,
            q => q.WithdrawState.TransactionNo != null,
            q => q.GlobalWalletTransferState != null,
        };

        if (Channel != null) result.Add(q => q.WithdrawState.Channel == Channel);

        if (Status != null)
        {
            var withdrawStates = Status != TransactionStatus.Success ? EntityFactory.GetWithdrawStates((TransactionStatus)Status) : Array.Empty<string>();
            var globalTransferStates = EntityFactory.GetGlobalStates((TransactionStatus)Status, TransactionType.Withdraw);

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

            result.Add(q => withdrawStates.Contains(q.WithdrawState.CurrentState) || globalTransferStates.Contains(q.GlobalWalletTransferState!.CurrentState));
        }

        if (!string.IsNullOrEmpty(State))
        {
            result.Add(q => q.WithdrawState.CurrentState == State || q.GlobalWalletTransferState!.CurrentState == State);
        }

        if (!string.IsNullOrEmpty(BankCode)) result.Add(q => q.WithdrawState.BankCode == BankCode);

        if (!string.IsNullOrEmpty(BankAccountNo))
        {
            result.Add(q => q.WithdrawState.BankAccountNo != null && q.WithdrawState.BankAccountNo.Replace("-", "") == BankAccountNo.Replace("-", ""));
        }

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.WithdrawState.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.WithdrawState.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.WithdrawState.TransactionNo == TransactionNo);

        if (EffectiveDateFrom != null) result.Add(q => q.WithdrawState.PaymentDisbursedDateTime >= EffectiveDateFrom);

        if (EffectiveDateTo != null) result.Add(q => q.WithdrawState.PaymentDisbursedDateTime <= EffectiveDateTo);

        if (CreatedAtFrom != null) result.Add(q => q.WithdrawState.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.WithdrawState.CreatedAt <= CreatedAtTo);

        return result;
    }
}
