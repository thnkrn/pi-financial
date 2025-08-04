namespace Pi.WalletService.IntegrationEvents.AtsEvents;

public record WithdrawAtsFailedEvent(
    Guid CorrelationId,
    string FailedReason);