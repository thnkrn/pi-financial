namespace Pi.WalletService.IntegrationEvents;

public record WithdrawFailedEvent(Guid TicketId, string UserId, string TransactionNo, string Product, decimal Amount, string Reason);