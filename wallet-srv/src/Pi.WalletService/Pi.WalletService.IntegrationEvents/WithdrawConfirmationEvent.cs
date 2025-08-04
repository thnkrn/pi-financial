namespace Pi.WalletService.IntegrationEvents;

public record WithdrawConfirmationReceived(
    string TransactionNo,
    decimal ConfirmAmount,
    decimal BankFee
);
