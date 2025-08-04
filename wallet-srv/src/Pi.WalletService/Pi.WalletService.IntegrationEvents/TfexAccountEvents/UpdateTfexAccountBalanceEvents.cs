namespace Pi.WalletService.IntegrationEvents.TfexAccountEvents;

public record UpdateTfexAccountBalanceSuccessEvent(
    string UserId,
    string TransactionNo,
    string SetTradeAccountNo,
    decimal Amount
);