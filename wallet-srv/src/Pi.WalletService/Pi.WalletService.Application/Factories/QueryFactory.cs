using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositMachineState = Pi.WalletService.IntegrationEvents.Models.CashDepositState;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;
using DepositMachineState = Pi.WalletService.IntegrationEvents.Models.DepositState;
using CashWithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.CashWithdrawState;
using CashWithdrawState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashWithdrawState;
using DepositState = Pi.WalletService.Domain.AggregatesModel.DepositAggregate.DepositState;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;
using RefundMachineState = Pi.WalletService.IntegrationEvents.Models.RefundState;
using RefundState = Pi.WalletService.Domain.AggregatesModel.RefundAggregate.RefundState;
using WithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.WithdrawState;
using WithdrawState = Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate.WithdrawState;
using DepositEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.DepositEntrypointState;
using WithdrawEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.WithdrawEntrypointState;
using GlobalTransferMachineState = Pi.WalletService.IntegrationEvents.Models.GlobalTransferState;
using QrDepositMachineState = Pi.WalletService.IntegrationEvents.Models.QrDepositState;
using OddDepositMachineState = Pi.WalletService.IntegrationEvents.Models.OddDepositState;
using AtsDepositMachineState = Pi.WalletService.IntegrationEvents.Models.AtsDepositState;
using OddWithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.OddWithdrawState;
using UpBackMachineState = Pi.WalletService.IntegrationEvents.Models.UpBackState;

namespace Pi.WalletService.Application.Factories;

public static class QueryFactory
{
    public static Transaction NewTransaction(DepositState depositState)
    {
        var status = depositState.CurrentState != null
            ? GetTransactionStatus(depositState.CurrentState, TransactionType.Deposit, depositState.Product)
            : TransactionStatus.Pending;

        return new Transaction(
            depositState.CorrelationId,
            depositState.UserId,
            depositState.AccountCode,
            depositState.CustomerCode,
            depositState.CustomerName,
            depositState.CurrentState,
            status.ToString(),
            depositState.TransactionNo,
            TransactionType.Deposit,
            depositState.RequestedAmount,
            Currency.THB,
            depositState.QrValue,
            depositState.DepositQrGenerateDateTime,
            depositState.PaymentReceivedAmount,
            depositState.PaymentReceivedDateTime,
            null,
            depositState.Channel,
            depositState.Product,
            depositState.BankAccountNo,
            depositState.BankAccountName,
            depositState.BankName,
            depositState.BankFee,
            depositState.CreatedAt.ToUniversalTime(),
            null
        );
    }

    public static Transaction NewTransaction(CashWithdrawState withdrawState)
    {
        var status = withdrawState.CurrentState != null
            ? GetTransactionStatus(withdrawState.CurrentState, TransactionType.Withdraw, withdrawState.Product)
            : TransactionStatus.Pending;

        return new Transaction(
            withdrawState.CorrelationId,
            withdrawState.UserId,
            withdrawState.AccountCode,
            withdrawState.CustomerCode,
            null,
            withdrawState.CurrentState,
            status.ToString(),
            withdrawState.TransactionNo,
            TransactionType.Withdraw,
            withdrawState.RequestedAmount,
            Currency.THB,
            null,
            null,
            null,
            null,
            null,
            withdrawState.Channel,
            withdrawState.Product,
            withdrawState.BankAccountNo,
            null,
            withdrawState.BankName,
            withdrawState.BankFee,
            withdrawState.CreatedAt.ToUniversalTime(),
            null
        );
    }

