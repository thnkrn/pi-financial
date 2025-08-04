namespace Pi.WalletService.Domain.Events.Deposit;

public enum MockQrDepositEventState
{
    DepositCompleted,
    DepositFailed,
    DepositFailedNameMismatch,
    DepositFailedInvalidSource,
    InvalidPaymentDateTime
}

public enum MockQrDepositEventStateV2
{
    QrDepositCompleted,
    QrDepositFailed,
    FailedNameMismatch,
    FailedAmountMismatch,
    FailedInvalidSource,
    InvalidPaymentDateTime
}
