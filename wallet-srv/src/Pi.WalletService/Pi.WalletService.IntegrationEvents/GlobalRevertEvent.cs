namespace Pi.WalletService.IntegrationEvents;

public record GlobalRevertEvent(
    Guid CorrelationId
);