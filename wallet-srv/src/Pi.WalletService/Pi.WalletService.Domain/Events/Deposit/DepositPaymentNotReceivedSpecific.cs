namespace Pi.WalletService.Domain.Events.Deposit;

public record DepositPaymentNotReceivedSpecific(
    string TransactionNo,
    DateTime PaymentUpdatedDateTime,
    string FailedReason
);