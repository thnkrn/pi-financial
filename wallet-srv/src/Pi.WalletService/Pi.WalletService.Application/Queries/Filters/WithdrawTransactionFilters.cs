using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Application.Queries.Filters;

public class WithdrawTransactionFilters : EntityTransactionV2Filters, IQueryFilter<Transaction>
{
    public DateTime? PaymentDisbursedDateTimeFrom { get; set; }
    public DateTime? PaymentDisbursedDateTimeTo { get; set; }

    public List<Expression<Func<Transaction, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<Transaction, bool>>>
        {
            q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.TransactionNo != null,
        };

        if (Channel != null) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.Channel == Channel);

        if (Status != null)
        {
            if (Status is Domain.AggregatesModel.TransactionAggregate.Status.Pending or Domain.AggregatesModel.TransactionAggregate.Status.Processing)
            {
                var withdrawEntrypointStates = EntityFactory.GetWithdrawEntrypointStates(Status.Value);
                var upBackStates = EntityFactory.GetUpBackStates(Status.Value);
                var globalTransferStates = EntityFactory.GetGlobalTransferStates(Status.Value);
                var oddWithdrawStates = EntityFactory.GetOddWithdrawStates(Status.Value);
                var atsWithdrawStates = EntityFactory.GetAtsDepositStates(Status.Value);
                var recoveryStates = EntityFactory.GetRecoveryStates(Status.Value);
                var excludeEntryPointStates =
                    EntityFactory.GetWithdrawEntrypointStates(Domain.AggregatesModel.TransactionAggregate.Status.Success)
                        .Concat(EntityFactory.GetWithdrawEntrypointStates(Domain.AggregatesModel.TransactionAggregate.Status.Fail));
                result.Add(q =>
                    q.WithdrawEntrypoint != null
                    && !excludeEntryPointStates.Contains(q.WithdrawEntrypoint.CurrentState)
                    && ((q.UpBack != null && upBackStates.Contains(q.UpBack.CurrentState))
                        || withdrawEntrypointStates.Contains(q.WithdrawEntrypoint.CurrentState)
                        || (q.GlobalTransfer != null && globalTransferStates.Contains(q.GlobalTransfer.CurrentState))
                        || (q.Recovery != null && recoveryStates.Contains(q.Recovery.CurrentState))
                        || (q.OddWithdraw != null && oddWithdrawStates.Contains(q.OddWithdraw.CurrentState))
                        || (q.AtsWithdraw != null && atsWithdrawStates.Contains(q.AtsWithdraw.CurrentState)))
                );
            }
            else
            {
                var withdrawEntrypointStates = EntityFactory.GetWithdrawEntrypointStates(Status.Value);
                result.Add(q => q.WithdrawEntrypoint != null && withdrawEntrypointStates.Contains(q.WithdrawEntrypoint.CurrentState));
            }
        }

        if (Product is { Length: > 0 }) result.Add(q => q.WithdrawEntrypoint != null && Product.Contains(q.WithdrawEntrypoint.Product));

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.BankName != null && q.WithdrawEntrypoint.BankName.ToLower() == BankName.ToLower());

        if (!string.IsNullOrEmpty(BankCode)) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.BankCode == BankCode);

        if (!string.IsNullOrEmpty(BankAccountNo)) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.BankAccountNo != null && q.WithdrawEntrypoint.BankAccountNo == BankAccountNo);

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.TransactionNo == TransactionNo);


        if (EffectiveDateFrom != null || EffectiveDateTo != null)
        {
            var withdrawEntrypointStates =
                EntityFactory.GetWithdrawEntrypointStates(Domain.AggregatesModel.TransactionAggregate.Status.Success);
            result.Add(q => q.WithdrawEntrypoint != null && withdrawEntrypointStates.Contains(q.WithdrawEntrypoint.CurrentState));

            if (EffectiveDateFrom != null) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.UpdatedAt >= EffectiveDateFrom);

            if (EffectiveDateTo != null) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.UpdatedAt <= EffectiveDateTo);
        }

        if (CreatedAtFrom != null) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.CreatedAt <= CreatedAtTo);

        if (UserId != null) result.Add(q => q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.UserId == UserId);

        if (PaymentDisbursedDateTimeFrom != null || PaymentDisbursedDateTimeTo != null)
        {
            if (Channel == null || Channel == IntegrationEvents.AggregatesModel.Channel.OnlineViaKKP)
            {
                var oddWithdrawStates = EntityFactory.GetOddWithdrawStates(Domain.AggregatesModel.TransactionAggregate.Status.Success);
                result.Add(q => q.OddWithdraw != null && oddWithdrawStates.Contains(q.OddWithdraw.CurrentState));

                if (PaymentDisbursedDateTimeFrom != null) result.Add(q => q.OddWithdraw != null && q.OddWithdraw.PaymentDisbursedDateTime >= PaymentDisbursedDateTimeFrom);

                if (PaymentDisbursedDateTimeTo != null) result.Add(q => q.OddWithdraw != null && q.OddWithdraw.PaymentDisbursedDateTime <= PaymentDisbursedDateTimeTo);
            }

            if (Channel == null || Channel == IntegrationEvents.AggregatesModel.Channel.ATS)
            {
                var atsWithdrawStates = EntityFactory.GetAtsWithdrawStates(Domain.AggregatesModel.TransactionAggregate.Status.Success);
                result.Add(q => q.AtsWithdraw != null && atsWithdrawStates.Contains(q.AtsWithdraw.CurrentState));

                if (PaymentDisbursedDateTimeFrom != null) result.Add(q => q.AtsWithdraw != null && q.AtsWithdraw.PaymentDisbursedDateTime >= PaymentDisbursedDateTimeFrom);

                if (PaymentDisbursedDateTimeTo != null) result.Add(q => q.AtsWithdraw != null && q.AtsWithdraw.PaymentDisbursedDateTime <= PaymentDisbursedDateTimeTo);
            }

        }

        if (State != null)
        {
            result.Add(q =>
                (q.WithdrawEntrypoint != null && q.WithdrawEntrypoint.CurrentState == State)
                || (q.OddWithdraw != null && q.OddWithdraw.CurrentState == State)
                || (q.AtsWithdraw != null && q.AtsWithdraw.CurrentState == State)
                || (q.UpBack != null && q.UpBack.CurrentState == State)
                || (q.GlobalTransfer != null && q.GlobalTransfer.CurrentState == State)
                || (q.Recovery != null && q.Recovery.CurrentState == State)
            );
        }

        if (NotStates is { Length: > 0 })
        {
            result.Add(q =>
                (q.WithdrawEntrypoint == null || !NotStates.Contains(q.WithdrawEntrypoint.CurrentState))
                && (q.OddWithdraw == null || !NotStates.Contains(q.OddWithdraw.CurrentState))
                && (q.AtsWithdraw == null || !NotStates.Contains(q.AtsWithdraw.CurrentState))
                && (q.UpBack == null || !NotStates.Contains(q.UpBack.CurrentState))
                && (q.GlobalTransfer == null || !NotStates.Contains(q.GlobalTransfer.CurrentState))
                && (q.Recovery == null || !NotStates.Contains(q.Recovery.CurrentState))
            );
        }

        return result;
    }
}