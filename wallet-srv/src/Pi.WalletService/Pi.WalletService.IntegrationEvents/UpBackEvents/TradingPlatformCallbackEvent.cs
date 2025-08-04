namespace Pi.WalletService.IntegrationEvents.UpBackEvents;

public record TradingPlatformCallbackSuccessEvent(
    string UserId,
    string TransactionNo,
    string SetTradeAccountNo,
    decimal Amount
);
