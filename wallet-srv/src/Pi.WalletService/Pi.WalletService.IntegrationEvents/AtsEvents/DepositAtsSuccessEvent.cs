namespace Pi.WalletService.IntegrationEvents.AtsEvents;

public record DepositAtsSuccessEvent(
    Guid CorrelationId,
    decimal Amount
);
