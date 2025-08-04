using System.Linq.Expressions;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Application.Queries.Filters;

public class DepositTransactionFilters : EntityTransactionV2Filters, IQueryFilter<Transaction>
{
    public DateTime? PaymentReceivedDateTimeFrom { get; set; }
    public DateTime? PaymentReceivedDateTimeTo { get; set; }

    public List<Expression<Func<Transaction, bool>>> GetExpressions()
    {
        var result = new List<Expression<Func<Transaction, bool>>>
        {
            q => q.DepositEntrypoint != null && q.DepositEntrypoint.TransactionNo != null,
        };

        if (Channel != null) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.Channel == Channel);

        if (Status != null)
        {
            if (Status is Domain.AggregatesModel.TransactionAggregate.Status.Pending or Domain.AggregatesModel.TransactionAggregate.Status.Processing)
            {
                var upBackStates = EntityFactory.GetUpBackStates(Status.Value);
                var globalTransferStates = EntityFactory.GetGlobalTransferStates(Status.Value);
                var oddDepositStates = EntityFactory.GetOddDepositStates(Status.Value);
                var atsDepositStates = EntityFactory.GetAtsDepositStates(Status.Value);
                var qrDepositStates =
                    EntityFactory.GetQrDepositStates(Status.Value);
                var excludeEntryPointStates =
                    EntityFactory.GetDepositEntrypointStates(Domain.AggregatesModel.TransactionAggregate.Status.Success)
                        .Concat(EntityFactory.GetDepositEntrypointStates(Domain.AggregatesModel.TransactionAggregate.Status.Fail));
                result.Add(q =>
                    q.DepositEntrypoint != null
                    && !excludeEntryPointStates.Contains(q.DepositEntrypoint.CurrentState)
                    && (q.UpBack != null && upBackStates.Contains(q.UpBack.CurrentState)
                        || (q.GlobalTransfer != null && globalTransferStates.Contains(q.GlobalTransfer.CurrentState))
                        || (q.OddDeposit != null && oddDepositStates.Contains(q.OddDeposit.CurrentState))
                        || (q.AtsDeposit != null && atsDepositStates.Contains(q.AtsDeposit.CurrentState))
                        || (q.QrDeposit != null && qrDepositStates.Contains(q.QrDeposit.CurrentState)))
                );
            }
            else
            {
                var depositEntrypointStates = EntityFactory.GetDepositEntrypointStates(Status.Value);
                result.Add(q => q.DepositEntrypoint != null && depositEntrypointStates.Contains(q.DepositEntrypoint.CurrentState));
            }
        }

        if (Product is { Length: > 0 }) result.Add(q => q.DepositEntrypoint != null && Product.Contains(q.DepositEntrypoint.Product));

        if (!string.IsNullOrEmpty(BankName)) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.BankName != null && q.DepositEntrypoint.BankName.ToLower() == BankName.ToLower());

        if (!string.IsNullOrEmpty(BankAccountNo)) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.BankAccountNo != null && q.DepositEntrypoint.BankAccountNo == BankAccountNo);

        if (!string.IsNullOrEmpty(CustomerCode)) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.CustomerCode == CustomerCode);

        if (!string.IsNullOrEmpty(AccountCode)) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.AccountCode == AccountCode);

        if (!string.IsNullOrEmpty(TransactionNo)) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.TransactionNo == TransactionNo);

        if (!string.IsNullOrEmpty(BankCode)) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.BankCode == BankCode);

        if (EffectiveDateFrom != null || EffectiveDateTo != null)
        {
            var depositEntrypointStates =
                EntityFactory.GetDepositEntrypointStates(Domain.AggregatesModel.TransactionAggregate.Status.Success);
            result.Add(q => q.DepositEntrypoint != null && depositEntrypointStates.Contains(q.DepositEntrypoint.CurrentState));

            if (EffectiveDateFrom != null) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.UpdatedAt >= EffectiveDateFrom);

            if (EffectiveDateTo != null) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.UpdatedAt <= EffectiveDateTo);
        }

        if (CreatedAtFrom != null) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.CreatedAt >= CreatedAtFrom);

        if (CreatedAtTo != null) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.CreatedAt <= CreatedAtTo);

        if (UserId != null) result.Add(q => q.DepositEntrypoint != null && q.DepositEntrypoint.UserId == UserId);

        if (PaymentReceivedDateTimeFrom != null || PaymentReceivedDateTimeTo != null)
        {
            if (Channel is IntegrationEvents.AggregatesModel.Channel.QR or null)
            {
                var qrDepositStates = EntityFactory.GetQrDepositStates(Domain.AggregatesModel.TransactionAggregate.Status.Success);
                result.Add(q => q.QrDeposit != null && qrDepositStates.Contains(q.QrDeposit.CurrentState));

                if (PaymentReceivedDateTimeFrom != null) result.Add(q => q.QrDeposit != null && q.QrDeposit.PaymentReceivedDateTime >= PaymentReceivedDateTimeFrom);
                if (PaymentReceivedDateTimeTo != null) result.Add(q => q.QrDeposit != null && q.QrDeposit.PaymentReceivedDateTime <= PaymentReceivedDateTimeTo);

            }

            if (Channel is IntegrationEvents.AggregatesModel.Channel.ODD or null)
            {
                var oddDepositStates = EntityFactory.GetOddDepositStates(Domain.AggregatesModel.TransactionAggregate.Status.Success);
                result.Add(q => q.OddDeposit != null && oddDepositStates.Contains(q.OddDeposit.CurrentState));

                if (PaymentReceivedDateTimeFrom != null) result.Add(q => q.OddDeposit != null && q.OddDeposit.PaymentReceivedDateTime >= PaymentReceivedDateTimeFrom);
                if (PaymentReceivedDateTimeTo != null) result.Add(q => q.OddDeposit != null && q.OddDeposit.PaymentReceivedDateTime <= PaymentReceivedDateTimeTo);

            }
        }

        if (State != null)
        {
            result.Add(q =>
                (q.DepositEntrypoint != null && q.DepositEntrypoint.CurrentState == State)
                || (q.AtsDeposit != null && q.AtsDeposit.CurrentState == State)
                || (q.QrDeposit != null && q.QrDeposit.CurrentState == State)
                || (q.UpBack != null && q.UpBack.CurrentState == State)
                || (q.GlobalTransfer != null && q.GlobalTransfer.CurrentState == State)
                || (q.OddDeposit != null && q.OddDeposit.CurrentState == State)
                || (q.RefundInfo != null && q.RefundInfo.CurrentState == State)
            );
        }

        if (NotStates is { Length: > 0 })
        {
            result.Add(q =>
                (q.DepositEntrypoint == null || !NotStates.Contains(q.DepositEntrypoint.CurrentState))
                && (q.AtsDeposit == null || !NotStates.Contains(q.AtsDeposit.CurrentState))
                && (q.QrDeposit == null || !NotStates.Contains(q.QrDeposit.CurrentState))
                && (q.UpBack == null || !NotStates.Contains(q.UpBack.CurrentState))
                && (q.GlobalTransfer == null || !NotStates.Contains(q.GlobalTransfer.CurrentState))
                && (q.OddDeposit == null || !NotStates.Contains(q.OddDeposit.CurrentState))
                && (q.RefundInfo == null || !NotStates.Contains(q.RefundInfo.CurrentState))
            );
        }

        return result;
    }
}