    public static DepositTransaction NewDepositTransaction(CashDepositState cashDepositState)
    {
        var status = cashDepositState.CurrentState != null
            ? GetTransactionStatus(cashDepositState.CurrentState, TransactionType.Deposit, cashDepositState.Product)
            : TransactionStatus.Pending;

        DateTime? effectiveDate = null;
        if (cashDepositState is { CurrentState: not null } &&
            GetTransactionStatus(cashDepositState.CurrentState, TransactionType.Deposit, cashDepositState.Product) == TransactionStatus.Success)
        {
            effectiveDate = cashDepositState.UpdatedAt;
        }

        return new DepositTransaction
        {
            Id = cashDepositState.CorrelationId,
            UserId = cashDepositState.UserId,
            Status = status,
            TransactionNo = cashDepositState.TransactionNo,
            AccountCode = cashDepositState.AccountCode,
            CustomerCode = cashDepositState.CustomerCode,
            Product = cashDepositState.Product,
            CurrentState = cashDepositState.CurrentState,
            Channel = cashDepositState.Channel,
            CreatedAt = cashDepositState.CreatedAt,
            FailedReason = cashDepositState.FailedReason,
            Purpose = cashDepositState.Purpose,
            RequestedAmount = cashDepositState.RequestedAmount,
            PaymentReceivedDateTime = cashDepositState.PaymentReceivedDateTime,
            BankName = cashDepositState.BankName,
            TransactionType = TransactionType.Deposit,
            Currency = Currency.THB,
            Amount = cashDepositState.RequestedAmount,
            EffectiveDateTime = effectiveDate,
            // Unsupported fields
            ReceivedAmount = null,
            PaymentReceivedAmount = null,
            CustomerName = null,
            BankAccountName = null,
            QrGenerateDateTime = null,
            QrExpiredTime = null,
            QrCodeExpiredTimeInMinute = 0,
            QrTransactionNo = null,
            QrValue = null,
            QrTransactionRef = null,
            BankFee = null,
            BankCode = null,
            BankAccountNo = null,
        };
    }

    public static DepositTransaction NewDepositTransaction(DepositState depositState, DateTime? effectiveDatetime)
    {
        var status = depositState.CurrentState != null
            ? GetTransactionStatus(depositState.CurrentState, TransactionType.Deposit, depositState.Product)
            : TransactionStatus.Pending;

        return new DepositTransaction
        {
            Purpose = depositState.Purpose,
            RequestedAmount = depositState.RequestedAmount,
            ReceivedAmount = depositState.PaymentReceivedAmount,
            PaymentReceivedDateTime = depositState.PaymentReceivedDateTime,
            PaymentReceivedAmount = depositState.PaymentReceivedAmount,
            CustomerName = depositState.CustomerName,
            BankAccountName = depositState.BankAccountName,
            Amount = depositState.PaymentReceivedAmount ?? depositState.RequestedAmount,
            QrGenerateDateTime = depositState.DepositQrGenerateDateTime,
            QrExpiredTime = depositState.DepositQrGenerateDateTime != null
                ? depositState.DepositQrGenerateDateTime!.Value.AddMinutes(depositState.QrCodeExpiredTimeInMinute)
                    .ToUniversalTime()
                : null,
            QrCodeExpiredTimeInMinute = depositState.QrCodeExpiredTimeInMinute,
            QrTransactionNo = depositState.QrTransactionNo,
            QrValue = depositState.QrValue,
            QrTransactionRef = depositState.QrTransactionRef,
            Id = depositState.CorrelationId,
            UserId = depositState.UserId,
            TransactionNo = depositState.TransactionNo,
            AccountCode = depositState.AccountCode,
            CustomerCode = depositState.CustomerCode,
            Product = depositState.Product,
            TransactionType = TransactionType.Deposit,
            CurrentState = depositState.CurrentState,
            BankFee = depositState.BankFee,
            BankName = depositState.BankName,
            BankCode = depositState.BankCode,
            BankAccountNo = depositState.BankAccountNo,
            Currency = Currency.THB,
            Channel = depositState.Channel,
            CreatedAt = depositState.CreatedAt,
            FailedReason = depositState.FailedReason,
            Status = status,
            EffectiveDateTime = effectiveDatetime
        };
    }

    public static DepositTransaction NewDepositTransaction(DepositState depositState)
    {
        return NewDepositTransaction(depositState, depositState.PaymentReceivedDateTime);
    }

