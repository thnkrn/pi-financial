namespace Pi.WalletService.IntegrationEvents;

public record CashDepositFailedEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    string Product,
    decimal Amount
);