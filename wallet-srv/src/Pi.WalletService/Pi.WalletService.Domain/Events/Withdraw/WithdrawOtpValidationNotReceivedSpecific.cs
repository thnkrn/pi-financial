namespace Pi.WalletService.Domain.Events.Withdraw;

public record WithdrawOtpValidationNotReceivedSpecific(
    string TransactionNo,
    DateTime PaymentUpdatedDateTime,
    string FailedReason
);