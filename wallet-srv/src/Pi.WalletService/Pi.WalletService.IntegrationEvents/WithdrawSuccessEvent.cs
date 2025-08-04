namespace Pi.WalletService.IntegrationEvents;

public record WithdrawSuccessEvent(
    Guid TicketId,
    string UserId,
    string TransactionNo,
    string Product,
    decimal Amount
);