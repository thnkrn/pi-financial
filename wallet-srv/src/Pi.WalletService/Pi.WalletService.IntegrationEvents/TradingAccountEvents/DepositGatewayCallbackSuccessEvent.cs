namespace Pi.WalletService.IntegrationEvents.TradingAccountEvents;

public record CashDepositGatewayCallbackSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal? Amount
);

public record CashWithdrawGatewayCallbackSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal? Amount
);