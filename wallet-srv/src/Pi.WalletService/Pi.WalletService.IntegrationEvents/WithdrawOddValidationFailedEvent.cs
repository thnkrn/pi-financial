namespace Pi.WalletService.IntegrationEvents;

public record WithdrawOddValidationFailedEvent(Guid CorrelationId, string FailedReason);
