namespace Pi.WalletService.Domain.Events.Deposit;

public record DepositPaymentFailed(
    string TransactionNo,
    decimal BankFee,
    decimal PaymentReceivedAmount,
    DateTime PaymentReceivedDateTime,
    string CustomerName,
    string BankAccountName,
    string BankName,
    string BankAccountNo,
    string FailedReason
);