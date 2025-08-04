namespace Pi.WalletService.IntegrationEvents;

public record DepositOddSuccessEvent(
    Guid CorrelationId,
    string UserId,
    string Product,
    decimal NetAmount
);
