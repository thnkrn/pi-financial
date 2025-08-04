namespace Pi.WalletService.IntegrationEvents;

public record NonGlobalWithdrawFailedEvent(Guid TicketId, string UserId, string TransactionNo, string Product, decimal Amount, string Reason);