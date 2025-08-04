namespace Pi.WalletService.IntegrationEvents;

public record OtpValidationNotReceived(Guid CorrelationId, string FailedReason);