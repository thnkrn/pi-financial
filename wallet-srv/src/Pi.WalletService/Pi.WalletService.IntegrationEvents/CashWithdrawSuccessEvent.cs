namespace Pi.WalletService.IntegrationEvents;

public record CashWithdrawSuccessEvent(
    string UserId,
    string TransactionNo,
    DateTime PaymentDisbursedDateTime,
    string Product,
    decimal PaymentDisbursedAmount
);