namespace Pi.WalletService.IntegrationEvents.TradingAccountEvents;

public record CashDepositGatewayCallbackFailedEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal Amount,
    string ResultCode,
    string Reason
);

public record CashWithdrawGatewayCallbackFailedEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal Amount,
    string ResultCode,
    string Reason
);