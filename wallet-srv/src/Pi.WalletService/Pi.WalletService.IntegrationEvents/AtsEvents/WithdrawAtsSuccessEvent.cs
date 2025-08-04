namespace Pi.WalletService.IntegrationEvents.AtsEvents;

public record WithdrawAtsSuccessEvent(
    Guid CorrelationId,
    decimal Amount);