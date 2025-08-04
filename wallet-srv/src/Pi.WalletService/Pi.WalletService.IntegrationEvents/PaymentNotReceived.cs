namespace Pi.WalletService.IntegrationEvents;

public record PaymentNotReceived(Guid CorrelationId, string FailedReason);
