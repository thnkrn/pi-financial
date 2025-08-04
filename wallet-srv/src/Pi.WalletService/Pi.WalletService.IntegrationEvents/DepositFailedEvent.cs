namespace Pi.WalletService.IntegrationEvents;

public record DepositFailedEvent(Guid TicketId, string UserId, string TransactionNo, string Product, decimal Amount);