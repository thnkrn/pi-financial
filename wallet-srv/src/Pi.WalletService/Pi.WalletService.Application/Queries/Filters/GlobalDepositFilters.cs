using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.Models;
using DepositMachineState = Pi.WalletService.IntegrationEvents.Models.DepositState;

namespace Pi.WalletService.Application.Queries.Filters;

public class GlobalDepositFilters : EntityTransactionFilters, IQueryFilter<GlobalDepositTransaction>
{
    public DateTime? PaymentReceivedFrom { get; set; }
    public DateTime? PaymentReceivedTo { get; set; }
    public string? BankName { get; set; }

    private static string GetDepositStateName(string state)
    {
        if (string.Equals(state, GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.RefundFailed), StringComparison.CurrentCultureIgnoreCase))
        {
            return DepositMachineState.GetName(() => DepositMachineState.DepositRefundFailed);
        }

        if (string.Equals(state, GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.RefundSucceed), StringComparison.CurrentCultureIgnoreCase))
        {
            return DepositMachineState.GetName(() => DepositMachineState.DepositRefundSucceed);
        }

        if (string.Equals(state, GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.Refunding), StringComparison.CurrentCultureIgnoreCase))
        {
            return DepositMachineState.GetName(() => DepositMachineState.DepositRefunding);
        }

        return state;
    }

    public List<Expression<Func<GlobalDepositTransaction, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<GlobalDepositTransaction, bool>>>()
        {
            q => q.DepositState.Product == Product.GlobalEquities,
            q => q.DepositState.TransactionNo != null,
            q => q.GlobalWalletTransferState != null,
        };

        if (Channel != null) result.Add(q => q.DepositState.Channel == Channel);

        if (Status != null)
        {
            var depositStates = Status != TransactionStatus.Success ? EntityFactory.GetDepositStates((TransactionStatus)Status) : Array.Empty<string>();
            var globalTransferStates = EntityFactory.GetGlobalStates((TransactionStatus)Status, TransactionType.Deposit);
            result.Add(q => depositStates.Contains(q.DepositState.CurrentState) || globalTransferStates.Contains(q.GlobalWalletTransferState!.CurrentState));
        }

        if (!string.IsNullOrEmpty(State))
        {
            var depositState = GetDepositStateName(State);
            result.Add(q => q.DepositState.CurrentState == depositState || q.GlobalWalletTransferState!.CurrentState == State);
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

        if (EffectiveDateFrom != null) result.Add(q => q.GlobalWalletTransferState!.TransferCompleteTime >= EffectiveDateFrom);

        if (EffectiveDateTo != null) result.Add(q => q.GlobalWalletTransferState!.TransferCompleteTime <= EffectiveDateTo);

        if (PaymentReceivedFrom != null) result.Add(q => q.DepositState!.PaymentReceivedDateTime >= PaymentReceivedFrom);

        if (PaymentReceivedTo != null) result.Add(q => q.DepositState!.PaymentReceivedDateTime <= PaymentReceivedTo);

        if (CreatedAtFrom != null) result.Add(q => q.DepositState.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.DepositState.CreatedAt <= CreatedAtTo);

        return result;
    }
}