    public static DepositTransaction NewDepositTransaction(GlobalDepositTransaction globalDepositState)
    {
        if (globalDepositState.GlobalWalletTransferState?.CurrentState != GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositProcessing)
            && globalDepositState.GlobalWalletTransferState?.CurrentState != GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed))
        {
            globalDepositState.DepositState.CurrentState = globalDepositState.GlobalWalletTransferState?.CurrentState;
            globalDepositState.DepositState.FailedReason = globalDepositState.GlobalWalletTransferState?.FailedReason;
        }

        globalDepositState.DepositState.CreatedAt = globalDepositState.GlobalWalletTransferState?.CreatedAt ?? globalDepositState.DepositState.CreatedAt;

        return NewDepositTransaction(globalDepositState.DepositState, globalDepositState.GlobalWalletTransferState?.TransferCompleteTime);
    }

    public static DepositTransaction NewDepositTransaction(ThaiDepositTransaction thaiDepositTransaction)
    {
        if (thaiDepositTransaction.DepositState.CurrentState == DepositMachineState.GetName(() => DepositMachineState.DepositCompleted))
        {
            thaiDepositTransaction.DepositState.CurrentState = thaiDepositTransaction.CashDepositState?.CurrentState;
            thaiDepositTransaction.DepositState.FailedReason = thaiDepositTransaction.CashDepositState?.FailedReason;
        }

        DateTime? effectiveDate = null;
        if (thaiDepositTransaction.CashDepositState != null
            && thaiDepositTransaction.CashDepositState?.CurrentState != null
            && GetTransactionStatus(thaiDepositTransaction.CashDepositState.CurrentState, TransactionType.Deposit, thaiDepositTransaction.CashDepositState.Product) == TransactionStatus.Success
        )
        {
            effectiveDate = thaiDepositTransaction.CashDepositState.UpdatedAt;
        }

        return NewDepositTransaction(thaiDepositTransaction.DepositState, effectiveDate);
    }

    public static WithdrawTransaction NewWithdrawTransaction(WithdrawState withdrawState, decimal? requestedAmount = null)
    {
        var status = withdrawState.CurrentState != null
            ? GetTransactionStatus(withdrawState.CurrentState, TransactionType.Withdraw, withdrawState.Product)
            : TransactionStatus.Pending;

        return new WithdrawTransaction
        {
            PaymentDisbursedDateTime = withdrawState.PaymentDisbursedDateTime,
            PaymentDisbursedAmount = withdrawState.PaymentDisbursedAmount,
            OtpRequestRef = withdrawState.OtpRequestRef,
            OtpRequestId = withdrawState.OtpRequestId,
            OtpConfirmedDateTime = withdrawState.OtpConfirmedDateTime,
            Amount = withdrawState.PaymentDisbursedAmount ?? requestedAmount,
            Id = withdrawState.CorrelationId,
            UserId = withdrawState.UserId,
            TransactionNo = withdrawState.TransactionNo,
            AccountCode = withdrawState.AccountCode,
            CustomerCode = withdrawState.CustomerCode,
            Product = withdrawState.Product,
            TransactionType = TransactionType.Withdraw,
            CurrentState = withdrawState.CurrentState,
            BankFee = withdrawState.BankFee,
            BankName = withdrawState.BankName,
            BankCode = withdrawState.BankCode,
            BankAccountNo = withdrawState.BankAccountNo,
            Currency = Currency.THB,
            Channel = withdrawState.Channel,
            CreatedAt = withdrawState.CreatedAt,
            FailedReason = withdrawState.FailedReason,
            Status = status,
            EffectiveDateTime = withdrawState.PaymentDisbursedDateTime
        };
    }

    public static WithdrawTransaction NewWithdrawTransaction(GlobalWithdrawTransaction globalWithdrawState)
    {
        if (globalWithdrawState.GlobalWalletTransferState?.CurrentState != GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.WithdrawalProcessing))
        {
            globalWithdrawState.WithdrawState.CurrentState = globalWithdrawState.GlobalWalletTransferState?.CurrentState;
            globalWithdrawState.WithdrawState.FailedReason = globalWithdrawState.GlobalWalletTransferState?.FailedReason;
        }

        return NewWithdrawTransaction(globalWithdrawState.WithdrawState);
    }

    public static WithdrawTransaction NewWithdrawTransaction(ThaiWithdrawTransaction thaiWithdrawTransaction)
    {
        if (thaiWithdrawTransaction.CashWithdrawState?.CurrentState != CashWithdrawMachineState.GetName(() => CashWithdrawMachineState.WithdrawalProcessing))
        {
            thaiWithdrawTransaction.WithdrawState.CurrentState = thaiWithdrawTransaction.CashWithdrawState?.CurrentState;
            thaiWithdrawTransaction.WithdrawState.FailedReason = thaiWithdrawTransaction.CashWithdrawState?.FailedReason;
        }

        return NewWithdrawTransaction(thaiWithdrawTransaction.WithdrawState, thaiWithdrawTransaction.CashWithdrawState?.RequestedAmount);
    }

    public static RefundTransaction NewRefundTransaction(RefundState refundState)
    {
        var status = refundState.CurrentState != null
            ? GetTransactionStatus(refundState.CurrentState, TransactionType.Refund, refundState.Product)
            : TransactionStatus.Pending;

        return new RefundTransaction
        {
            Amount = refundState.Amount,
            Id = refundState.CorrelationId,
            UserId = refundState.UserId,
            TransactionNo = refundState.TransactionNo,
            AccountCode = refundState.AccountCode,
            CustomerCode = refundState.CustomerCode,
            Product = refundState.Product,
            TransactionType = TransactionType.Refund,
            CurrentState = refundState.CurrentState,
            BankCode = refundState.BankCode,
            BankAccountNo = refundState.BankAccountNo,
            Currency = Currency.THB,
            Channel = refundState.Channel,
            CreatedAt = refundState.CreatedAt,
            FailedReason = refundState.FailedReason,
            Status = status,
            BankFee = refundState.BankFee,
            BankName = refundState.BankName,
            EffectiveDateTime = refundState.RefundedAt,
            DepositTransactionNo = refundState.DepositTransactionNo,
            RefundedAt = refundState.RefundedAt
        };
    }

    public static TransactionStatus GetTransactionStatus(string state, TransactionType transactionType, Product product)
    {
        var statuses = new Dictionary<TransactionStatus, List<string>>
        {
            {
                TransactionStatus.Success,
                new List<string>
                {
                    CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositCompleted),
                    // V2
                    WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawSucceed),
                    QrDepositMachineState.GetName(() => QrDepositMachineState.QrDepositCompleted),
                    OddDepositMachineState.GetName(() => OddDepositMachineState.OddDepositCompleted),
                    OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.OddWithdrawCompleted),
                    UpBackMachineState.GetName(() => UpBackMachineState.UpBackCompleted),
                    GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.GlobalTransferCompleted),
                    AtsDepositMachineState.GetName(() => AtsDepositMachineState.AtsDepositCompleted),
                    "Final",
                }
            },
            {
                TransactionStatus.Fail,
                new List<string>
                {
                    DepositMachineState.GetName(() => DepositMachineState.DepositRefundSucceed),
                    DepositMachineState.GetName(() => DepositMachineState.DepositFailed),
                    DepositMachineState.GetName(() => DepositMachineState.DepositPaymentNotReceived),
                    WithdrawMachineState.GetName(() => WithdrawMachineState.WithdrawalFailed),
                    WithdrawMachineState.GetName(() => WithdrawMachineState.RequestingOtpValidationFailed),
                    WithdrawMachineState.GetName(() => WithdrawMachineState.OtpValidationNotReceived),
                    GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed),
                    GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.GlobalWithdrawOtpValidationNotReceived),
                    GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.GlobalDepositPaymentNotReceived),
                    GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed),
                    CashWithdrawMachineState.GetName(() => CashWithdrawMachineState.RevertTransferSuccess),
                    CashWithdrawMachineState.GetName(() => CashWithdrawMachineState.CashWithdrawOtpValidationNotReceived),
                    // V2
                    DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.DepositFailed),
                    DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.DepositPaymentNotReceived),
                    WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawFailed),
                    WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.OtpValidationNotReceived),
                    QrDepositMachineState.GetName(() => QrDepositMachineState.QrDepositFailed),
                    OddDepositMachineState.GetName(() => OddDepositMachineState.OddDepositFailed),
                    AtsDepositMachineState.GetName(() => AtsDepositMachineState.AtsDepositFailed),
                    OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.OddWithdrawFailed),
                    UpBackMachineState.GetName(() => UpBackMachineState.UpBackFailed),
                    GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.GlobalTransferFailed),
                }
            }
        };

        switch (transactionType)
        {
            case TransactionType.Deposit:
                statuses[TransactionStatus.Fail] = statuses[TransactionStatus.Fail]
                    .Append(GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.RefundSucceed))
                    // TODO: remove when refund v2 is implemented
                    .Append(GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.RefundSucceed))
                    .ToList();
                break;
            case TransactionType.Refund:
                statuses[TransactionStatus.Success] = statuses[TransactionStatus.Success]
                    .Append(RefundMachineState.GetName(() => RefundMachineState.RefundSucceed))
                    .ToList();
                break;
        }

        if (product == Product.GlobalEquities)
        {
            statuses[TransactionStatus.Fail].AddRange(
                new List<string>
                {
                    GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.TransferRequestFailed),
                    GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.RevertTransferSuccess),
                    // TODO: remove when refund v2 is implemented
                    GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.TransferRequestFailed),
                    GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.RevertTransferSuccess)
                });
        }

        if (statuses[TransactionStatus.Success].Contains(state)) return TransactionStatus.Success;

        return statuses[TransactionStatus.Fail].Contains(state)
            ? TransactionStatus.Fail
            : TransactionStatus.Pending;
    }
}
