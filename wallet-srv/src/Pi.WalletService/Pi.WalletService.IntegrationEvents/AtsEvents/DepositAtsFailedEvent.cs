namespace Pi.WalletService.IntegrationEvents.AtsEvents;

public record DepositAtsFailedEvent(
    Guid CorrelationId,
    string FailedReason);
