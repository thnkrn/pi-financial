namespace Pi.WalletService.IntegrationEvents;

public record GlobalDepositSuccessEvent(Guid TicketId, string UserId, string TransactionNo);