namespace Pi.WalletService.IntegrationEvents;

public record CashWithdrawFailedEvent(
    string UserId,
    string TransactionNo,
    string Product,
    decimal Amount
);