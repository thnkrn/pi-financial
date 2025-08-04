namespace Pi.WalletService.Domain.Events.Deposit;

public record DepositPaymentCallbackReceived(
    string TransactionNo,
    decimal BankFee,
    decimal PaymentReceivedAmount,
    DateTime PaymentReceivedDateTime,
    string BankAccountName,
    string BankName,
    string BankShortName,
    string BankCode,
    string BankAccountNo
);