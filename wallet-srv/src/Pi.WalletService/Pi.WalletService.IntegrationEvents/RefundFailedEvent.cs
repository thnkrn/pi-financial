namespace Pi.WalletService.IntegrationEvents;

public record RefundFailedEvent(Guid CorrelationId, string DepositTransactionNo);
