namespace Pi.WalletService.IntegrationEvents;

public record GlobalDepositFailedEvent(Guid TicketId, string UserId, string TransactionNo);