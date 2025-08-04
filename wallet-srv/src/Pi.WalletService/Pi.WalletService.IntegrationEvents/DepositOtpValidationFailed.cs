namespace Pi.WalletService.IntegrationEvents;

public record DepositOtpValidationFailed(
    Guid CorrelationId,
    string FailedReason
    );
