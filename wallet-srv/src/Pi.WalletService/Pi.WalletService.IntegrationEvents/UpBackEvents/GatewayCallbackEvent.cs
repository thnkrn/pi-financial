namespace Pi.WalletService.IntegrationEvents.UpBackEvents;

public record GatewayCallbackSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal? Amount
);

public record GatewayCallbackFailedEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal Amount,
    string ResultCode
);
