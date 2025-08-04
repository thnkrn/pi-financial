namespace Pi.WalletService.IntegrationEvents;

public record UpdateTransactionStatusSuccessEvent(Guid CorrelationId);

public record UpdateTransactionStatusFailedEvent(Guid CorrelationId);