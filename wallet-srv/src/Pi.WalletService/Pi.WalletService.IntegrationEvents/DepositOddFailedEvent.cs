namespace Pi.WalletService.IntegrationEvents;

public record DepositOddFailedEvent(
    Guid CorrelationId,
    string Reason,
    bool ShouldNotify = false,
    string? FailedCode = null
    );
