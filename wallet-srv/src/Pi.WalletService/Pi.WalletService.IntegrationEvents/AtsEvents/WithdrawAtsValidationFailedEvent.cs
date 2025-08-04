namespace Pi.WalletService.IntegrationEvents.AtsEvents;

public record WithdrawAtsValidationFailedEvent(Guid CorrelationId, string FailedReason);