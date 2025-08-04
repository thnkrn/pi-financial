namespace Pi.WalletService.IntegrationEvents.AtsEvents;

public record AtsGatewayCallbackSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal Amount
);

public record AtsGatewayCallbackFailedEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    DateTime TransactionDateTime,
    string Product,
    decimal Amount,
    string ResultCode
);
