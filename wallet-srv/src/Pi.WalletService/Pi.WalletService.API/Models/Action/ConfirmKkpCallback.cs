namespace Pi.WalletService.API.Models.Action;

public record ConfirmKkpCallback(
    decimal PaymentReceivedAmount,
    DateTime PaymentReceivedDateTime,
    string BankCode,
    string BankAccountNo
);